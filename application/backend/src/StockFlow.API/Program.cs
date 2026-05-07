using StockFlow.API.Middleware;
using StockFlow.API.HealthChecks;
using StockFlow.Application;
using StockFlow.Infrastructure;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.BackgroundJobs;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting StockFlow Pro API...");

    var builder = WebApplication.CreateBuilder(args);

    // Railway: Use PORT environment variable if present
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://*:{port}");

    // Add Serilog
    builder.Host.UseSerilog();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("https://stockflow-pro.infinityfreeapp.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();

    // Add Application services (MediatR, Mapster, FluentValidation)
    builder.Services.AddApplication();

    // Add Infrastructure services (DbContext, Repositories, Identity, JWT)
    builder.Services.AddInfrastructure(builder.Configuration);

    // Add Hangfire with PostgreSQL
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => 
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = builder.Configuration.GetValue<int>("HangfireSettings:WorkerCount");
    });

    // Register background job classes
    builder.Services.AddScoped<InventoryJobs>();
    builder.Services.AddScoped<TokenCleanupJobs>();

    // Add API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"),
            new QueryStringApiVersionReader("api-version"));
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Add Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        // Global policy: 100 requests per minute per IP
        options.AddFixedWindowLimiter("global", limiterOptions =>
        {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 5;
        });

        // Auth policy: 5 login attempts per minute per IP
        options.AddSlidingWindowLimiter("auth", limiterOptions =>
        {
            limiterOptions.PermitLimit = 5;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.SegmentsPerWindow = 2;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 2;
        });

        // Strict policy: 10 requests per minute per IP (for sensitive operations)
        options.AddFixedWindowLimiter("strict", limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 2;
        });

        // Token bucket policy: 20 tokens per minute (for burst traffic)
        options.AddTokenBucketLimiter("token", limiterOptions =>
        {
            limiterOptions.TokenLimit = 20;
            limiterOptions.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
            limiterOptions.TokensPerPeriod = 20;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 5;
        });

        // Concurrency policy: Max 10 concurrent requests per IP
        options.AddConcurrencyLimiter("concurrency", limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            limiterOptions.QueueLimit = 5;
        });

        // Global limiter - applied to all endpoints by default
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            });
        });

        // Custom rejection response
        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            
            TimeSpan? retryAfter = null;
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue))
            {
                retryAfter = retryAfterValue;
                context.HttpContext.Response.Headers.RetryAfter = retryAfterValue.TotalSeconds.ToString();
            }

            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = "Too many requests. Please try again later.",
                retryAfterSeconds = retryAfter?.TotalSeconds
            }, cancellationToken);
        };
    });

    // Add Health Checks
    var redisConnection = builder.Configuration.GetConnectionString("Redis");
    
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
            name: "postgresql",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "postgresql", "database" })
        .AddCheck<HangfireHealthCheck>(
            "hangfire",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "hangfire", "background-jobs" });

    // Add Redis health check only if Redis is configured
    if (!string.IsNullOrEmpty(redisConnection))
    {
        builder.Services.AddHealthChecks()
            .AddRedis(
                redisConnectionString: redisConnection,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "cache", "redis" });
    }

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "StockFlow Pro API v1",
        Description = "A comprehensive inventory and stock management system - Version 1",
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "StockFlow Pro API v2",
        Description = "A comprehensive inventory and stock management system - Version 2 (Enhanced features)",
    });

    // Add JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

    var app = builder.Build();

    // Initialize database and seed roles
    await DbInitializer.InitializeAsync(app.Services);

    // Configure the HTTP request pipeline.
    app.UseSerilogRequestLogging(); // Add Serilog HTTP request logging
    
    // Enable CORS
    app.UseCors("AllowAll");
    
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "StockFlow Pro API v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "StockFlow Pro API v2");
        });
    }

    app.UseHttpsRedirection();

    // Add Rate Limiting middleware
    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    // Add Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
        DashboardTitle = "StockFlow Pro Background Jobs"
    });

    app.MapControllers();

    // Map Health Checks
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("db"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false // No checks, always returns healthy
    });

    // Schedule recurring jobs
    RecurringJob.AddOrUpdate<InventoryJobs>(
        "check-low-stock",
        job => job.CheckLowStockLevels(),
        Cron.Daily(9)); // Run daily at 9 AM

    RecurringJob.AddOrUpdate<TokenCleanupJobs>(
        "cleanup-expired-tokens",
        job => job.CleanupExpiredTokens(),
        Cron.Daily(2)); // Run daily at 2 AM

    Log.Information("StockFlow Pro API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible to integration tests
public partial class Program { }

# Hangfire Background Jobs - Implementation Guide

## Overview
Hangfire is a background job processing framework for .NET that enables fire-and-forget, delayed, recurring, and continuations jobs. This implementation uses SQL Server for persistent job storage.

## Installation

```bash
# Install Hangfire packages
dotnet add package Hangfire.AspNetCore --version 1.8.9
dotnet add package Hangfire.SqlServer --version 1.8.9
dotnet add package Hangfire.Core --version 1.8.9 # For Infrastructure layer
```

## Configuration

### appsettings.json
```json
{
  "HangfireSettings": {
    "DashboardTitle": "StockFlow Pro Background Jobs",
    "DashboardPath": "/hangfire",
    "WorkerCount": 5
  }
}
```

### Program.cs
```csharp
using Hangfire;
using Hangfire.SqlServer;
using StockFlow.Infrastructure.BackgroundJobs;

// Add Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = builder.Configuration.GetValue<int>("HangfireSettings:WorkerCount");
});

// Register background job classes
builder.Services.AddScoped<InventoryJobs>();
builder.Services.AddScoped<TokenCleanupJobs>();

// Add Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "StockFlow Pro Background Jobs"
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
```

## Background Job Classes

### 1. InventoryJobs
**Purpose**: Monitor and alert on low stock levels

**Location**: `StockFlow.Infrastructure/BackgroundJobs/InventoryJobs.cs`

**Features**:
- Checks all inventory items against their threshold
- Logs warnings for low stock items
- Can be extended to send email notifications

**Schedule**: Daily at 9 AM

```csharp
public async Task CheckLowStockLevels()
{
    var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync();
    
    if (lowStockItems.Any())
    {
        _logger.LogWarning("Found {Count} low stock items", lowStockItems.Count());
        // Send notifications here
    }
}
```

### 2. TokenCleanupJobs
**Purpose**: Clean up expired and revoked refresh tokens

**Location**: `StockFlow.Infrastructure/BackgroundJobs/TokenCleanupJobs.cs`

**Features**:
- Removes expired refresh tokens from database
- Removes revoked tokens
- Keeps database clean and performant

**Schedule**: Daily at 2 AM

```csharp
public async Task CleanupExpiredTokens()
{
    var allTokens = await _refreshTokenRepository.GetAllAsync();
    var expiredTokens = allTokens
        .Where(t => t.ExpiresAt < DateTime.UtcNow || t.RevokedAt != null)
        .ToList();

    foreach (var token in expiredTokens)
    {
        await _refreshTokenRepository.DeleteAsync(token);
    }

    await _unitOfWork.SaveChangesAsync();
}
```

## Job Types

### 1. Fire-and-Forget Jobs
Execute once, immediately in the background:
```csharp
BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget job!"));
```

### 2. Delayed Jobs
Execute once after a specified delay:
```csharp
BackgroundJob.Schedule(() => Console.WriteLine("Delayed job!"), TimeSpan.FromMinutes(5));
```

### 3. Recurring Jobs
Execute on a schedule using CRON expressions:
```csharp
RecurringJob.AddOrUpdate(
    "job-id",
    () => Console.WriteLine("Recurring job!"),
    Cron.Daily(9)); // Daily at 9 AM
```

### 4. Continuation Jobs
Execute after another job completes:
```csharp
var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("First job"));
BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("Continuation job!"));
```

## CRON Expressions

Common schedules:
- `Cron.Minutely()` - Every minute
- `Cron.Hourly()` - Every hour
- `Cron.Daily()` - Every day at midnight
- `Cron.Daily(9)` - Every day at 9 AM
- `Cron.Weekly()` - Every week on Monday at midnight
- `Cron.Monthly()` - First day of month at midnight
- `Cron.Yearly()` - January 1st at midnight

Custom CRON:
```csharp
// Every 5 minutes
RecurringJob.AddOrUpdate("job-id", () => DoWork(), "*/5 * * * *");

// Every weekday at 8 AM
RecurringJob.AddOrUpdate("job-id", () => DoWork(), "0 8 * * 1-5");
```

## Dashboard

### Access
- URL: `http://localhost:5057/hangfire`
- Authorization: Localhost (all users) | Production (Admin role only)

### Features
1. **Jobs**: View all job types (enqueued, scheduled, processing, succeeded, failed)
2. **Recurring Jobs**: Manage scheduled jobs, trigger manually
3. **Servers**: Monitor Hangfire server instances
4. **Retries**: View and manage failed jobs
5. **Statistics**: Real-time job processing metrics

### Dashboard Authorization

```csharp
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow access in development (localhost)
        if (httpContext.Request.Host.Host.Contains("localhost"))
        {
            return true;
        }

        // In production, require authenticated Admin user
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}
```

## IBackgroundJobService Interface

Abstraction for job scheduling:

```csharp
public interface IBackgroundJobService
{
    void EnqueueJob(Action job);
    void EnqueueJob<T>(Action<T> job);
    void ScheduleJob(Action job, TimeSpan delay);
    void RecurringJob(string jobId, Action job, string cronExpression);
}
```

Usage in application:
```csharp
public class ProductService
{
    private readonly IBackgroundJobService _jobService;

    public async Task<Product> CreateProduct(CreateProductCommand command)
    {
        var product = await _repository.AddAsync(product);
        
        // Enqueue background job to update cache
        _jobService.EnqueueJob(() => UpdateProductCache(product.Id));
        
        return product;
    }
}
```

## Database Tables

Hangfire creates several tables in SQL Server:
- `HangFire.Job` - Job metadata
- `HangFire.State` - Job state transitions
- `HangFire.JobParameter` - Job parameters
- `HangFire.JobQueue` - Job queue
- `HangFire.Server` - Server instances
- `HangFire.Hash`, `HangFire.List`, `HangFire.Set`, `HangFire.Counter` - Supporting tables
- `HangFire.AggregatedCounter` - Statistics

## Storage Options

**SQL Server** (Current):
- Persistent storage
- Supports distributed processing
- Automatic retries
- Schema: `HangFire`

**Other Options**:
- Redis
- In-Memory (for testing)
- MongoDB
- PostgreSQL

## Best Practices

### 1. Job Design
- ✅ Keep jobs idempotent (safe to retry)
- ✅ Use dependency injection for job classes
- ✅ Log job execution with Serilog
- ✅ Handle exceptions gracefully
- ❌ Avoid long-running jobs (> 1 hour)
- ❌ Don't pass large objects as parameters

### 2. Monitoring
- Monitor dashboard regularly
- Set up alerts for failed jobs
- Track job execution time
- Monitor storage growth

### 3. Performance
- Configure appropriate worker count
- Use job batches for bulk operations
- Clean up old jobs periodically
- Monitor database size

### 4. Error Handling
```csharp
public async Task ProcessData()
{
    try
    {
        _logger.LogInformation("Starting data processing...");
        
        // Job logic here
        
        _logger.LogInformation("Data processing completed");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing data");
        throw; // Hangfire will retry
    }
}
```

## Scheduled Jobs in StockFlow Pro

| Job ID | Class | Method | Schedule | Purpose |
|--------|-------|--------|----------|---------|
| `check-low-stock` | `InventoryJobs` | `CheckLowStockLevels()` | Daily 9 AM | Check inventory levels and alert |
| `cleanup-expired-tokens` | `TokenCleanupJobs` | `CleanupExpiredTokens()` | Daily 2 AM | Remove expired refresh tokens |

## Adding New Jobs

1. **Create Job Class**:
```csharp
public class EmailJobs
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailJobs> _logger;

    public EmailJobs(IEmailService emailService, ILogger<EmailJobs> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendDailyReport()
    {
        _logger.LogInformation("Generating daily report...");
        await _emailService.SendReportAsync();
    }
}
```

2. **Register in DI Container** (Program.cs):
```csharp
builder.Services.AddScoped<EmailJobs>();
```

3. **Schedule the Job** (Program.cs):
```csharp
RecurringJob.AddOrUpdate<EmailJobs>(
    "send-daily-report",
    job => job.SendDailyReport(),
    Cron.Daily(17)); // 5 PM daily
```

## Testing Jobs

### Manual Trigger
Use the dashboard to trigger jobs manually for testing.

### Unit Testing
```csharp
[Fact]
public async Task CheckLowStockLevels_ShouldLogWarning_WhenLowStockExists()
{
    // Arrange
    var mockRepo = new Mock<IInventoryRepository>();
    var mockLogger = new Mock<ILogger<InventoryJobs>>();
    
    var lowStock = new List<Inventory> 
    { 
        new Inventory { Quantity = 5, Threshold = 10 } 
    };
    
    mockRepo.Setup(r => r.GetLowStockItemsAsync())
        .ReturnsAsync(lowStock);
    
    var job = new InventoryJobs(mockRepo.Object, mockLogger.Object);
    
    // Act
    await job.CheckLowStockLevels();
    
    // Assert
    mockLogger.Verify(
        x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("low stock")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

## Troubleshooting

### Jobs Not Running
1. Check Hangfire Server is running (dashboard shows server)
2. Verify database connection
3. Check logs for exceptions
4. Ensure job class is registered in DI

### Dashboard Not Accessible
1. Verify URL: `/hangfire`
2. Check authorization filter
3. Ensure middleware is registered

### Performance Issues
1. Reduce worker count
2. Optimize job logic
3. Clean up old jobs
4. Monitor database size

## Production Considerations

1. **Security**:
   - Restrict dashboard access (Admin role only)
   - Use HTTPS
   - Secure database connection strings

2. **Scaling**:
   - Multiple servers can process jobs
   - Configure worker count based on load
   - Use dedicated job processing servers

3. **Monitoring**:
   - Set up alerts for failed jobs
   - Monitor job execution time
   - Track storage growth

4. **Maintenance**:
   - Clean up succeeded jobs older than 7 days
   - Archive old job data
   - Monitor database size

## Resources

- **Hangfire Documentation**: https://docs.hangfire.io
- **Dashboard**: http://localhost:5057/hangfire
- **CRON Expression Tool**: https://crontab.guru
- **NuGet Package**: https://www.nuget.org/packages/Hangfire.AspNetCore

## Summary

✅ Hangfire installed and configured  
✅ SQL Server storage configured  
✅ Dashboard available at `/hangfire`  
✅ 2 recurring jobs scheduled  
✅ Background job service interface  
✅ Serilog integration for logging  
✅ Authorization filter implemented  
✅ Database tables auto-created  

The background job system is fully operational and ready for production use!

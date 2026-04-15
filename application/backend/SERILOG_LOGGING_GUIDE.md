# Serilog Structured Logging Guide

## 🎯 Overview
StockFlow Pro implements structured logging using **Serilog** with multiple sinks (Console, File) for comprehensive application monitoring and debugging.

---

## ✅ Features Implemented

### 1. **Dual Logging Sinks**
- ✅ **Console Sink** - Formatted output for development
- ✅ **File Sink** - Persistent logs with daily rotation
- ✅ **Structured Logging** - Machine-readable JSON-like format
- ✅ **HTTP Request Logging** - Automatic request/response logging

### 2. **Log Enrichment**
- ✅ Machine Name
- ✅ Thread ID
- ✅ Trace ID (for error tracking)
- ✅ Application Name
- ✅ Environment

### 3. **Logging in Services**
- ✅ AuthService - Login, Registration, Token operations
- ✅ GlobalExceptionHandlingMiddleware - Error tracking with TraceId

---

## 📋 Configuration

### appsettings.json
```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/stockflow-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}",
          "fileSizeLimitBytes": 10485760,
          "rollOnFileSizeLimit": true
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "StockFlow Pro API",
      "Environment": "Development"
    }
  }
}
```

### Program.cs Setup
```csharp
using Serilog;

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
    builder.Host.UseSerilog(); // Add Serilog

    // ... services configuration

    var app = builder.Build();
    
    app.UseSerilogRequestLogging(); // HTTP request logging
    
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
```

---

## 📝 Log Levels

| Level | Usage | Example |
|-------|-------|---------|
| **Verbose** | Detailed trace information | Internal state changes |
| **Debug** | Debugging information | Method entry/exit |
| **Information** | General informational messages | User login, API startup |
| **Warning** | Warning messages | Deprecated API usage |
| **Error** | Error messages | Exceptions, failures |
| **Fatal** | Critical failures | Application crash |

---

## 💡 Usage Examples

### 1. **In Services (with Dependency Injection)**
```csharp
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILogger<AuthService> logger)
    {
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

        // ... login logic

        _logger.LogInformation("User logged in successfully: {UserId}, {Email}", 
            user.Id, user.Email);
        
        return authResponse;
    }
}
```

### 2. **In Controllers**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        _logger.LogDebug("Fetching product with ID: {ProductId}", id);
        
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", id);
            return NotFound();
        }
        
        return Ok(product);
    }
}
```

### 3. **Error Logging with Context**
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, 
        "Failed to process order. OrderId: {OrderId}, UserId: {UserId}", 
        orderId, userId);
    throw;
}
```

### 4. **Structured Logging with Properties**
```csharp
using (LogContext.PushProperty("UserId", userId))
using (LogContext.PushProperty("CorrelationId", correlationId))
{
    _logger.LogInformation("Processing payment for order: {OrderId}", orderId);
    // All logs within this scope will include UserId and CorrelationId
}
```

---

## 📂 Log Files

### File Naming Convention
```
Logs/stockflow-YYYYMMDD.log
```

Examples:
- `Logs/stockflow-20260415.log`
- `Logs/stockflow-20260416.log`

### File Rotation Settings
- **Rolling Interval**: Daily (new file each day)
- **Retention**: 30 days (older logs automatically deleted)
- **File Size Limit**: 10 MB per file
- **Roll on Size Limit**: Yes (creates new file if size exceeded)

---

## 🔍 Log Output Examples

### Console Output (Development)
```
[19:22:34 INF]  - Starting StockFlow Pro API...
[19:22:36 INF] StockFlow.Infrastructure.Persistence.ApplicationDbContext - Database initialized successfully
[19:22:36 INF]  - StockFlow Pro API started successfully
[19:22:45 INF] StockFlow.Infrastructure.Services.AuthService - Login attempt for email: admin@stockflowpro.com
[19:22:45 INF] StockFlow.Infrastructure.Services.AuthService - User logged in successfully: a7b3c4d5-e6f7-8901-2345-678901234567, admin@stockflowpro.com
```

### File Output (Logs/stockflow-20260415.log)
```
2026-04-15 19:22:34.364 +05:30 [INF]  - Starting StockFlow Pro API...
2026-04-15 19:22:36.166 +05:30 [INF] Microsoft.EntityFrameworkCore.Migrations - No migrations were applied. The database is already up to date.
2026-04-15 19:22:36.628 +05:30 [INF] StockFlow.Infrastructure.Persistence.ApplicationDbContext - Database initialized successfully
2026-04-15 19:22:36.750 +05:30 [INF]  - StockFlow Pro API started successfully
2026-04-15 19:22:45.123 +05:30 [INF] StockFlow.Infrastructure.Services.AuthService - Login attempt for email: admin@stockflowpro.com
2026-04-15 19:22:45.456 +05:30 [INF] StockFlow.Infrastructure.Services.AuthService - User logged in successfully: a7b3c4d5-e6f7-8901-2345-678901234567, admin@stockflowpro.com
```

### HTTP Request Logging
```
[19:23:10 INF] HTTP POST /api/auth/login responded 200 in 234.5678 ms
[19:23:15 INF] HTTP GET /api/products responded 200 in 45.1234 ms
[19:23:20 INF] HTTP POST /api/products responded 201 in 156.7890 ms
```

### Error Logging with TraceId
```
2026-04-15 19:25:30.123 +05:30 [ERR] StockFlow.API.Middleware.GlobalExceptionHandlingMiddleware - Unhandled exception occurred. TraceId: 0HN4G8K9J7F6E5D4C3B2A1, Path: /api/products, Method: POST
System.InvalidOperationException: Product with SKU 'LAP-001' already exists
   at StockFlow.Application.Products.Commands.CreateProductCommandHandler.Handle(CreateProductCommand request, CancellationToken cancellationToken)
```

---

## 🎯 Currently Logged Events

### Authentication & Authorization
- ✅ User registration attempts
- ✅ Login attempts (success/failure)
- ✅ Token refresh operations
- ✅ Token revocation
- ✅ Invalid credentials warnings
- ✅ Inactive account warnings

### Application Lifecycle
- ✅ Application startup
- ✅ Application shutdown
- ✅ Database initialization
- ✅ Migration status

### HTTP Requests
- ✅ Request path, method, status code
- ✅ Response time (ms)
- ✅ Query parameters

### Errors & Exceptions
- ✅ Unhandled exceptions with stack trace
- ✅ Validation errors
- ✅ Not found errors
- ✅ Unauthorized access attempts
- ✅ TraceId for correlation

---

## 🔧 Advanced Configuration

### 1. **Add Additional Sinks**

#### SQL Server Sink
```bash
dotnet add package Serilog.Sinks.MSSqlServer
```

```json
{
  "Name": "MSSqlServer",
  "Args": {
    "connectionString": "DefaultConnection",
    "tableName": "Logs",
    "autoCreateSqlTable": true
  }
}
```

#### Seq (Structured Log Server)
```bash
dotnet add package Serilog.Sinks.Seq
```

```json
{
  "Name": "Seq",
  "Args": {
    "serverUrl": "http://localhost:5341"
  }
}
```

#### Azure Application Insights
```bash
dotnet add package Serilog.Sinks.ApplicationInsights
```

### 2. **Filter Logs by Namespace**
```json
"Override": {
  "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
  "StockFlow.Infrastructure": "Debug",
  "StockFlow.Application": "Information"
}
```

### 3. **Add Custom Enrichers**
```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Version", "1.0.0")
    .Enrich.WithEnvironmentName()
    .Enrich.WithClientIp()
    .CreateLogger();
```

---

## 📊 Log Analysis

### View Today's Logs
```powershell
Get-Content Logs/stockflow-20260415.log | Select-String "ERR"
```

### Count Error Logs
```powershell
(Get-Content Logs/stockflow-20260415.log | Select-String "ERR").Count
```

### Find Specific User Activity
```powershell
Get-Content Logs/stockflow-20260415.log | Select-String "admin@stockflowpro.com"
```

### Tail Live Logs
```powershell
Get-Content Logs/stockflow-20260415.log -Wait -Tail 50
```

---

## 🚀 Production Recommendations

1. **Use Separate Log Files by Level**
   ```json
   {
     "Name": "File",
     "Args": {
       "path": "Logs/errors-.log",
       "restrictedToMinimumLevel": "Error"
     }
   }
   ```

2. **Send Errors to Centralized Service**
   - Use Seq, ELK Stack, or Azure Application Insights
   - Enable alerts for critical errors

3. **Reduce Verbosity in Production**
   ```json
   "MinimumLevel": {
     "Default": "Warning",
     "Override": {
       "StockFlow": "Information"
     }
   }
   ```

4. **Add Performance Metrics**
   ```csharp
   using (LogContext.PushProperty("ElapsedMs", stopwatch.ElapsedMilliseconds))
   {
       _logger.LogInformation("Query completed");
   }
   ```

5. **Implement Log Sampling for High Traffic**
   - Sample 10% of requests in production
   - Always log errors and warnings

---

## ✅ Benefits

| Feature | Benefit |
|---------|---------|
| Structured Logging | Easy to parse and query |
| Multiple Sinks | Console for dev, Files for production |
| Daily Rotation | Automatic log file management |
| Enrichment | Additional context (Machine, Thread, etc.) |
| HTTP Logging | Track API performance |
| TraceId | Correlate errors with requests |
| Retention Policy | Automatic cleanup of old logs |

---

## 🔗 Resources

- [Serilog Documentation](https://serilog.net/)
- [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
- [Structured Logging Concepts](https://messagetemplates.org/)

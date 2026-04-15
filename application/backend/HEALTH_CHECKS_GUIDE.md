# Health Checks - Implementation Guide

## Overview
ASP.NET Core Health Checks provide a way to monitor the health of your application and its dependencies. This implementation monitors SQL Server, Hangfire, and Redis (if configured).

## Installation

```bash
# Install health check packages
dotnet add package AspNetCore.HealthChecks.SqlServer --version 8.0.0
dotnet add package AspNetCore.HealthChecks.Redis --version 8.0.0
dotnet add package AspNetCore.HealthChecks.UI.Client --version 8.0.0
```

## Configuration

### Program.cs
```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

// Add Health Checks
var redisConnection = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sqlserver",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "sqlserver" })
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

// Map Health Check endpoints
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
```

## Health Check Endpoints

### 1. `/health` - Overall Health
**Purpose**: Checks all configured health checks  
**Use Case**: General health monitoring, load balancer health probes

**Response Example** (Healthy):
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "sqlserver": {
      "status": "Healthy",
      "duration": "00:00:00.0567890",
      "tags": ["db", "sql", "sqlserver"]
    },
    "hangfire": {
      "status": "Healthy",
      "duration": "00:00:00.0123456",
      "data": {
        "servers": 1,
        "queues": 1,
        "enqueued": 0,
        "scheduled": 2,
        "processing": 0,
        "succeeded": 5,
        "failed": 0
      },
      "tags": ["hangfire", "background-jobs"]
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0234567",
      "tags": ["cache", "redis"]
    }
  }
}
```

**Response Example** (Unhealthy):
```json
{
  "status": "Unhealthy",
  "totalDuration": "00:00:00.5432101",
  "entries": {
    "sqlserver": {
      "status": "Unhealthy",
      "duration": "00:00:00.5000000",
      "exception": "Connection timeout...",
      "tags": ["db", "sql", "sqlserver"]
    }
  }
}
```

### 2. `/health/ready` - Readiness Probe
**Purpose**: Checks if the application is ready to accept requests  
**Use Case**: Kubernetes readiness probes, deployment validation

**Checks**: Only database health (tagged with "db")

**Response** (Ready):
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0567890",
  "entries": {
    "sqlserver": {
      "status": "Healthy",
      "duration": "00:00:00.0567890",
      "tags": ["db", "sql", "sqlserver"]
    }
  }
}
```

### 3. `/health/live` - Liveness Probe
**Purpose**: Checks if the application is alive (not deadlocked)  
**Use Case**: Kubernetes liveness probes

**Checks**: None (always returns Healthy if API responds)

**Response**:
```json
{
  "status": "Healthy"
}
```

## Custom Health Checks

### HangfireHealthCheck
**Location**: `StockFlow.API/HealthChecks/HangfireHealthCheck.cs`

```csharp
public class HangfireHealthCheck : IHealthCheck
{
    private readonly ILogger<HangfireHealthCheck> _logger;

    public HangfireHealthCheck(ILogger<HangfireHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var storage = JobStorage.Current;
            var monitoringApi = storage.GetMonitoringApi();
            var stats = monitoringApi.GetStatistics();
            
            var data = new Dictionary<string, object>
            {
                { "servers", stats.Servers },
                { "queues", stats.Queues },
                { "enqueued", stats.Enqueued },
                { "scheduled", stats.Scheduled },
                { "processing", stats.Processing },
                { "succeeded", stats.Succeeded },
                { "failed", stats.Failed }
            };

            if (stats.Servers == 0)
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded("No Hangfire servers running", null, data));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy("Hangfire is running successfully", data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hangfire health check failed");
            return Task.FromResult(
                HealthCheckResult.Unhealthy("Hangfire health check failed", ex));
        }
    }
}
```

## Health Status Values

| Status | HTTP Code | Meaning | Action |
|--------|-----------|---------|--------|
| `Healthy` | 200 | All checks passed | None |
| `Degraded` | 200 | Some non-critical checks failed | Monitor, may need attention |
| `Unhealthy` | 503 | Critical checks failed | Immediate action required |

## Configured Health Checks

### 1. SQL Server Health Check
- **Name**: `sqlserver`
- **Type**: Built-in (AspNetCore.HealthChecks.SqlServer)
- **Connection**: Azure SQL Database
- **Failure Status**: Unhealthy (critical)
- **Tags**: `db`, `sql`, `sqlserver`
- **What it checks**: Database connectivity, executes `SELECT 1`

### 2. Hangfire Health Check
- **Name**: `hangfire`
- **Type**: Custom (HangfireHealthCheck)
- **Failure Status**: Degraded (non-critical)
- **Tags**: `hangfire`, `background-jobs`
- **What it checks**:
  - Hangfire server is running
  - Job statistics (enqueued, scheduled, processing, succeeded, failed)
- **Returns Degraded when**: No Hangfire servers running
- **Returns Unhealthy when**: Exception accessing Hangfire

### 3. Redis Health Check (Optional)
- **Name**: `redis`
- **Type**: Built-in (AspNetCore.HealthChecks.Redis)
- **Connection**: Configured only if Redis connection string exists
- **Failure Status**: Degraded (non-critical)
- **Tags**: `cache`, `redis`
- **What it checks**: Redis connectivity, PING command
- **Note**: If Redis is not configured, this check is skipped

## Usage in Different Environments

### Development
```bash
# Test health endpoint
curl http://localhost:5057/health

# Test readiness
curl http://localhost:5057/health/ready

# Test liveness
curl http://localhost:5057/health/live
```

### Kubernetes Deployment
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: stockflow-api
spec:
  containers:
  - name: api
    image: stockflow-api:latest
    ports:
    - containerPort: 5057
    livenessProbe:
      httpGet:
        path: /health/live
        port: 5057
      initialDelaySeconds: 30
      periodSeconds: 10
      timeoutSeconds: 5
      failureThreshold: 3
    readinessProbe:
      httpGet:
        path: /health/ready
        port: 5057
      initialDelaySeconds: 10
      periodSeconds: 5
      timeoutSeconds: 3
      failureThreshold: 3
```

### Docker Compose
```yaml
version: '3.8'
services:
  api:
    image: stockflow-api:latest
    ports:
      - "5057:5057"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5057/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

### Load Balancer Configuration
**AWS Application Load Balancer**:
- Health Check Path: `/health/ready`
- Health Check Interval: 30 seconds
- Healthy Threshold: 2
- Unhealthy Threshold: 3
- Timeout: 5 seconds

**Azure Application Gateway**:
- Probe Path: `/health/ready`
- Probe Interval: 30 seconds
- Probe Timeout: 30 seconds
- Unhealthy Threshold: 3

## Creating Custom Health Checks

### Example: Database Row Count Check
```csharp
public class DatabaseRecordCountCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DatabaseRecordCountCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var productCount = await _context.Products.CountAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                { "productCount", productCount }
            };

            if (productCount == 0)
            {
                return HealthCheckResult.Degraded(
                    "No products in database", null, data);
            }

            return HealthCheckResult.Healthy(
                $"Database has {productCount} products", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Error checking product count", ex);
        }
    }
}

// Register in Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseRecordCountCheck>("database-records");
```

## Monitoring and Alerting

### Integration with Monitoring Tools

**Prometheus**:
```csharp
// Install: dotnet add package AspNetCore.HealthChecks.Publisher.Prometheus
builder.Services.AddHealthChecks()
    .AddCheck<HangfireHealthCheck>("hangfire")
    .ForwardToPrometheus();
```

**Application Insights**:
```csharp
builder.Services.AddHealthChecks()
    .AddApplicationInsightsPublisher();
```

### Custom Alerting
```csharp
public class EmailHealthCheckPublisher : IHealthCheckPublisher
{
    private readonly IEmailService _emailService;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        if (report.Status == HealthStatus.Unhealthy)
        {
            await _emailService.SendAlertAsync(
                "StockFlow API Unhealthy",
                $"Health check failed: {report.TotalDuration}");
        }
    }
}
```

## Best Practices

### 1. Endpoint Security
```csharp
// Restrict health check access in production
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireAuthorization("HealthCheckPolicy");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HealthCheckPolicy", policy =>
        policy.RequireRole("Admin"));
});
```

### 2. Timeout Configuration
```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: connectionString,
        timeout: TimeSpan.FromSeconds(3)); // Prevent long waits
```

### 3. Caching Health Check Results
```csharp
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(5);
    options.Period = TimeSpan.FromSeconds(30);
});
```

### 4. Logging
All health checks automatically log to Serilog:
```
[19:34:15 INF] Health check 'sqlserver' completed with status 'Healthy' in 56.78ms
[19:34:15 INF] Health check 'hangfire' completed with status 'Healthy' in 12.34ms
```

## Troubleshooting

### Health Check Always Returns Unhealthy
1. Check connection strings in appsettings.json
2. Verify database/Redis is accessible
3. Check firewall rules
4. Review logs for exceptions

### Health Check Timeout
1. Reduce timeout in health check configuration
2. Check database query performance
3. Verify network latency

### Redis Health Check Fails but App Works
- Redis health check is `Degraded` status, not `Unhealthy`
- App falls back to in-memory cache
- This is by design - cache failure shouldn't stop the app

## Testing Health Checks

### Unit Testing
```csharp
[Fact]
public async Task HangfireHealthCheck_ShouldReturnHealthy_WhenServersRunning()
{
    // Arrange
    var mockLogger = new Mock<ILogger<HangfireHealthCheck>>();
    var healthCheck = new HangfireHealthCheck(mockLogger.Object);
    
    // Act
    var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
    
    // Assert
    Assert.Equal(HealthStatus.Healthy, result.Status);
    Assert.Contains("servers", result.Data.Keys);
}
```

### Integration Testing
```csharp
[Fact]
public async Task HealthEndpoint_ShouldReturn200_WhenHealthy()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/health");
    
    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Assert.Contains("Healthy", content);
}
```

## Summary

✅ **Health Check Endpoints**:
- `/health` - Overall health (all checks)
- `/health/ready` - Readiness probe (database only)
- `/health/live` - Liveness probe (always healthy)

✅ **Configured Checks**:
- SQL Server (Unhealthy on failure)
- Hangfire (Degraded on failure)
- Redis (Degraded on failure, optional)

✅ **Response Format**: JSON with detailed status, duration, and data

✅ **Integration Ready**:
- Kubernetes probes
- Load balancers
- Monitoring tools
- Docker health checks

✅ **Production Ready**:
- Serilog integration
- Proper error handling
- Configurable timeouts
- Tag-based filtering

The health check system is fully operational and ready for production deployment!

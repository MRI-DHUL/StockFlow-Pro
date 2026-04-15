using Microsoft.Extensions.Diagnostics.HealthChecks;
using Hangfire.Storage;
using Hangfire;

namespace StockFlow.API.HealthChecks;

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
            
            // Try to get statistics to verify Hangfire is working
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
                    HealthCheckResult.Degraded("No Hangfire servers are running", null, data));
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

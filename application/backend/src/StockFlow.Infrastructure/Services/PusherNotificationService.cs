using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PusherServer;
using StockFlow.Application.Interfaces;

namespace StockFlow.Infrastructure.Services;

public class PusherNotificationService : INotificationService
{
    private readonly Pusher _pusher;
    private readonly ILogger<PusherNotificationService> _logger;
    private readonly string _defaultChannel;

    public PusherNotificationService(IConfiguration configuration, ILogger<PusherNotificationService> logger)
    {
        _logger = logger;
        
        var pusherSettings = configuration.GetSection("PusherSettings");
        var appId = pusherSettings["AppId"] ?? throw new InvalidOperationException("Pusher AppId not configured");
        var key = pusherSettings["Key"] ?? throw new InvalidOperationException("Pusher Key not configured");
        var secret = pusherSettings["Secret"] ?? throw new InvalidOperationException("Pusher Secret not configured");
        var cluster = pusherSettings["Cluster"] ?? "us2";
        _defaultChannel = pusherSettings["DefaultChannel"] ?? "stockflow-notifications";

        var options = new PusherOptions
        {
            Cluster = cluster,
            Encrypted = true
        };

        _pusher = new Pusher(appId, key, secret, options);
        _logger.LogInformation("Pusher service initialized with cluster: {Cluster}, default channel: {Channel}", 
            cluster, _defaultChannel);
    }

    public async Task SendNotificationAsync(string channel, string eventName, object data, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _pusher.TriggerAsync(channel, eventName, data);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Pusher notification sent successfully. Channel: {Channel}, Event: {Event}", 
                    channel, eventName);
            }
            else
            {
                _logger.LogWarning("Pusher notification failed. Status: {StatusCode}, Channel: {Channel}, Event: {Event}", 
                    result.StatusCode, channel, eventName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Pusher notification. Channel: {Channel}, Event: {Event}", 
                channel, eventName);
            throw;
        }
    }

    public async Task SendLowStockNotificationAsync(string productName, string sku, int currentQuantity, int threshold, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            type = "low-stock-alert",
            productName,
            sku,
            currentQuantity,
            threshold,
            timestamp = DateTime.UtcNow,
            severity = "warning"
        };

        await SendNotificationAsync(_defaultChannel, "low-stock-detected", data, cancellationToken);
        _logger.LogInformation("Low stock notification sent via Pusher for product {ProductName} ({SKU})", 
            productName, sku);
    }

    public async Task SendOrderPlacedNotificationAsync(Guid orderId, string customerName, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            type = "order-placed",
            orderId = orderId.ToString(),
            customerName,
            totalAmount,
            timestamp = DateTime.UtcNow
        };

        await SendNotificationAsync(_defaultChannel, "order-placed", data, cancellationToken);
        _logger.LogInformation("Order placed notification sent via Pusher for order {OrderId}", orderId);
    }

    public async Task SendStockUpdateNotificationAsync(string productName, int oldQuantity, int newQuantity, string warehouseName, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            type = "stock-updated",
            productName,
            oldQuantity,
            newQuantity,
            quantityChange = newQuantity - oldQuantity,
            warehouseName,
            timestamp = DateTime.UtcNow
        };

        await SendNotificationAsync(_defaultChannel, "stock-updated", data, cancellationToken);
        _logger.LogInformation("Stock update notification sent via Pusher for product {ProductName} in {Warehouse}", 
            productName, warehouseName);
    }
}

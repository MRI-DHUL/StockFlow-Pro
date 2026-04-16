namespace StockFlow.Application.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Send real-time notification via Pusher
    /// </summary>
    Task SendNotificationAsync(string channel, string eventName, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send low stock alert notification
    /// </summary>
    Task SendLowStockNotificationAsync(string productName, string sku, int currentQuantity, int threshold, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send order placed notification
    /// </summary>
    Task SendOrderPlacedNotificationAsync(Guid orderId, string customerName, decimal totalAmount, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send stock update notification
    /// </summary>
    Task SendStockUpdateNotificationAsync(string productName, int oldQuantity, int newQuantity, string warehouseName, CancellationToken cancellationToken = default);
}

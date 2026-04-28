using Microsoft.Extensions.Logging;
using StockFlow.Application.Interfaces;

namespace StockFlow.Infrastructure.BackgroundJobs;

public class InventoryJobs
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<InventoryJobs> _logger;

    public InventoryJobs(
        IInventoryRepository inventoryRepository,
        IEmailService emailService,
        INotificationService notificationService,
        ILogger<InventoryJobs> logger)
    {
        _inventoryRepository = inventoryRepository;
        _emailService = emailService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CheckLowStockLevels()
    {
        _logger.LogInformation("Starting low stock level check...");

        try
        {
            var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync();
            var count = lowStockItems.Count();

            if (count > 0)
            {
                _logger.LogWarning("Found {Count} low stock items", count);
                
                foreach (var item in lowStockItems)
                {
                    _logger.LogWarning(
                        "Low stock alert: Product {ProductId} ({ProductName}) in Warehouse {WarehouseId} - Quantity: {Quantity}, Threshold: {Threshold}",
                        item.ProductId, item.Product?.Name ?? "Unknown", item.WarehouseId, item.Quantity, item.Threshold);

                    try
                    {
                        // Send email notification
                        await _emailService.SendLowStockAlertAsync(
                            item.Product?.Name ?? "Unknown Product",
                            item.Product?.SKU ?? "N/A",
                            item.Quantity,
                            item.Threshold);

                        // Send real-time notification via Pusher
                        await _notificationService.SendLowStockNotificationAsync(
                            item.Product?.Name ?? "Unknown Product",
                            item.Product?.SKU ?? "N/A",
                            item.Quantity,
                            item.Threshold);

                        _logger.LogInformation(
                            "Low stock notifications sent for Product {ProductName} ({SKU})",
                            item.Product?.Name, item.Product?.SKU);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogError(notificationEx,
                            "Failed to send low stock notification for Product {ProductId}",
                            item.ProductId);
                        // Continue processing other items even if one fails
                    }
                }
            }
            else
            {
                _logger.LogInformation("No low stock items found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking low stock levels");
            throw;
        }
    }
}

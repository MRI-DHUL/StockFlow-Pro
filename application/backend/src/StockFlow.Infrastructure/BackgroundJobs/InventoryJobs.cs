using Microsoft.Extensions.Logging;
using StockFlow.Application.Interfaces;

namespace StockFlow.Infrastructure.BackgroundJobs;

public class InventoryJobs
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<InventoryJobs> _logger;

    public InventoryJobs(
        IInventoryRepository inventoryRepository,
        ILogger<InventoryJobs> logger)
    {
        _inventoryRepository = inventoryRepository;
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
                // In production, send email notifications or alerts here
                foreach (var item in lowStockItems)
                {
                    _logger.LogWarning(
                        "Low stock alert: Product {ProductId} in Warehouse {WarehouseId} - Quantity: {Quantity}, Threshold: {Threshold}",
                        item.ProductId, item.WarehouseId, item.Quantity, item.Threshold);
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

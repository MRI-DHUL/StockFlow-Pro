namespace StockFlow.Application.DTOs;

public class InventoryFilterParams : PaginationParams
{
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public bool? LowStock { get; set; } // If true, filter where quantity <= reorderLevel
}

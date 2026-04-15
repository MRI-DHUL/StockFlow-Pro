using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities;

public class Inventory : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int Threshold { get; set; }
    public DateTime LastUpdated { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;

    public bool IsLowStock => Quantity <= Threshold;
}

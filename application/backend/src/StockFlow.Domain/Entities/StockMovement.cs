using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public int Quantity { get; set; }
    public MovementType Type { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse? FromWarehouse { get; set; }
    public Warehouse? ToWarehouse { get; set; }
}

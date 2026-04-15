using StockFlow.Domain.Enums;

namespace StockFlow.Application.DTOs;

public class CreateStockMovementDto
{
    public Guid ProductId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid? ToWarehouseId { get; set; }
    public int Quantity { get; set; }
    public MovementType Type { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

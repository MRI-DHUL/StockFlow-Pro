namespace StockFlow.Application.DTOs;

public class CreateInventoryDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int Threshold { get; set; }
}

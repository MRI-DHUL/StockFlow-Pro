namespace StockFlow.Application.DTOs;

public class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public List<CreatePurchaseOrderItemDto> PurchaseOrderItems { get; set; } = new();
}

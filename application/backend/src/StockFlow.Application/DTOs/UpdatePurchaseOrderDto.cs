using StockFlow.Domain.Enums;

namespace StockFlow.Application.DTOs;

public class UpdatePurchaseOrderDto
{
    public PurchaseOrderStatus Status { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
}

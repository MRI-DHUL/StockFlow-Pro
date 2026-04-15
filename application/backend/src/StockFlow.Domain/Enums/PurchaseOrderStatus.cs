namespace StockFlow.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    InTransit = 3,
    Received = 4,
    Cancelled = 5
}

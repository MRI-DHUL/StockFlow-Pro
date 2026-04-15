namespace StockFlow.Domain.Enums;

public enum MovementType
{
    In = 0,         // Stock received into warehouse
    Out = 1,        // Stock removed from warehouse (e.g., order fulfillment)
    Transfer = 2,   // Stock moved between warehouses
    Adjustment = 3  // Manual stock adjustment
}

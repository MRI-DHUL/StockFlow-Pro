using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LeadTimeDays { get; set; }

    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

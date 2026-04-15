using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? ContactInfo { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    // Navigation properties
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<StockMovement> StockMovementsFrom { get; set; } = new List<StockMovement>();
    public ICollection<StockMovement> StockMovementsTo { get; set; } = new List<StockMovement>();
}

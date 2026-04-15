namespace StockFlow.Application.DTOs;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LeadTimeDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

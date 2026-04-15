namespace StockFlow.Application.DTOs;

public class UpdateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int LeadTimeDays { get; set; }
}

namespace StockFlow.Application.DTOs;

public class AuditLogFilterParams : PaginationParams
{
    public string? UserId { get; set; }
    public string? EntityName { get; set; }
    public string? Action { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

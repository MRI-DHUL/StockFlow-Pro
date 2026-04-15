namespace StockFlow.Application.DTOs;

public class OrderFilterParams : PaginationParams
{
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? Status { get; set; } // Pending, Processing, Shipped, Delivered, Cancelled
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}

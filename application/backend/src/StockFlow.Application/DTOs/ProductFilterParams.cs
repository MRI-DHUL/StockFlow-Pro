namespace StockFlow.Application.DTOs;

public class ProductFilterParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

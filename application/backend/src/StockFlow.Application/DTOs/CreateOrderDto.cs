namespace StockFlow.Application.DTOs;

public class CreateOrderDto
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

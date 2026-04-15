using StockFlow.Domain.Enums;

namespace StockFlow.Application.DTOs;

public class UpdateOrderDto
{
    public OrderStatus Status { get; set; }
}

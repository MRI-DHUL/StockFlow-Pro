using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Orders.Commands;

public record UpdateOrderCommand(Guid Id, UpdateOrderDto OrderDto) : IRequest<OrderDto?>;

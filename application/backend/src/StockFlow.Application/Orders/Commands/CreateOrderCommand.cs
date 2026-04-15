using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Orders.Commands;

public record CreateOrderCommand(CreateOrderDto OrderDto) : IRequest<OrderDto>;

using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

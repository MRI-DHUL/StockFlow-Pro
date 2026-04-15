using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Orders.Queries;

public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>;

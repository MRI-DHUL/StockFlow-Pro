using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Orders.Queries;

public record GetOrdersPagedQuery(OrderFilterParams FilterParams) : IRequest<PagedResult<OrderDto>>;

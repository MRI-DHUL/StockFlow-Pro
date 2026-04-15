using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.PurchaseOrders.Queries;

public record GetAllPurchaseOrdersQuery : IRequest<IEnumerable<PurchaseOrderDto>>;

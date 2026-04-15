using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Inventory.Queries;

public record GetLowStockItemsQuery : IRequest<IEnumerable<InventoryDto>>;

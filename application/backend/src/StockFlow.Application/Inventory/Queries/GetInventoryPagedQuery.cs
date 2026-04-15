using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Inventory.Queries;

public record GetInventoryPagedQuery(InventoryFilterParams FilterParams) : IRequest<PagedResult<InventoryDto>>;

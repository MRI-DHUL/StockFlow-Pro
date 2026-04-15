using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Inventory.Queries;

public record GetAllInventoryQuery : IRequest<IEnumerable<InventoryDto>>;

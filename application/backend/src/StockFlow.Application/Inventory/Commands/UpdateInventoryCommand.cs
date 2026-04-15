using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Inventory.Commands;

public record UpdateInventoryCommand(Guid Id, UpdateInventoryDto InventoryDto) : IRequest<InventoryDto?>;

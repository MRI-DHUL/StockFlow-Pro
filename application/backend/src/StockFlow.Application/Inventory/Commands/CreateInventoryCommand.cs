using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Inventory.Commands;

public record CreateInventoryCommand(CreateInventoryDto InventoryDto) : IRequest<InventoryDto>;

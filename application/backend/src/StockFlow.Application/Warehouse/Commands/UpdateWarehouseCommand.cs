using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Warehouse.Commands;

public record UpdateWarehouseCommand(Guid Id, UpdateWarehouseDto WarehouseDto) : IRequest<WarehouseDto?>;

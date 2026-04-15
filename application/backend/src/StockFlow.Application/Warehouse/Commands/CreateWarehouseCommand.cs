using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Warehouse.Commands;

public record CreateWarehouseCommand(CreateWarehouseDto WarehouseDto) : IRequest<WarehouseDto>;

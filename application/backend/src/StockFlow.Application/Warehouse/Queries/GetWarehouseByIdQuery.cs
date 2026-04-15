using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Warehouse.Queries;

public record GetWarehouseByIdQuery(Guid Id) : IRequest<WarehouseDto?>;

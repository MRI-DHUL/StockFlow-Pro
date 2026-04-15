using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Warehouse.Queries;

public record GetAllWarehousesQuery : IRequest<IEnumerable<WarehouseDto>>;

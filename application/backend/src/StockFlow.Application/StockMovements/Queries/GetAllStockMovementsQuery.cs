using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.StockMovements.Queries;

public record GetAllStockMovementsQuery : IRequest<IEnumerable<StockMovementDto>>;

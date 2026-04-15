using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.StockMovements.Queries;

public record GetStockMovementByIdQuery(Guid Id) : IRequest<StockMovementDto?>;

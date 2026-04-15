using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.StockMovements.Commands;

public record CreateStockMovementCommand(CreateStockMovementDto StockMovementDto) : IRequest<StockMovementDto>;

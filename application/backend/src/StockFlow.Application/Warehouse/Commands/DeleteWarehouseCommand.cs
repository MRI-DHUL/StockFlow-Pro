using MediatR;

namespace StockFlow.Application.Warehouse.Commands;

public record DeleteWarehouseCommand(Guid Id) : IRequest<bool>;

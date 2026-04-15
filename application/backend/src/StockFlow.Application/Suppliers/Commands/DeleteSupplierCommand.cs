using MediatR;

namespace StockFlow.Application.Suppliers.Commands;

public record DeleteSupplierCommand(Guid Id) : IRequest<bool>;

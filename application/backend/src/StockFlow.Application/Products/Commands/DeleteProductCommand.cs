using MediatR;

namespace StockFlow.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

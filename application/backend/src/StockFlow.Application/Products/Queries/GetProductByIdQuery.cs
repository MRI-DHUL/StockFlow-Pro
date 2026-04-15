using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

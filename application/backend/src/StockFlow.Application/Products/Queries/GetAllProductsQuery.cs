using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

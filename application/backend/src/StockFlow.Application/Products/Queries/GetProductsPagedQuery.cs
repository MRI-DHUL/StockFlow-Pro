using MediatR;
using StockFlow.Application.DTOs;

namespace StockFlow.Application.Products.Queries;

public record GetProductsPagedQuery(ProductFilterParams FilterParams) : IRequest<PagedResult<ProductDto>>;

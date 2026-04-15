using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Products.Queries;

public class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsPagedQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        var query = _productRepository.Query();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.FilterParams.SearchTerm))
        {
            var searchTerm = request.FilterParams.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchTerm) || 
                p.SKU.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterParams.Category))
        {
            query = query.Where(p => p.Category == request.FilterParams.Category);
        }

        if (request.FilterParams.MinPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= request.FilterParams.MinPrice.Value);
        }

        if (request.FilterParams.MaxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= request.FilterParams.MaxPrice.Value);
        }

        // Apply sorting
        query = query.ApplySorting(
            request.FilterParams.SortBy ?? "Name",
            request.FilterParams.SortDescending);

        // Get paged result
        var pagedProducts = await query.ToPagedResultAsync(
            request.FilterParams.PageNumber,
            request.FilterParams.PageSize,
            cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(pagedProducts.Items),
            PageNumber = pagedProducts.PageNumber,
            PageSize = pagedProducts.PageSize,
            TotalCount = pagedProducts.TotalCount
        };
    }
}

using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "products_all";

    public GetAllProductsQueryHandler(
        IProductRepository productRepository, 
        IMapper mapper,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Try to get from cache
        var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(CacheKey, cancellationToken);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        // If not in cache, get from database
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        // Store in cache for 15 minutes
        await _cacheService.SetAsync(CacheKey, productDtos, TimeSpan.FromMinutes(15), cancellationToken);

        return productDtos;
    }
}

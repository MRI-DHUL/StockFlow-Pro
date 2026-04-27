using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private const string CacheKeyAll = "products_all";

    public ProductService(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IMapper mapper,
        ICacheService cacheService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductFilterParams filterParams, CancellationToken cancellationToken = default)
    {
        var query = _productRepository.Query();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
        {
            var searchTerm = filterParams.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.SKU.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.Category))
        {
            query = query.Where(p => p.Category == filterParams.Category);
        }

        if (filterParams.MinPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= filterParams.MinPrice.Value);
        }

        if (filterParams.MaxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= filterParams.MaxPrice.Value);
        }

        // Apply sorting
        query = query.ApplySorting(
            filterParams.SortBy ?? "Name",
            filterParams.SortDescending);

        // Get paged result
        var pagedProducts = await query.ToPagedResultAsync(
            filterParams.PageNumber,
            filterParams.PageSize,
            cancellationToken);

        return _mapper.Map<PagedResult<ProductDto>>(pagedProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Try to get from cache
        var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(CacheKeyAll, cancellationToken);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        // If not in cache, get from database
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        // Store in cache for 15 minutes
        await _cacheService.SetAsync(CacheKeyAll, productDtos, TimeSpan.FromMinutes(15), cancellationToken);

        return productDtos;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createProductDto, cancellationToken);

        // Check for duplicate by Name or SKU
        var existingProduct = await _unitOfWork.Products.Query()
            .FirstOrDefaultAsync(p => p.Name.ToLower() == createProductDto.Name.ToLower() || p.SKU.ToLower() == createProductDto.SKU.ToLower(), cancellationToken);
        if (existingProduct != null)
        {
            // Return existing product, do not add duplicate
            return _mapper.Map<ProductDto>(existingProduct);
        }

        var product = _mapper.Map<Product>(createProductDto);
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate products cache
        await _cacheService.RemoveAsync(CacheKeyAll, cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateProductDto, cancellationToken);

        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

        if (product == null)
            return null;

        _mapper.Map(updateProductDto, product);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate caches
        await _cacheService.RemoveAsync(CacheKeyAll, cancellationToken);
        await _cacheService.RemoveAsync($"product_{id}", cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);

        if (product == null)
            return false;

        await _unitOfWork.Products.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate caches
        await _cacheService.RemoveAsync(CacheKeyAll, cancellationToken);
        await _cacheService.RemoveAsync($"product_{id}", cancellationToken);

        return true;
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _productRepository.Query()
            .Where(p => p.Category != null)
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        return categories;
    }
}

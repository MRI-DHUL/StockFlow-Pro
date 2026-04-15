using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Inventory.Queries;

public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, IEnumerable<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "inventory_lowstock";

    public GetLowStockItemsQueryHandler(
        IInventoryRepository inventoryRepository, 
        IMapper mapper,
        ICacheService cacheService)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<InventoryDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        // Try to get from cache
        var cachedLowStock = await _cacheService.GetAsync<List<InventoryDto>>(CacheKey, cancellationToken);
        if (cachedLowStock != null)
        {
            return cachedLowStock;
        }

        // If not in cache, get from database
        var lowStockItems = await _inventoryRepository.GetLowStockItemsAsync(cancellationToken);
        var lowStockDtos = _mapper.Map<List<InventoryDto>>(lowStockItems);

        // Store in cache for 3 minutes (critical data, shorter cache)
        await _cacheService.SetAsync(CacheKey, lowStockDtos, TimeSpan.FromMinutes(3), cancellationToken);

        return lowStockDtos;
    }
}

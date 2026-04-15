using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Inventory.Queries;

public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, IEnumerable<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "inventory_all";

    public GetAllInventoryQueryHandler(
        IInventoryRepository inventoryRepository, 
        IMapper mapper,
        ICacheService cacheService)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<InventoryDto>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
    {
        // Try to get from cache
        var cachedInventory = await _cacheService.GetAsync<List<InventoryDto>>(CacheKey, cancellationToken);
        if (cachedInventory != null)
        {
            return cachedInventory;
        }

        // If not in cache, get from database
        var inventories = await _inventoryRepository.GetAllAsync(cancellationToken);
        var inventoryDtos = _mapper.Map<List<InventoryDto>>(inventories);

        // Store in cache for 5 minutes (inventory changes frequently)
        await _cacheService.SetAsync(CacheKey, inventoryDtos, TimeSpan.FromMinutes(5), cancellationToken);

        return inventoryDtos;
    }
}

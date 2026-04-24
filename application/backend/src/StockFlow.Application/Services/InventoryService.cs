using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInventoryDto> _createValidator;
    private readonly IValidator<UpdateInventoryDto> _updateValidator;

    public InventoryService(
        IUnitOfWork unitOfWork,
        IInventoryRepository inventoryRepository,
        IMapper mapper,
        IValidator<CreateInventoryDto> createValidator,
        IValidator<UpdateInventoryDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<InventoryDto>> GetPagedAsync(InventoryFilterParams filterParams, CancellationToken cancellationToken = default)
    {
        var query = _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsQueryable();

        // Apply filters
        if (filterParams.ProductId.HasValue)
        {
            query = query.Where(i => i.ProductId == filterParams.ProductId.Value);
        }

        if (filterParams.WarehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == filterParams.WarehouseId.Value);
        }

        if (filterParams.LowStock.HasValue && filterParams.LowStock.Value)
        {
            query = query.Where(i => i.Quantity <= i.Threshold);
        }

        // Apply sorting
        query = query.ApplySorting(
            filterParams.SortBy ?? "LastUpdated",
            filterParams.SortDescending);

        var pagedInventory = await query.ToPagedResultAsync(
            filterParams.PageNumber,
            filterParams.PageSize,
            cancellationToken);

        return _mapper.Map<PagedResult<InventoryDto>>(pagedInventory);
    }

    public async Task<IEnumerable<InventoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<InventoryDto>>(inventory);
    }

    public async Task<IEnumerable<InventoryDto>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        var lowStockItems = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Quantity <= i.Threshold)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<InventoryDto>>(lowStockItems);
    }

    public async Task<InventoryDto> CreateAsync(CreateInventoryDto createInventoryDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createInventoryDto, cancellationToken);

        var inventory = _mapper.Map<Inventory>(createInventoryDto);
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.AddAsync(inventory, cancellationToken);

        var result = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == inventory.Id, cancellationToken);

        return result != null ? _mapper.Map<InventoryDto>(result) : throw new InvalidOperationException("Failed to create inventory");
    }

    public async Task<InventoryDto?> UpdateAsync(Guid id, UpdateInventoryDto updateInventoryDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateInventoryDto, cancellationToken);

        var inventory = await _unitOfWork.Inventories.GetByIdAsync(id, cancellationToken);

        if (inventory == null)
            return null;

        inventory.Quantity = updateInventoryDto.Quantity;
        inventory.Threshold = updateInventoryDto.Threshold;
        inventory.LastUpdated = DateTime.UtcNow;

        await _unitOfWork.Inventories.UpdateAsync(inventory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        return result != null ? _mapper.Map<InventoryDto>(result) : null;
    }

    public async Task<InventoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.Query()
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        return inventory != null ? _mapper.Map<InventoryDto>(inventory) : null;
    }
}

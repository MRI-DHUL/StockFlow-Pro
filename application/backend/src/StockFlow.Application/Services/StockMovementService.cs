using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services;

public class StockMovementService : IStockMovementService
{
    private readonly IRepository<StockMovement> _stockMovementRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateStockMovementDto> _createValidator;

    public StockMovementService(
        IRepository<StockMovement> stockMovementRepository,
        IInventoryRepository inventoryRepository,
        IMapper mapper,
        IValidator<CreateStockMovementDto> createValidator)
    {
        _stockMovementRepository = stockMovementRepository;
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<StockMovementDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var movements = await _stockMovementRepository.Query()
            .Include(sm => sm.Product)
            .Include(sm => sm.FromWarehouse)
            .Include(sm => sm.ToWarehouse)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<StockMovementDto>>(movements);
    }

    public async Task<StockMovementDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movement = await _stockMovementRepository.Query()
            .Include(sm => sm.Product)
            .Include(sm => sm.FromWarehouse)
            .Include(sm => sm.ToWarehouse)
            .FirstOrDefaultAsync(sm => sm.Id == id, cancellationToken);

        return movement != null ? _mapper.Map<StockMovementDto>(movement) : null;
    }

    public async Task<StockMovementDto> CreateAsync(CreateStockMovementDto createStockMovementDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createStockMovementDto, cancellationToken);

        // Create stock movement record
        var movement = _mapper.Map<StockMovement>(createStockMovementDto);
        await _stockMovementRepository.AddAsync(movement, cancellationToken);

        // Update inventory based on movement type
        await UpdateInventoryAsync(createStockMovementDto, cancellationToken);

        var result = await _stockMovementRepository.Query()
            .Include(sm => sm.Product)
            .Include(sm => sm.FromWarehouse)
            .Include(sm => sm.ToWarehouse)
            .FirstOrDefaultAsync(sm => sm.Id == movement.Id, cancellationToken);

        return result != null ? _mapper.Map<StockMovementDto>(result) : throw new InvalidOperationException("Failed to create stock movement");
    }

    private async Task UpdateInventoryAsync(CreateStockMovementDto dto, CancellationToken cancellationToken)
    {
        switch (dto.Type)
        {
            case MovementType.In:
                // Increase inventory in destination warehouse
                if (dto.ToWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.ToWarehouseId.Value, dto.Quantity, cancellationToken);
                }
                break;

            case MovementType.Out:
                // Decrease inventory in source warehouse
                if (dto.FromWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.FromWarehouseId.Value, -dto.Quantity, cancellationToken);
                }
                break;

            case MovementType.Transfer:
                // Decrease from source, increase in destination
                if (dto.FromWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.FromWarehouseId.Value, -dto.Quantity, cancellationToken);
                }
                if (dto.ToWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.ToWarehouseId.Value, dto.Quantity, cancellationToken);
                }
                break;

            case MovementType.Adjustment:
                // Manual adjustment - can be positive or negative
                if (dto.ToWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.ToWarehouseId.Value, dto.Quantity, cancellationToken);
                }
                else if (dto.FromWarehouseId.HasValue)
                {
                    await AdjustInventoryAsync(dto.ProductId, dto.FromWarehouseId.Value, dto.Quantity, cancellationToken);
                }
                break;
        }
    }

    private async Task AdjustInventoryAsync(Guid productId, Guid warehouseId, int adjustment, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);

        if (inventory == null)
        {
            // Create new inventory record if it doesn't exist
            inventory = new Inventory
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = Math.Max(0, adjustment),
                Threshold = 10,
                LastUpdated = DateTime.UtcNow
            };

            await _inventoryRepository.AddAsync(inventory, cancellationToken);
        }
        else
        {
            // Update existing inventory
            inventory.Quantity += adjustment;
            inventory.Quantity = Math.Max(0, inventory.Quantity); // Prevent negative quantities
            inventory.LastUpdated = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
        }
    }
}

using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.StockMovements.Commands;

public class CreateStockMovementCommandHandler : IRequestHandler<CreateStockMovementCommand, StockMovementDto>
{
    private readonly IRepository<Domain.Entities.StockMovement> _stockMovementRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public CreateStockMovementCommandHandler(
        IRepository<Domain.Entities.StockMovement> stockMovementRepository,
        IInventoryRepository inventoryRepository,
        IMapper mapper)
    {
        _stockMovementRepository = stockMovementRepository;
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<StockMovementDto> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {
        var dto = request.StockMovementDto;

        // Create stock movement record
        var movement = _mapper.Map<Domain.Entities.StockMovement>(dto);
        await _stockMovementRepository.AddAsync(movement, cancellationToken);

        // Update inventory based on movement type
        await UpdateInventoryAsync(dto, cancellationToken);

        var result = await _stockMovementRepository.GetByIdAsync(movement.Id, cancellationToken);
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
            inventory = new Domain.Entities.Inventory
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
            inventory.Quantity = Math.Max(0, inventory.Quantity + adjustment);
            inventory.LastUpdated = DateTime.UtcNow;
            await _inventoryRepository.UpdateAsync(inventory, cancellationToken);
        }
    }
}

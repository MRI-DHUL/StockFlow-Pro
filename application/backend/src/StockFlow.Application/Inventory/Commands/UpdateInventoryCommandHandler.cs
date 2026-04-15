using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Inventory.Commands;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, InventoryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateInventoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryDto?> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _unitOfWork.Inventories.GetByIdAsync(request.Id, cancellationToken);
        
        if (inventory == null)
            return null;

        inventory.Quantity = request.InventoryDto.Quantity;
        inventory.Threshold = request.InventoryDto.Threshold;
        inventory.LastUpdated = DateTime.UtcNow;

        await _unitOfWork.Inventories.UpdateAsync(inventory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Inventories.GetByIdAsync(inventory.Id, cancellationToken);
        return result != null ? _mapper.Map<InventoryDto>(result) : null;
    }
}

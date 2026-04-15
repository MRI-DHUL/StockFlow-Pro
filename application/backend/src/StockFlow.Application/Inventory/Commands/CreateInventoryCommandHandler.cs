using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Inventory.Commands;

public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, InventoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInventoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryDto> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = _mapper.Map<Domain.Entities.Inventory>(request.InventoryDto);
        inventory.LastUpdated = DateTime.UtcNow;

        await _unitOfWork.Inventories.AddAsync(inventory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Inventories.GetByIdAsync(inventory.Id, cancellationToken);
        return result != null ? _mapper.Map<InventoryDto>(result) : throw new InvalidOperationException("Failed to create inventory");
    }
}

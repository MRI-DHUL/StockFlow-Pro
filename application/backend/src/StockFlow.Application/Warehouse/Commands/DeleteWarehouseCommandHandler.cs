using MediatR;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Warehouse.Commands;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, bool>
{
    private readonly IRepository<Domain.Entities.Warehouse> _warehouseRepository;

    public DeleteWarehouseCommandHandler(IRepository<Domain.Entities.Warehouse> warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<bool> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (warehouse == null)
            return false;

        await _warehouseRepository.DeleteAsync(warehouse, cancellationToken);
        return true;
    }
}

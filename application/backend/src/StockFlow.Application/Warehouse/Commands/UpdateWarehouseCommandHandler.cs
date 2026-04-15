using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Warehouse.Commands;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, WarehouseDto?>
{
    private readonly IRepository<Domain.Entities.Warehouse> _warehouseRepository;
    private readonly IMapper _mapper;

    public UpdateWarehouseCommandHandler(IRepository<Domain.Entities.Warehouse> warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<WarehouseDto?> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (warehouse == null)
            return null;

        warehouse.Name = request.WarehouseDto.Name;
        warehouse.Location = request.WarehouseDto.Location;
        warehouse.Capacity = request.WarehouseDto.Capacity;

        await _warehouseRepository.UpdateAsync(warehouse, cancellationToken);

        return _mapper.Map<WarehouseDto>(warehouse);
    }
}

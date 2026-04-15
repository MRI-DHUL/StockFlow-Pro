using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Warehouse.Commands;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IRepository<Domain.Entities.Warehouse> _warehouseRepository;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(IRepository<Domain.Entities.Warehouse> warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = _mapper.Map<Domain.Entities.Warehouse>(request.WarehouseDto);

        await _warehouseRepository.AddAsync(warehouse, cancellationToken);

        return _mapper.Map<WarehouseDto>(warehouse);
    }
}

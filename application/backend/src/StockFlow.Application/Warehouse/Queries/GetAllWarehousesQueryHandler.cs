using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Warehouse.Queries;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, IEnumerable<WarehouseDto>>
{
    private readonly IRepository<Domain.Entities.Warehouse> _warehouseRepository;
    private readonly IMapper _mapper;

    public GetAllWarehousesQueryHandler(IRepository<Domain.Entities.Warehouse> warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
    }
}

using FluentValidation;
using MapsterMapper;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateWarehouseDto> _createValidator;
    private readonly IValidator<UpdateWarehouseDto> _updateValidator;

    public WarehouseService(
        IRepository<Warehouse> warehouseRepository,
        IMapper mapper,
        IValidator<CreateWarehouseDto> createValidator,
        IValidator<UpdateWarehouseDto> updateValidator)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<WarehouseDto>>(warehouses);
    }

    public async Task<WarehouseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id, cancellationToken);
        return warehouse != null ? _mapper.Map<WarehouseDto>(warehouse) : null;
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto createWarehouseDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createWarehouseDto, cancellationToken);

        var warehouse = _mapper.Map<Warehouse>(createWarehouseDto);

        await _warehouseRepository.AddAsync(warehouse, cancellationToken);

        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<WarehouseDto?> UpdateAsync(Guid id, UpdateWarehouseDto updateWarehouseDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateWarehouseDto, cancellationToken);

        var warehouse = await _warehouseRepository.GetByIdAsync(id, cancellationToken);

        if (warehouse == null)
            return null;

        _mapper.Map(updateWarehouseDto, warehouse);

        await _warehouseRepository.UpdateAsync(warehouse, cancellationToken);

        return _mapper.Map<WarehouseDto>(warehouse);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id, cancellationToken);

        if (warehouse == null)
            return false;

        await _warehouseRepository.DeleteAsync(warehouse, cancellationToken);

        return true;
    }
}

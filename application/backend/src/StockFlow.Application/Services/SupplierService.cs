using FluentValidation;
using MapsterMapper;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSupplierDto> _createValidator;
    private readonly IValidator<UpdateSupplierDto> _updateValidator;

    public SupplierService(
        IUnitOfWork unitOfWork,
        IRepository<Supplier> supplierRepository,
        IMapper mapper,
        IValidator<CreateSupplierDto> createValidator,
        IValidator<UpdateSupplierDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<SupplierDto>>(suppliers);
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        return supplier != null ? _mapper.Map<SupplierDto>(supplier) : null;
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto createSupplierDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createSupplierDto, cancellationToken);

        var supplier = _mapper.Map<Supplier>(createSupplierDto);

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierDto?> UpdateAsync(Guid id, UpdateSupplierDto updateSupplierDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateSupplierDto, cancellationToken);

        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);

        if (supplier == null)
            return null;

        _mapper.Map(updateSupplierDto, supplier);

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);

        if (supplier == null)
            return false;

        await _supplierRepository.DeleteAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

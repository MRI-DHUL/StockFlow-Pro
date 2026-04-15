using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Suppliers.Commands;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierDto?>
{
    private readonly IRepository<Domain.Entities.Supplier> _supplierRepository;
    private readonly IMapper _mapper;

    public UpdateSupplierCommandHandler(IRepository<Domain.Entities.Supplier> supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<SupplierDto?> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (supplier == null)
            return null;

        supplier.Name = request.SupplierDto.Name;
        supplier.ContactInfo = request.SupplierDto.ContactInfo;
        supplier.Email = request.SupplierDto.Email;
        supplier.Phone = request.SupplierDto.Phone;
        supplier.LeadTimeDays = request.SupplierDto.LeadTimeDays;

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);

        return _mapper.Map<SupplierDto>(supplier);
    }
}

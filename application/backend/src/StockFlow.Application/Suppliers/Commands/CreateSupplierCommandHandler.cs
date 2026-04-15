using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Suppliers.Commands;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly IRepository<Domain.Entities.Supplier> _supplierRepository;
    private readonly IMapper _mapper;

    public CreateSupplierCommandHandler(IRepository<Domain.Entities.Supplier> supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = _mapper.Map<Domain.Entities.Supplier>(request.SupplierDto);

        await _supplierRepository.AddAsync(supplier, cancellationToken);

        return _mapper.Map<SupplierDto>(supplier);
    }
}

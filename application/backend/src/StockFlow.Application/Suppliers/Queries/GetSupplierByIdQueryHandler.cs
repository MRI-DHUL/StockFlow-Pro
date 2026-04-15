using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Suppliers.Queries;

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly IRepository<Domain.Entities.Supplier> _supplierRepository;
    private readonly IMapper _mapper;

    public GetSupplierByIdQueryHandler(IRepository<Domain.Entities.Supplier> supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        return supplier != null ? _mapper.Map<SupplierDto>(supplier) : null;
    }
}

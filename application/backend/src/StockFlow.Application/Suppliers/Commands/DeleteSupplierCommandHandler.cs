using MediatR;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.Suppliers.Commands;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, bool>
{
    private readonly IRepository<Domain.Entities.Supplier> _supplierRepository;

    public DeleteSupplierCommandHandler(IRepository<Domain.Entities.Supplier> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (supplier == null)
            return false;

        await _supplierRepository.DeleteAsync(supplier, cancellationToken);
        return true;
    }
}

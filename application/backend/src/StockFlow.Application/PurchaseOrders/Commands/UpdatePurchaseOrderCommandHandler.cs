using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.PurchaseOrders.Commands;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto?>
{
    private readonly IRepository<Domain.Entities.PurchaseOrder> _purchaseOrderRepository;
    private readonly IMapper _mapper;

    public UpdatePurchaseOrderCommandHandler(IRepository<Domain.Entities.PurchaseOrder> purchaseOrderRepository, IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto?> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (purchaseOrder == null)
            return null;

        purchaseOrder.Status = request.PurchaseOrderDto.Status;
        purchaseOrder.ActualDeliveryDate = request.PurchaseOrderDto.ActualDeliveryDate;

        await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);

        var result = await _purchaseOrderRepository.GetByIdAsync(purchaseOrder.Id, cancellationToken);
        return result != null ? _mapper.Map<PurchaseOrderDto>(result) : null;
    }
}

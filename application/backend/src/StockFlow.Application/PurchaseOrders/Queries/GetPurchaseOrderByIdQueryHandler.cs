using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.PurchaseOrders.Queries;

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>
{
    private readonly IRepository<Domain.Entities.PurchaseOrder> _purchaseOrderRepository;
    private readonly IMapper _mapper;

    public GetPurchaseOrderByIdQueryHandler(IRepository<Domain.Entities.PurchaseOrder> purchaseOrderRepository, IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        return purchaseOrder != null ? _mapper.Map<PurchaseOrderDto>(purchaseOrder) : null;
    }
}

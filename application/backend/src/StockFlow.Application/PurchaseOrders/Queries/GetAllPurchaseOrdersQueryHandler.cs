using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.PurchaseOrders.Queries;

public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, IEnumerable<PurchaseOrderDto>>
{
    private readonly IRepository<Domain.Entities.PurchaseOrder> _purchaseOrderRepository;
    private readonly IMapper _mapper;

    public GetAllPurchaseOrdersQueryHandler(IRepository<Domain.Entities.PurchaseOrder> purchaseOrderRepository, IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrders = await _purchaseOrderRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrders);
    }
}

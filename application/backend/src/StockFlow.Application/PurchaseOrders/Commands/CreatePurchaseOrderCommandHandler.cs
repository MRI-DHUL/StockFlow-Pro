using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IRepository<Domain.Entities.PurchaseOrder> _purchaseOrderRepository;
    private readonly IRepository<Domain.Entities.Supplier> _supplierRepository;
    private readonly IMapper _mapper;

    public CreatePurchaseOrderCommandHandler(
        IRepository<Domain.Entities.PurchaseOrder> purchaseOrderRepository,
        IRepository<Domain.Entities.Supplier> supplierRepository,
        IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.PurchaseOrderDto.SupplierId, cancellationToken);
        
        if (supplier == null)
            throw new KeyNotFoundException($"Supplier with ID {request.PurchaseOrderDto.SupplierId} not found.");

        var purchaseOrder = new Domain.Entities.PurchaseOrder
        {
            PONumber = GeneratePONumber(),
            SupplierId = request.PurchaseOrderDto.SupplierId,
            Status = PurchaseOrderStatus.Draft,
            ExpectedDeliveryDate = request.PurchaseOrderDto.ExpectedDeliveryDate,
            TotalAmount = 0
        };

        decimal totalAmount = 0;
        var poItems = new List<PurchaseOrderItem>();

        foreach (var itemDto in request.PurchaseOrderDto.PurchaseOrderItems)
        {
            var poItem = new PurchaseOrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                Subtotal = itemDto.UnitPrice * itemDto.Quantity
            };

            totalAmount += poItem.Subtotal;
            poItems.Add(poItem);
        }

        purchaseOrder.TotalAmount = totalAmount;
        purchaseOrder.PurchaseOrderItems = poItems;

        await _purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);

        var result = await _purchaseOrderRepository.GetByIdAsync(purchaseOrder.Id, cancellationToken);
        return result != null ? _mapper.Map<PurchaseOrderDto>(result) : throw new InvalidOperationException("Failed to create purchase order");
    }

    private static string GeneratePONumber()
    {
        return $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

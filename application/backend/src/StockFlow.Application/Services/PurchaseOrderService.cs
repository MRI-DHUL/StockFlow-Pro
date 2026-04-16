using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePurchaseOrderDto> _createValidator;
    private readonly IValidator<UpdatePurchaseOrderDto> _updateValidator;

    public PurchaseOrderService(
        IRepository<PurchaseOrder> purchaseOrderRepository,
        IRepository<Supplier> supplierRepository,
        IMapper mapper,
        IValidator<CreatePurchaseOrderDto> createValidator,
        IValidator<UpdatePurchaseOrderDto> updateValidator)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var purchaseOrders = await _purchaseOrderRepository.Query()
            .Include(po => po.Supplier)
            .Include(po => po.PurchaseOrderItems)
            .ThenInclude(poi => poi.Product)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<PurchaseOrderDto>>(purchaseOrders);
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchaseOrder = await _purchaseOrderRepository.Query()
            .Include(po => po.Supplier)
            .Include(po => po.PurchaseOrderItems)
            .ThenInclude(poi => poi.Product)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);

        return purchaseOrder != null ? _mapper.Map<PurchaseOrderDto>(purchaseOrder) : null;
    }

    public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto createPurchaseOrderDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createPurchaseOrderDto, cancellationToken);

        var supplier = await _supplierRepository.GetByIdAsync(createPurchaseOrderDto.SupplierId, cancellationToken);

        if (supplier == null)
            throw new KeyNotFoundException($"Supplier with ID {createPurchaseOrderDto.SupplierId} not found.");

        var purchaseOrder = new PurchaseOrder
        {
            PONumber = GeneratePONumber(),
            SupplierId = createPurchaseOrderDto.SupplierId,
            Status = PurchaseOrderStatus.Draft,
            ExpectedDeliveryDate = createPurchaseOrderDto.ExpectedDeliveryDate,
            TotalAmount = 0
        };

        decimal totalAmount = 0;
        var poItems = new List<PurchaseOrderItem>();

        foreach (var itemDto in createPurchaseOrderDto.PurchaseOrderItems)
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

        var result = await _purchaseOrderRepository.Query()
            .Include(po => po.Supplier)
            .Include(po => po.PurchaseOrderItems)
            .ThenInclude(poi => poi.Product)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrder.Id, cancellationToken);

        return result != null ? _mapper.Map<PurchaseOrderDto>(result) : throw new InvalidOperationException("Failed to create purchase order");
    }

    public async Task<PurchaseOrderDto?> UpdateAsync(Guid id, UpdatePurchaseOrderDto updatePurchaseOrderDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updatePurchaseOrderDto, cancellationToken);

        var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);

        if (purchaseOrder == null)
            return null;

        purchaseOrder.Status = updatePurchaseOrderDto.Status;
        purchaseOrder.ActualDeliveryDate = updatePurchaseOrderDto.ActualDeliveryDate;

        await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);

        var result = await _purchaseOrderRepository.Query()
            .Include(po => po.Supplier)
            .Include(po => po.PurchaseOrderItems)
            .ThenInclude(poi => poi.Product)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);

        return result != null ? _mapper.Map<PurchaseOrderDto>(result) : null;
    }

    private static string GeneratePONumber()
    {
        return $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

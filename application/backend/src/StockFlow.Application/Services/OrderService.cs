using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderDto> _createValidator;
    private readonly IValidator<UpdateOrderDto> _updateValidator;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateOrderDto> createValidator,
        IValidator<UpdateOrderDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<OrderDto>> GetPagedAsync(OrderFilterParams filterParams, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filterParams.OrderNumber))
        {
            var orderNumber = filterParams.OrderNumber.ToLower();
            query = query.Where(o => o.OrderNumber.ToLower().Contains(orderNumber));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.CustomerName))
        {
            var customerName = filterParams.CustomerName.ToLower();
            query = query.Where(o => o.CustomerName != null && o.CustomerName.ToLower().Contains(customerName));
        }

        if (!string.IsNullOrWhiteSpace(filterParams.Status))
        {
            query = query.Where(o => o.Status.ToString() == filterParams.Status);
        }

        if (filterParams.CreatedFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filterParams.CreatedFrom.Value);
        }

        if (filterParams.CreatedTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= filterParams.CreatedTo.Value);
        }

        if (filterParams.MinTotalAmount.HasValue)
        {
            query = query.Where(o => o.TotalAmount >= filterParams.MinTotalAmount.Value);
        }

        if (filterParams.MaxTotalAmount.HasValue)
        {
            query = query.Where(o => o.TotalAmount <= filterParams.MaxTotalAmount.Value);
        }

        // Apply sorting
        query = query.ApplySorting(
            filterParams.SortBy ?? "CreatedAt",
            filterParams.SortDescending);

        var pagedOrders = await query.ToPagedResultAsync(
            filterParams.PageNumber,
            filterParams.PageSize,
            cancellationToken);

        return _mapper.Map<PagedResult<OrderDto>>(pagedOrders);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createOrderDto, cancellationToken);

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = createOrderDto.CustomerId,
            CustomerName = createOrderDto.CustomerName,
            Status = OrderStatus.Pending,
            TotalAmount = 0
        };

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in createOrderDto.OrderItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.UnitPrice,
                Subtotal = product.UnitPrice * itemDto.Quantity
            };

            totalAmount += orderItem.Subtotal;
            orderItems.Add(orderItem);
        }

        order.TotalAmount = totalAmount;
        order.OrderItems = orderItems;

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        return result != null ? _mapper.Map<OrderDto>(result) : throw new InvalidOperationException("Failed to create order");
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderDto updateOrderDto, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateOrderDto, cancellationToken);

        var order = await _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order == null)
            return null;

        order.Status = updateOrderDto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Orders.Query()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return result != null ? _mapper.Map<OrderDto>(result) : null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id, cancellationToken);

        if (order == null)
            return false;

        await _unitOfWork.Orders.DeleteAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

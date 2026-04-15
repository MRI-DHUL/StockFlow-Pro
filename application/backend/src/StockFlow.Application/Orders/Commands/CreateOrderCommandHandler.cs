using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Orders.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = request.OrderDto.CustomerId,
            CustomerName = request.OrderDto.CustomerName,
            Status = OrderStatus.Pending,
            TotalAmount = 0
        };

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in request.OrderDto.OrderItems)
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

        var result = await _unitOfWork.Orders.GetByIdAsync(order.Id, cancellationToken);
        return result != null ? _mapper.Map<OrderDto>(result) : throw new InvalidOperationException("Failed to create order");
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

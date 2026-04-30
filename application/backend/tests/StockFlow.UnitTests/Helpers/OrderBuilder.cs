using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.UnitTests.Helpers;

/// <summary>
/// Test data builder for Order entities
/// </summary>
public class OrderBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _orderNumber = "ORD-001";
    private string? _customerId;
    private string? _customerName;
    private string? _customerEmail;
    private OrderStatus _status = OrderStatus.Pending;
    private decimal _totalAmount = 100.00m;
    private DateTime? _completedAt;
    private DateTime _createdAt = DateTime.UtcNow;
    private List<OrderItem> _orderItems = new();

    public OrderBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OrderBuilder WithOrderNumber(string orderNumber)
    {
        _orderNumber = orderNumber;
        return this;
    }

    public OrderBuilder WithCustomerId(string? customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder WithCustomerName(string? customerName)
    {
        _customerName = customerName;
        return this;
    }

    public OrderBuilder WithCustomerEmail(string? customerEmail)
    {
        _customerEmail = customerEmail;
        return this;
    }

    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }

    public OrderBuilder WithTotalAmount(decimal totalAmount)
    {
        _totalAmount = totalAmount;
        return this;
    }

    public OrderBuilder ThatIsCompleted()
    {
        _status = OrderStatus.Delivered;
        _completedAt = DateTime.UtcNow;
        return this;
    }

    public OrderBuilder ThatIsCancelled()
    {
        _status = OrderStatus.Cancelled;
        return this;
    }

    public Order Build()
    {
        return new Order
        {
            Id = _id,
            OrderNumber = _orderNumber,
            CustomerId = _customerId,
            CustomerName = _customerName,
            CustomerEmail = _customerEmail,
            Status = _status,
            TotalAmount = _totalAmount,
            CompletedAt = _completedAt,
            CreatedAt = _createdAt,
            OrderItems = _orderItems
        };
    }

    public static OrderBuilder Create() => new();
}

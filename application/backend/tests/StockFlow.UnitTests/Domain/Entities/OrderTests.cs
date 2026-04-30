using FluentAssertions;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Order_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            CustomerId = "CUST-123",
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Status = OrderStatus.Pending,
            TotalAmount = 1500.00m,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        order.Id.Should().NotBeEmpty();
        order.OrderNumber.Should().Be("ORD-001");
        order.CustomerId.Should().Be("CUST-123");
        order.CustomerName.Should().Be("John Doe");
        order.CustomerEmail.Should().Be("john@example.com");
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(1500.00m);
        order.CompletedAt.Should().BeNull();
        order.OrderItems.Should().BeEmpty();
        order.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void Order_Status_CanBeSetToValidValues(OrderStatus status)
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Status = status
        };

        // Assert
        order.Status.Should().Be(status);
    }

    [Fact]
    public void Order_CompletedAt_CanBeSetWhenDelivered()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending
        };

        // Act
        order.Status = OrderStatus.Delivered;
        order.CompletedAt = DateTime.UtcNow;

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
        order.CompletedAt.Should().NotBeNull();
        order.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Order_OrderItems_InitializesAsEmptyCollection()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001"
        };

        // Assert
        order.OrderItems.Should().NotBeNull();
        order.OrderItems.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(999999.99)]
    public void Order_TotalAmount_AcceptsValidValues(decimal amount)
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            TotalAmount = amount
        };

        // Assert
        order.TotalAmount.Should().Be(amount);
    }

    [Fact]
    public void Order_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        order.IsDeleted = true;
        order.DeletedAt = DateTime.UtcNow;

        // Assert
        order.IsDeleted.Should().BeTrue();
        order.DeletedAt.Should().NotBeNull();
        order.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Order_WithNullCustomerEmail_IsValid()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            CustomerName = "John Doe",
            CustomerEmail = null,
            Status = OrderStatus.Pending
        };

        // Assert
        order.CustomerEmail.Should().BeNull();
    }
}

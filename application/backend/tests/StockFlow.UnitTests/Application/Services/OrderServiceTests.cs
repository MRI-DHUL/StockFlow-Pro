using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Extensions;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.UnitTests.Application.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateOrderDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateOrderDto>> _updateValidatorMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateOrderDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateOrderDto>>();
        _emailServiceMock = new Mock<IEmailService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        var orderRepositoryMock = new Mock<IRepository<Order>>();
        var productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock.Setup(u => u.Orders).Returns(orderRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Products).Returns(productRepositoryMock.Object);

        _orderService = new OrderService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _emailServiceMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOrderDto()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            CustomerName = "John Doe",
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            OrderItems = new List<OrderItem>()
        };

        var orderDto = new OrderDto
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            CustomerName = "John Doe",
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m
        };

        var ordersQueryable = new List<Order> { order }.AsQueryable().BuildMock();
        _unitOfWorkMock.Setup(u => u.Orders.Query()).Returns(ordersQueryable);

        _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);

        // Act
        var result = await _orderService.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.OrderNumber.Should().Be("ORD-001");
        result.CustomerName.Should().Be("John Doe");
        result.TotalAmount.Should().Be(100.00m);

        _unitOfWorkMock.Verify(u => u.Orders.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var emptyList = new List<Order>().AsQueryable().BuildMock();
        _unitOfWorkMock.Setup(u => u.Orders.Query()).Returns(emptyList);

        // Act
        var result = await _orderService.GetByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.Orders.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), OrderNumber = "ORD-001", Status = OrderStatus.Pending, TotalAmount = 100m, OrderItems = new List<OrderItem>() },
            new() { Id = Guid.NewGuid(), OrderNumber = "ORD-002", Status = OrderStatus.Processing, TotalAmount = 200m, OrderItems = new List<OrderItem>() }
        };

        var orderDtos = new List<OrderDto>
        {
            new() { Id = orders[0].Id, OrderNumber = "ORD-001", Status = OrderStatus.Pending, TotalAmount = 100m },
            new() { Id = orders[1].Id, OrderNumber = "ORD-002", Status = OrderStatus.Processing, TotalAmount = 200m }
        };

        var ordersQueryable = orders.AsQueryable().BuildMock();
        _unitOfWorkMock.Setup(u => u.Orders.Query()).Returns(ordersQueryable);
        _mapperMock.Setup(m => m.Map<List<OrderDto>>(It.IsAny<List<Order>>())).Returns(orderDtos);

        // Act
        var result = await _orderService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(orderDtos);

        _unitOfWorkMock.Verify(u => u.Orders.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<List<OrderDto>>(It.IsAny<List<Order>>()), Times.Once);
    }

    

    

    

    

    [Fact]
    public async Task GetPagedAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), OrderNumber = "ORD-001", CustomerName = "John Doe", Status = OrderStatus.Pending, TotalAmount = 100m, CreatedAt = DateTime.UtcNow, OrderItems = new List<OrderItem>() },
            new() { Id = Guid.NewGuid(), OrderNumber = "ORD-002", CustomerName = "Jane Smith", Status = OrderStatus.Processing, TotalAmount = 200m, CreatedAt = DateTime.UtcNow, OrderItems = new List<OrderItem>() }
        };

        var orderDtos = new List<OrderDto>
        {
            new() { Id = orders[0].Id, OrderNumber = "ORD-001", CustomerName = "John Doe", Status = OrderStatus.Pending, TotalAmount = 100m },
            new() { Id = orders[1].Id, OrderNumber = "ORD-002", CustomerName = "Jane Smith", Status = OrderStatus.Processing, TotalAmount = 200m }
        };

        var filterParams = new OrderFilterParams
        {
            Status = OrderStatus.Pending.ToString(),
            PageNumber = 1,
            PageSize = 10
        };

        var ordersQueryable = orders.AsQueryable().BuildMock();
        _unitOfWorkMock.Setup(u => u.Orders.Query()).Returns(ordersQueryable);

        var pagedResult = new PagedResult<OrderDto>
        {
            Items = orderDtos.Where(o => o.Status == OrderStatus.Pending).ToList(),
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mapperMock.Setup(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<Order>>())).Returns(pagedResult);

        // Act
        var result = await _orderService.GetPagedAsync(filterParams);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be(OrderStatus.Pending);

        _unitOfWorkMock.Verify(u => u.Orders.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<Order>>()), Times.Once);
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public async Task UpdateAsync_WithDifferentStatuses_UpdatesCorrectly(OrderStatus status)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var updateDto = new UpdateOrderDto { Status = status };

        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            OrderItems = new List<OrderItem>()
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(updateDto, default(CancellationToken)))
            .ReturnsAsync(new ValidationResult());

        var ordersQueryable = new List<Order> { order }.AsQueryable().BuildMock();
        _unitOfWorkMock.Setup(u => u.Orders.Query()).Returns(ordersQueryable);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(default(CancellationToken)))
            .ReturnsAsync(1);

        var updatedOrderDto = new OrderDto
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Status = status,
            TotalAmount = 100.00m
        };

        _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(updatedOrderDto);

        // Act
        var result = await _orderService.UpdateAsync(orderId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(status);
        order.Status.Should().Be(status);
    }
}





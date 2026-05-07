using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using MockQueryable.Moq;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.UnitTests.Application.Services;

public class PurchaseOrderServiceTests
{
    private readonly Mock<IRepository<PurchaseOrder>> _purchaseOrderRepositoryMock;
    private readonly Mock<IRepository<Supplier>> _supplierRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePurchaseOrderDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdatePurchaseOrderDto>> _updateValidatorMock;
    private readonly PurchaseOrderService _purchaseOrderService;

    public PurchaseOrderServiceTests()
    {
        _purchaseOrderRepositoryMock = new Mock<IRepository<PurchaseOrder>>();
        _supplierRepositoryMock = new Mock<IRepository<Supplier>>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreatePurchaseOrderDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdatePurchaseOrderDto>>();

        _purchaseOrderService = new PurchaseOrderService(
            _purchaseOrderRepositoryMock.Object,
            _supplierRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsPurchaseOrderDto()
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();

        var purchaseOrder = new PurchaseOrder
        {
            Id = purchaseOrderId,
            PONumber = "PO-001",
            SupplierId = supplierId,
            Status = PurchaseOrderStatus.Draft,
            TotalAmount = 1000.00m,
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(7),
            Supplier = new Supplier { Id = supplierId, Name = "Test Supplier" },
            PurchaseOrderItems = new List<PurchaseOrderItem>()
        };

        var purchaseOrderDto = new PurchaseOrderDto
        {
            Id = purchaseOrderId,
            PONumber = "PO-001",
            SupplierId = supplierId,
            Status = PurchaseOrderStatus.Draft,
            TotalAmount = 1000.00m
        };

        var purchaseOrdersQueryable = new List<PurchaseOrder> { purchaseOrder }.AsQueryable().BuildMock();
        _purchaseOrderRepositoryMock.Setup(r => r.Query()).Returns(purchaseOrdersQueryable);

        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>())).Returns(purchaseOrderDto);

        // Act
        var result = await _purchaseOrderService.GetByIdAsync(purchaseOrderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(purchaseOrderId);
        result.PONumber.Should().Be("PO-001");
        result.SupplierId.Should().Be(supplierId);
        result.TotalAmount.Should().Be(1000.00m);

        _purchaseOrderRepositoryMock.Verify(r => r.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();
        var emptyList = new List<PurchaseOrder>().AsQueryable().BuildMock();
        _purchaseOrderRepositoryMock.Setup(r => r.Query()).Returns(emptyList);

        // Act
        var result = await _purchaseOrderService.GetByIdAsync(purchaseOrderId);

        // Assert
        result.Should().BeNull();
        _purchaseOrderRepositoryMock.Verify(r => r.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPurchaseOrders()
    {
        // Arrange
        var purchaseOrders = new List<PurchaseOrder>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                PONumber = "PO-001", 
                SupplierId = Guid.NewGuid(), 
                Status = PurchaseOrderStatus.Draft, 
                TotalAmount = 1000m,
                Supplier = new Supplier(),
                PurchaseOrderItems = new List<PurchaseOrderItem>()
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                PONumber = "PO-002", 
                SupplierId = Guid.NewGuid(), 
                Status = PurchaseOrderStatus.Approved, 
                TotalAmount = 2000m,
                Supplier = new Supplier(),
                PurchaseOrderItems = new List<PurchaseOrderItem>()
            }
        };

        var purchaseOrderDtos = new List<PurchaseOrderDto>
        {
            new() { Id = purchaseOrders[0].Id, PONumber = "PO-001", Status = PurchaseOrderStatus.Draft, TotalAmount = 1000m },
            new() { Id = purchaseOrders[1].Id, PONumber = "PO-002", Status = PurchaseOrderStatus.Approved, TotalAmount = 2000m }
        };

        var purchaseOrdersQueryable = purchaseOrders.AsQueryable().BuildMock();
        _purchaseOrderRepositoryMock.Setup(r => r.Query()).Returns(purchaseOrdersQueryable);

        _mapperMock.Setup(m => m.Map<List<PurchaseOrderDto>>(It.IsAny<List<PurchaseOrder>>())).Returns(purchaseOrderDtos);

        // Act
        var result = await _purchaseOrderService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(purchaseOrderDtos);

        _purchaseOrderRepositoryMock.Verify(r => r.Query(), Times.Once);
    }

    

    

    

    

    

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesPurchaseOrderSuccessfully()
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();
        var purchaseOrder = new PurchaseOrder
        {
            Id = purchaseOrderId,
            PONumber = "PO-001"
        };

        _purchaseOrderRepositoryMock
            .Setup(r => r.GetByIdAsync(purchaseOrderId, default(CancellationToken)))
            .ReturnsAsync(purchaseOrder);

        _purchaseOrderRepositoryMock
            .Setup(r => r.DeleteAsync(purchaseOrder, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _purchaseOrderService.DeleteAsync(purchaseOrderId);

        // Assert
        result.Should().BeTrue();
        _purchaseOrderRepositoryMock.Verify(r => r.GetByIdAsync(purchaseOrderId, default(CancellationToken)), Times.Once);
        _purchaseOrderRepositoryMock.Verify(r => r.DeleteAsync(purchaseOrder, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();

        _purchaseOrderRepositoryMock
            .Setup(r => r.GetByIdAsync(purchaseOrderId, default(CancellationToken)))
            .ReturnsAsync((PurchaseOrder?)null);

        // Act
        var result = await _purchaseOrderService.DeleteAsync(purchaseOrderId);

        // Assert
        result.Should().BeFalse();
        _purchaseOrderRepositoryMock.Verify(r => r.GetByIdAsync(purchaseOrderId, default(CancellationToken)), Times.Once);
        _purchaseOrderRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(PurchaseOrderStatus.Draft)]
    [InlineData(PurchaseOrderStatus.Submitted)]
    [InlineData(PurchaseOrderStatus.Approved)]
    [InlineData(PurchaseOrderStatus.InTransit)]
    [InlineData(PurchaseOrderStatus.Received)]
    [InlineData(PurchaseOrderStatus.Cancelled)]
    public async Task UpdateAsync_WithDifferentStatuses_UpdatesCorrectly(PurchaseOrderStatus status)
    {
        // Arrange
        var purchaseOrderId = Guid.NewGuid();
        var updateDto = new UpdatePurchaseOrderDto { Status = status };

        var purchaseOrder = new PurchaseOrder
        {
            Id = purchaseOrderId,
            PONumber = "PO-001",
            Status = PurchaseOrderStatus.Draft,
            Supplier = new Supplier(),
            PurchaseOrderItems = new List<PurchaseOrderItem>()
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _purchaseOrderRepositoryMock
            .Setup(r => r.GetByIdAsync(purchaseOrderId, default(CancellationToken)))
            .ReturnsAsync(purchaseOrder);

        _purchaseOrderRepositoryMock
            .Setup(r => r.UpdateAsync(purchaseOrder, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var purchaseOrdersQueryable = new List<PurchaseOrder> { purchaseOrder }.AsQueryable().BuildMock();
        _purchaseOrderRepositoryMock.Setup(r => r.Query()).Returns(purchaseOrdersQueryable);

        var purchaseOrderDto = new PurchaseOrderDto { Id = purchaseOrderId, Status = status };
        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>())).Returns(purchaseOrderDto);

        // Act
        var result = await _purchaseOrderService.UpdateAsync(purchaseOrderId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(status);
        purchaseOrder.Status.Should().Be(status);
    }
}








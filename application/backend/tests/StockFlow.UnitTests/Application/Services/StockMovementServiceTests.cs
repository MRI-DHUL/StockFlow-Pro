using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.UnitTests.Application.Services;

public class StockMovementServiceTests
{
    private readonly Mock<IRepository<StockMovement>> _stockMovementRepositoryMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateStockMovementDto>> _createValidatorMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<StockMovementService>> _loggerMock;
    private readonly StockMovementService _stockMovementService;

    public StockMovementServiceTests()
    {
        _stockMovementRepositoryMock = new Mock<IRepository<StockMovement>>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateStockMovementDto>>();
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<StockMovementService>>();

        _stockMovementService = new StockMovementService(
            _stockMovementRepositoryMock.Object,
            _inventoryRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsStockMovementDto()
    {
        // Arrange
        var movementId = Guid.NewGuid();
        var movement = new StockMovement
        {
            Id = movementId,
            ProductId = Guid.NewGuid(),
            Type = MovementType.In,
            Quantity = 100,
            CreatedAt = DateTime.UtcNow,
            Product = new Product { Id = Guid.NewGuid(), Name = "Test Product" }
        };

        var movementDto = new StockMovementDto
        {
            Id = movementId,
            ProductId = movement.ProductId,
            Type = MovementType.In,
            Quantity = 100
        };

        var movementsQueryable = new List<StockMovement> { movement }.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        _mapperMock.Setup(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>())).Returns(movementDto);

        // Act
        var result = await _stockMovementService.GetByIdAsync(movementId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(movementId);
        result.ProductId.Should().Be(movement.ProductId);
        result.Quantity.Should().Be(100);

        _stockMovementRepositoryMock.Verify(r => r.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var movementId = Guid.NewGuid();
        var emptyList = new List<StockMovement>().AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(emptyList);

        // Act
        var result = await _stockMovementService.GetByIdAsync(movementId);

        // Assert
        result.Should().BeNull();
        _stockMovementRepositoryMock.Verify(r => r.Query(), Times.Once);
        _mapperMock.Verify(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStockMovements()
    {
        // Arrange
        var movements = new List<StockMovement>
        {
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Type = MovementType.In, Quantity = 100, Product = new Product() },
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Type = MovementType.Out, Quantity = 50, Product = new Product() }
        };

        var movementDtos = new List<StockMovementDto>
        {
            new() { Id = movements[0].Id, ProductId = movements[0].ProductId, Type = MovementType.In, Quantity = 100 },
            new() { Id = movements[1].Id, ProductId = movements[1].ProductId, Type = MovementType.Out, Quantity = 50 }
        };

        var movementsQueryable = movements.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        _mapperMock.Setup(m => m.Map<List<StockMovementDto>>(It.IsAny<List<StockMovement>>())).Returns(movementDtos);

        // Act
        var result = await _stockMovementService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(movementDtos);

        _stockMovementRepositoryMock.Verify(r => r.Query(), Times.Once);
    }

    [Fact]
    public async Task GetByProductIdAsync_WithValidProductId_ReturnsProductMovements()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var movements = new List<StockMovement>
        {
            new() { Id = Guid.NewGuid(), ProductId = productId, Type = MovementType.In, Quantity = 100, CreatedAt = DateTime.UtcNow, Product = new Product() },
            new() { Id = Guid.NewGuid(), ProductId = productId, Type = MovementType.Out, Quantity = 50, CreatedAt = DateTime.UtcNow.AddHours(-1), Product = new Product() }
        };

        var movementDtos = new List<StockMovementDto>
        {
            new() { Id = movements[0].Id, ProductId = productId, Type = MovementType.In, Quantity = 100 },
            new() { Id = movements[1].Id, ProductId = productId, Type = MovementType.Out, Quantity = 50 }
        };

        var movementsQueryable = movements.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        _mapperMock.Setup(m => m.Map<List<StockMovementDto>>(It.IsAny<List<StockMovement>>())).Returns(movementDtos);

        // Act
        var result = await _stockMovementService.GetByProductIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.ProductId.Should().Be(productId));

        _stockMovementRepositoryMock.Verify(r => r.Query(), Times.Once);
    }

    

    [Fact]
    public async Task CreateAsync_WithOutMovement_CreatesAndDecreasesInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var createDto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = MovementType.Out,
            Quantity = 30,
            FromWarehouseId = warehouseId
        };

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Type = MovementType.Out,
            Quantity = 30,
            FromWarehouseId = warehouseId,
            Product = new Product { Id = productId, Name = "Test Product" },
            FromWarehouse = new Warehouse { Id = warehouseId, Name = "Test Warehouse" }
        };

        var movementDto = new StockMovementDto
        {
            Id = movement.Id,
            ProductId = productId,
            Type = MovementType.Out,
            Quantity = 30
        };

        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = 100,
            Threshold = 10
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<StockMovement>(createDto)).Returns(movement);

        _stockMovementRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StockMovement>(), default(CancellationToken))).Returns((StockMovement m, CancellationToken ct) => Task.FromResult(m));

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(productId, warehouseId, default(CancellationToken)))
            .ReturnsAsync(inventory);

        _inventoryRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Inventory>(), default(CancellationToken)))
            .Returns(Task.CompletedTask);

        var movementsQueryable = new List<StockMovement> { movement }.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        _mapperMock.Setup(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>())).Returns(movementDto);

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), default(CancellationToken)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _stockMovementService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(productId);
        result.Quantity.Should().Be(30);
        inventory.Quantity.Should().Be(70); // 100 - 30

        _inventoryRepositoryMock.Verify(r => r.UpdateAsync(inventory, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithTransferMovement_UpdatesBothWarehouses()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var fromWarehouseId = Guid.NewGuid();
        var toWarehouseId = Guid.NewGuid();

        var createDto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = MovementType.Transfer,
            Quantity = 50,
            FromWarehouseId = fromWarehouseId,
            ToWarehouseId = toWarehouseId
        };

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Type = MovementType.Transfer,
            Quantity = 50,
            FromWarehouseId = fromWarehouseId,
            ToWarehouseId = toWarehouseId,
            Product = new Product { Id = productId, Name = "Test Product" },
            FromWarehouse = new Warehouse { Id = fromWarehouseId, Name = "From Warehouse" },
            ToWarehouse = new Warehouse { Id = toWarehouseId, Name = "To Warehouse" }
        };

        var fromInventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = fromWarehouseId,
            Quantity = 100
        };

        var toInventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = toWarehouseId,
            Quantity = 30
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<StockMovement>(createDto)).Returns(movement);

        _stockMovementRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StockMovement>(), default(CancellationToken)))
            .Returns((StockMovement m, CancellationToken ct) => Task.FromResult(m));

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(productId, fromWarehouseId, default))
            .ReturnsAsync(fromInventory);

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(productId, toWarehouseId, default))
            .ReturnsAsync(toInventory);

        _inventoryRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Inventory>(), default))
            .Returns(Task.CompletedTask);

        var movementsQueryable = new List<StockMovement> { movement }.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        var movementDto = new StockMovementDto { Id = movement.Id, ProductId = productId, Type = MovementType.Transfer, Quantity = 50 };
        _mapperMock.Setup(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>())).Returns(movementDto);

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), default(CancellationToken)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _stockMovementService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        fromInventory.Quantity.Should().Be(50); // 100 - 50
        toInventory.Quantity.Should().Be(80); // 30 + 50

        _inventoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Inventory>(), default(CancellationToken)), Times.Exactly(2));
    }

    

    [Fact]
    public async Task CreateAsync_SendsNotificationSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();

        var createDto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = MovementType.In,
            Quantity = 100,
            ToWarehouseId = warehouseId
        };

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Type = MovementType.In,
            Quantity = 100,
            ToWarehouseId = warehouseId,
            Product = new Product { Id = productId, Name = "Test Product" },
            ToWarehouse = new Warehouse { Id = warehouseId, Name = "Test Warehouse" }
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<StockMovement>(createDto)).Returns(movement);

        _stockMovementRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<StockMovement>(), default(CancellationToken)))
            .Returns(Task.FromResult(movement));

        var inventory = new Inventory { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouseId, Quantity = 50 };

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(productId, warehouseId, default))
            .ReturnsAsync(inventory);

        var movementsQueryable = new List<StockMovement> { movement }.AsQueryable().BuildMock();
        _stockMovementRepositoryMock.Setup(r => r.Query()).Returns(movementsQueryable);

        var movementDto = new StockMovementDto { Id = movement.Id };
        _mapperMock.Setup(m => m.Map<StockMovementDto>(It.IsAny<StockMovement>())).Returns(movementDto);

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), default(CancellationToken)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _stockMovementService.CreateAsync(createDto);

        // Assert
        _notificationServiceMock.Verify(
            n => n.SendNotificationAsync(
                "stockflow-notifications",
                "stock-updated",
                It.IsAny<object>(),
                default(CancellationToken)),
            Times.Once);
    }
}




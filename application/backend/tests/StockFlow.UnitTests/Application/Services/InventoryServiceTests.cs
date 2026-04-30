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
using Xunit;

namespace StockFlow.UnitTests.Application.Services;

public class InventoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateInventoryDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateInventoryDto>> _updateValidatorMock;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateInventoryDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateInventoryDto>>();

        _unitOfWorkMock.Setup(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);

        _inventoryService = new InventoryService(
            _unitOfWorkMock.Object,
            _inventoryRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WithNewInventory_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20
        };

        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = createDto.ProductId,
            WarehouseId = createDto.WarehouseId,
            Quantity = createDto.Quantity,
            Threshold = createDto.Threshold,
            LastUpdated = DateTime.UtcNow,
            Product = new Product { Id = createDto.ProductId, Name = "Test Product", SKU = "TEST-001", UnitPrice = 10m },
            Warehouse = new Warehouse { Id = createDto.WarehouseId, Name = "Warehouse 1", Location = "Location 1", Capacity = 1000 }
        };

        var inventoryDto = new InventoryDto
        {
            Id = inventory.Id,
            ProductId = createDto.ProductId,
            WarehouseId = createDto.WarehouseId,
            Quantity = createDto.Quantity,
            Threshold = createDto.Threshold
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(createDto.ProductId, createDto.WarehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inventory?)null);

        _mapperMock
            .Setup(m => m.Map<Inventory>(createDto))
            .Returns(inventory);

        var inventoryQueryable = new List<Inventory> { inventory }.AsQueryable().BuildMock();
        _inventoryRepositoryMock
            .Setup(r => r.Query())
            .Returns(inventoryQueryable);

        _mapperMock
            .Setup(m => m.Map<InventoryDto>(It.IsAny<Inventory>()))
            .Returns(inventoryDto);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _inventoryService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(createDto.ProductId);
        result.WarehouseId.Should().Be(createDto.WarehouseId);
        result.Quantity.Should().Be(createDto.Quantity);
        result.Threshold.Should().Be(createDto.Threshold);

        _createValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateInventoryDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingInventory_UpdatesExistingRecord()
    {
        // Arrange
        var createDto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 150,
            Threshold = 30
        };

        var existingInventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = createDto.ProductId,
            WarehouseId = createDto.WarehouseId,
            Quantity = 100,
            Threshold = 20,
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _inventoryRepositoryMock
            .Setup(r => r.GetByProductAndWarehouseAsync(createDto.ProductId, createDto.WarehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingInventory);

        var existingInventoryQueryable = new List<Inventory> { existingInventory }.AsQueryable().BuildMock();
        _inventoryRepositoryMock
            .Setup(r => r.Query())
            .Returns(existingInventoryQueryable);

        _mapperMock
            .Setup(m => m.Map<InventoryDto>(It.IsAny<Inventory>()))
            .Returns(new InventoryDto
            {
                Id = existingInventory.Id,
                ProductId = createDto.ProductId,
                WarehouseId = createDto.WarehouseId,
                Quantity = 150,
                Threshold = 30
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _inventoryService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Quantity.Should().Be(150);
        result.Threshold.Should().Be(30);

        _unitOfWorkMock.Verify(u => u.Inventories.UpdateAsync(existingInventory, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesInventory()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();
        var updateDto = new UpdateInventoryDto
        {
            Quantity = 200,
            Threshold = 40
        };

        var existingInventory = new Inventory
        {
            Id = inventoryId,
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20,
            Product = new Product { Id = Guid.NewGuid(), Name = "Test Product", SKU = "TEST-001", UnitPrice = 10m },
            Warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 1", Location = "Location 1", Capacity = 1000 }
        };

        var updatedInventoryDto = new InventoryDto
        {
            Id = inventoryId,
            ProductId = existingInventory.ProductId,
            WarehouseId = existingInventory.WarehouseId,
            Quantity = 200,
            Threshold = 40
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateInventoryDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _unitOfWorkMock
            .Setup(u => u.Inventories.GetByIdAsync(inventoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingInventory);

        var existingInventoryQueryable2 = new List<Inventory> { existingInventory }.AsQueryable().BuildMock();
        _inventoryRepositoryMock
            .Setup(r => r.Query())
            .Returns(existingInventoryQueryable2);

        _mapperMock
            .Setup(m => m.Map<InventoryDto>(It.IsAny<Inventory>()))
            .Returns(updatedInventoryDto);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _inventoryService.UpdateAsync(inventoryId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Quantity.Should().Be(200);
        result.Threshold.Should().Be(40);

        existingInventory.Quantity.Should().Be(200);
        existingInventory.Threshold.Should().Be(40);

        _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateInventoryDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Inventories.UpdateAsync(existingInventory, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();
        var updateDto = new UpdateInventoryDto
        {
            Quantity = 200,
            Threshold = 40
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _unitOfWorkMock
            .Setup(u => u.Inventories.GetByIdAsync(inventoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inventory?)null);

        // Act
        var result = await _inventoryService.UpdateAsync(inventoryId, updateDto);

        // Assert
        result.Should().BeNull();

        _unitOfWorkMock.Verify(u => u.Inventories.UpdateAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesLastUpdatedTimestamp()
    {
        // Arrange
        var inventoryId = Guid.NewGuid();
        var updateDto = new UpdateInventoryDto
        {
            Quantity = 150,
            Threshold = 25
        };

        var oldTimestamp = DateTime.UtcNow.AddDays(-5);
        var existingInventory = new Inventory
        {
            Id = inventoryId,
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20,
            LastUpdated = oldTimestamp
        };

        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateInventoryDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _unitOfWorkMock
            .Setup(u => u.Inventories.GetByIdAsync(inventoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingInventory);

        var existingInventoryQueryable3 = new List<Inventory> { existingInventory }.AsQueryable().BuildMock();
        _inventoryRepositoryMock
            .Setup(r => r.Query())
            .Returns(existingInventoryQueryable3);

        _mapperMock
            .Setup(m => m.Map<InventoryDto>(It.IsAny<Inventory>()))
            .Returns(new InventoryDto { Id = inventoryId });

        // Act
        await _inventoryService.UpdateAsync(inventoryId, updateDto);

        // Assert
        existingInventory.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        existingInventory.LastUpdated.Should().BeAfter(oldTimestamp);
    }
}

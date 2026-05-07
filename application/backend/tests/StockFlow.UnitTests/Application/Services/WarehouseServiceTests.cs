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

public class WarehouseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Warehouse>> _warehouseRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateWarehouseDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateWarehouseDto>> _updateValidatorMock;
    private readonly WarehouseService _warehouseService;

    public WarehouseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseRepositoryMock = new Mock<IRepository<Warehouse>>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateWarehouseDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateWarehouseDto>>();

        _warehouseService = new WarehouseService(
            _unitOfWorkMock.Object,
            _warehouseRepositoryMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        _warehouseRepositoryMock.Setup(r => r.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Warehouse?)null);

        // Act
        var result = await _warehouseService.GetByIdAsync(warehouseId);

        // Assert
        result.Should().BeNull();
        _warehouseRepositoryMock.Verify(r => r.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<WarehouseDto>(It.IsAny<Warehouse>()), Times.Never);
    }

    

    

    

    

    

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();

        _warehouseRepositoryMock
            .Setup(r => r.GetByIdAsync(warehouseId, default(CancellationToken)))
            .ReturnsAsync((Warehouse?)null);

        // Act
        var result = await _warehouseService.DeleteAsync(warehouseId);

        // Assert
        result.Should().BeFalse();
        _warehouseRepositoryMock.Verify(r => r.GetByIdAsync(warehouseId, default(CancellationToken)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(50000)]
    [InlineData(100000)]
    public async Task CreateAsync_WithDifferentCapacities_CreatesSuccessfully(int capacity)
    {
        // Arrange
        var createDto = new CreateWarehouseDto
        {
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = capacity
        };

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Location = createDto.Location,
            Capacity = capacity
        };

        var warehouseDto = new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Capacity = capacity
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Warehouse>(createDto)).Returns(warehouse);

        _warehouseRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()))
            .Returns((Warehouse w, CancellationToken ct) => Task.FromResult(w));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var warehousesQueryable = new List<Warehouse> { warehouse }.AsQueryable().BuildMock();
        _warehouseRepositoryMock.Setup(r => r.Query()).Returns(warehousesQueryable);

        _mapperMock.Setup(m => m.Map<WarehouseDto>(It.IsAny<Warehouse>())).Returns(warehouseDto);

        // Act
        var result = await _warehouseService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Capacity.Should().Be(capacity);
    }
}









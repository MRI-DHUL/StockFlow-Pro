using FluentAssertions;
using StockFlow.Application.DTOs;
using StockFlow.Application.Validators;
using Xunit;

namespace StockFlow.UnitTests.Application.Validators;

public class CreateInventoryDtoValidatorTests
{
    private readonly CreateInventoryDtoValidator _validator;

    public CreateInventoryDtoValidatorTests()
    {
        _validator = new CreateInventoryDtoValidator();
    }

    [Fact]
    public async Task Validate_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithEmptyProductId_FailsValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.Empty,
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ProductId");
        result.Errors.First(e => e.PropertyName == "ProductId").ErrorMessage.Should().Be("Product ID is required");
    }

    [Fact]
    public async Task Validate_WithEmptyWarehouseId_FailsValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.Empty,
            Quantity = 100,
            Threshold = 20
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "WarehouseId");
        result.Errors.First(e => e.PropertyName == "WarehouseId").ErrorMessage.Should().Be("Warehouse ID is required");
    }

    [Fact]
    public async Task Validate_WithNegativeQuantity_FailsValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = -10,
            Threshold = 20
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Quantity");
        result.Errors.First(e => e.PropertyName == "Quantity").ErrorMessage.Should().Be("Quantity cannot be negative");
    }

    [Fact]
    public async Task Validate_WithZeroQuantity_PassesValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 0,
            Threshold = 20
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithNegativeThreshold_FailsValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = -5
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Threshold");
        result.Errors.First(e => e.PropertyName == "Threshold").ErrorMessage.Should().Be("Threshold cannot be negative");
    }

    [Fact]
    public async Task Validate_WithZeroThreshold_PassesValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 0
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithMultipleErrors_ReportsAllErrors()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.Empty,
            WarehouseId = Guid.Empty,
            Quantity = -10,
            Threshold = -5
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
        result.Errors.Should().Contain(e => e.PropertyName == "WarehouseId");
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
        result.Errors.Should().Contain(e => e.PropertyName == "Threshold");
    }

    [Fact]
    public async Task Validate_WithLargeQuantityAndThreshold_PassesValidation()
    {
        // Arrange
        var dto = new CreateInventoryDto
        {
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 1000000,
            Threshold = 50000
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

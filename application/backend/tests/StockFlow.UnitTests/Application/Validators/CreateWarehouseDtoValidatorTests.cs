using FluentAssertions;
using StockFlow.Application.DTOs;
using StockFlow.Application.Validators;
using Xunit;

namespace StockFlow.UnitTests.Application.Validators;

public class CreateWarehouseDtoValidatorTests
{
    private readonly CreateWarehouseDtoValidator _validator;

    public CreateWarehouseDtoValidatorTests()
    {
        _validator = new CreateWarehouseDtoValidator();
    }

    [Fact]
    public async Task Validate_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateWarehouseDto
        {
            Name = "Main Warehouse",
            Location = "123 Storage St, City",
            Capacity = 10000
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithEmptyName_FailsValidation()
    {
        // Arrange
        var dto = new CreateWarehouseDto
        {
            Name = "",
            Location = "Test Location",
            Capacity = 5000
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors.First(e => e.PropertyName == "Name").ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task Validate_WithEmptyLocation_FailsValidation()
    {
        // Arrange
        var dto = new CreateWarehouseDto
        {
            Name = "Test Warehouse",
            Location = "",
            Capacity = 5000
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Location");
        result.Errors.First(e => e.PropertyName == "Location").ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task Validate_WithZeroOrNegativeCapacity_FailsValidation()
    {
        // Arrange
        var dto = new CreateWarehouseDto
        {
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = 0
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Capacity");
        result.Errors.First(e => e.PropertyName == "Capacity").ErrorMessage.Should().Contain("greater than 0");
    }

    [Fact]
    public async Task Validate_WithMinimalRequiredFields_PassesValidation()
    {
        // Arrange
        var dto = new CreateWarehouseDto
        {
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = 5000
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

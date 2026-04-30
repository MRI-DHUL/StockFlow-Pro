using FluentAssertions;
using StockFlow.Application.DTOs;
using StockFlow.Application.Validators;
using Xunit;

namespace StockFlow.UnitTests.Application.Validators;

public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;

    public CreateProductDtoValidatorTests()
    {
        _validator = new CreateProductDtoValidator();
    }

    [Fact]
    public async Task Validate_WithValidData_PassesValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            Category = "Electronics",
            UnitPrice = 99.99m,
            Description = "A test product"
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
        var dto = new CreateProductDto
        {
            Name = "",
            SKU = "TEST-001",
            UnitPrice = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors.First(e => e.PropertyName == "Name").ErrorMessage.Should().Be("Product name is required");
    }

    [Fact]
    public async Task Validate_WithTooLongName_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = new string('A', 201),
            SKU = "TEST-001",
            UnitPrice = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors.First(e => e.PropertyName == "Name").ErrorMessage.Should().Be("Product name cannot exceed 200 characters");
    }

    [Fact]
    public async Task Validate_WithEmptySKU_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "",
            UnitPrice = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SKU");
        result.Errors.First(e => e.PropertyName == "SKU").ErrorMessage.Should().Be("SKU is required");
    }

    [Fact]
    public async Task Validate_WithTooLongSKU_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = new string('A', 51),
            UnitPrice = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "SKU");
        result.Errors.First(e => e.PropertyName == "SKU").ErrorMessage.Should().Be("SKU cannot exceed 50 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public async Task Validate_WithZeroOrNegativePrice_FailsValidation(decimal price)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = price
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "UnitPrice");
        result.Errors.First(e => e.PropertyName == "UnitPrice").ErrorMessage.Should().Be("Unit price must be greater than 0");
    }

    [Fact]
    public async Task Validate_WithTooLongCategory_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m,
            Category = new string('A', 101)
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Category");
        result.Errors.First(e => e.PropertyName == "Category").ErrorMessage.Should().Be("Category cannot exceed 100 characters");
    }

    [Fact]
    public async Task Validate_WithTooLongDescription_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m,
            Description = new string('A', 1001)
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
        result.Errors.First(e => e.PropertyName == "Description").ErrorMessage.Should().Be("Description cannot exceed 1000 characters");
    }

    [Fact]
    public async Task Validate_WithNullCategory_PassesValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m,
            Category = null
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithNullDescription_PassesValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m,
            Description = null
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
        var dto = new CreateProductDto
        {
            Name = "",
            SKU = "",
            UnitPrice = -10
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "SKU");
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPrice");
    }
}

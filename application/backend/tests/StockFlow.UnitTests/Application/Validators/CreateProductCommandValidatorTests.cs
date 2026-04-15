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
    public void Validate_ValidProduct_PassesValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Valid Product",
            SKU = "VALID-SKU-001",
            Category = "Electronics",
            UnitPrice = 99.99m,
            Description = "A valid product description"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyName_FailsValidation(string? name)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = name!,
            SKU = "SKU-001",
            UnitPrice = 10.99m
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptySKU_FailsValidation(string? sku)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Product Name",
            SKU = sku!,
            UnitPrice = 10.99m
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SKU");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void Validate_InvalidUnitPrice_FailsValidation(decimal price)
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Product Name",
            SKU = "SKU-001",
            UnitPrice = price
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPrice");
    }

    [Fact]
    public void Validate_NameTooLong_FailsValidation()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = new string('A', 201), // Exceeds 200 character limit
            SKU = "SKU-001",
            UnitPrice = 10.99m
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }
}

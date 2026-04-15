using FluentAssertions;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Product_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            SKU = "TEST-001",
            Category = "Electronics",
            UnitPrice = 99.99m,
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Test Product");
        product.SKU.Should().Be("TEST-001");
        product.Category.Should().Be("Electronics");
        product.UnitPrice.Should().Be(99.99m);
        product.Description.Should().Be("Test Description");
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.IsDeleted.Should().BeFalse();
        product.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Product_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = 99.99m,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;

        // Assert
        product.IsDeleted.Should().BeTrue();
        product.DeletedAt.Should().NotBeNull();
        product.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10.50)]
    [InlineData(999.99)]
    [InlineData(1000000.00)]
    public void Product_UnitPrice_AcceptsValidValues(decimal price)
    {
        // Arrange & Act
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            SKU = "TEST-001",
            UnitPrice = price
        };

        // Assert
        product.UnitPrice.Should().Be(price);
    }
}

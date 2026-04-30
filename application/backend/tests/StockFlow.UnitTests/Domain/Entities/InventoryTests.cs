using FluentAssertions;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class InventoryTests
{
    [Fact]
    public void Inventory_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        inventory.Id.Should().NotBeEmpty();
        inventory.ProductId.Should().NotBeEmpty();
        inventory.WarehouseId.Should().NotBeEmpty();
        inventory.Quantity.Should().Be(100);
        inventory.Threshold.Should().Be(20);
        inventory.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        inventory.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Inventory_IsLowStock_ReturnsTrueWhenQuantityEqualToThreshold()
    {
        // Arrange & Act
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 20,
            Threshold = 20
        };

        // Assert
        inventory.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void Inventory_IsLowStock_ReturnsTrueWhenQuantityBelowThreshold()
    {
        // Arrange & Act
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 10,
            Threshold = 20
        };

        // Assert
        inventory.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void Inventory_IsLowStock_ReturnsFalseWhenQuantityAboveThreshold()
    {
        // Arrange & Act
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 50,
            Threshold = 20
        };

        // Assert
        inventory.IsLowStock.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 10, true)]
    [InlineData(5, 10, true)]
    [InlineData(10, 10, true)]
    [InlineData(11, 10, false)]
    [InlineData(100, 10, false)]
    public void Inventory_IsLowStock_WorksCorrectlyForDifferentValues(int quantity, int threshold, bool expectedLowStock)
    {
        // Arrange & Act
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = quantity,
            Threshold = threshold
        };

        // Assert
        inventory.IsLowStock.Should().Be(expectedLowStock);
    }

    [Fact]
    public void Inventory_Quantity_CanBeUpdated()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20,
            LastUpdated = DateTime.UtcNow
        };

        // Act
        inventory.Quantity = 50;
        inventory.LastUpdated = DateTime.UtcNow;

        // Assert
        inventory.Quantity.Should().Be(50);
        inventory.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Inventory_Threshold_CanBeUpdated()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20
        };

        // Act
        inventory.Threshold = 30;

        // Assert
        inventory.Threshold.Should().Be(30);
    }

    [Fact]
    public void Inventory_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Threshold = 20,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        inventory.IsDeleted = true;
        inventory.DeletedAt = DateTime.UtcNow;

        // Assert
        inventory.IsDeleted.Should().BeTrue();
        inventory.DeletedAt.Should().NotBeNull();
        inventory.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}

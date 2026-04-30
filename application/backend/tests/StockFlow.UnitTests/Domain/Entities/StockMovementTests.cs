using FluentAssertions;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class StockMovementTests
{
    [Fact]
    public void StockMovement_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            FromWarehouseId = Guid.NewGuid(),
            ToWarehouseId = Guid.NewGuid(),
            Quantity = 50,
            Type = MovementType.Transfer,
            Reference = "REF-001",
            Notes = "Transfer between warehouses",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        stockMovement.Id.Should().NotBeEmpty();
        stockMovement.ProductId.Should().NotBeEmpty();
        stockMovement.FromWarehouseId.Should().NotBeEmpty();
        stockMovement.ToWarehouseId.Should().NotBeEmpty();
        stockMovement.Quantity.Should().Be(50);
        stockMovement.Type.Should().Be(MovementType.Transfer);
        stockMovement.Reference.Should().Be("REF-001");
        stockMovement.Notes.Should().Be("Transfer between warehouses");
        stockMovement.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(MovementType.In)]
    [InlineData(MovementType.Out)]
    [InlineData(MovementType.Transfer)]
    [InlineData(MovementType.Adjustment)]
    public void StockMovement_Type_CanBeSetToValidValues(MovementType type)
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Type = type,
            Quantity = 10
        };

        // Assert
        stockMovement.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void StockMovement_Quantity_AcceptsPositiveValues(int quantity)
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = quantity,
            Type = MovementType.In
        };

        // Assert
        stockMovement.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void StockMovement_ForIncomingStock_FromWarehouseCanBeNull()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            FromWarehouseId = null,
            ToWarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Type = MovementType.In,
            Reference = "PO-001"
        };

        // Assert
        stockMovement.FromWarehouseId.Should().BeNull();
        stockMovement.ToWarehouseId.Should().NotBeEmpty();
        stockMovement.Type.Should().Be(MovementType.In);
    }

    [Fact]
    public void StockMovement_ForOutgoingStock_ToWarehouseCanBeNull()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            FromWarehouseId = Guid.NewGuid(),
            ToWarehouseId = null,
            Quantity = 50,
            Type = MovementType.Out,
            Reference = "ORDER-001"
        };

        // Assert
        stockMovement.FromWarehouseId.Should().NotBeEmpty();
        stockMovement.ToWarehouseId.Should().BeNull();
        stockMovement.Type.Should().Be(MovementType.Out);
    }

    [Fact]
    public void StockMovement_ForTransfer_BothWarehousesShouldBeSet()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            FromWarehouseId = Guid.NewGuid(),
            ToWarehouseId = Guid.NewGuid(),
            Quantity = 25,
            Type = MovementType.Transfer
        };

        // Assert
        stockMovement.FromWarehouseId.Should().NotBeEmpty();
        stockMovement.ToWarehouseId.Should().NotBeEmpty();
        stockMovement.Type.Should().Be(MovementType.Transfer);
    }

    [Fact]
    public void StockMovement_OptionalFields_CanBeNull()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ToWarehouseId = Guid.NewGuid(),
            Quantity = 10,
            Type = MovementType.In,
            Reference = null,
            Notes = null
        };

        // Assert
        stockMovement.Reference.Should().BeNull();
        stockMovement.Notes.Should().BeNull();
    }

    [Fact]
    public void StockMovement_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 10,
            Type = MovementType.In,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        stockMovement.IsDeleted = true;
        stockMovement.DeletedAt = DateTime.UtcNow;

        // Assert
        stockMovement.IsDeleted.Should().BeTrue();
        stockMovement.DeletedAt.Should().NotBeNull();
        stockMovement.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void StockMovement_WithReference_CanTrackSource()
    {
        // Arrange & Act
        var stockMovement = new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ToWarehouseId = Guid.NewGuid(),
            Quantity = 100,
            Type = MovementType.In,
            Reference = "PO-2024-001"
        };

        // Assert
        stockMovement.Reference.Should().Be("PO-2024-001");
    }
}

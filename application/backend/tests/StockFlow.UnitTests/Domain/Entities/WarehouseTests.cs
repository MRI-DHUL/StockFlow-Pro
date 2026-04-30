using FluentAssertions;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class WarehouseTests
{
    [Fact]
    public void Warehouse_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Main Warehouse",
            Location = "123 Storage St, City",
            Capacity = 10000,
            ContactInfo = "Manager: John Smith",
            Email = "warehouse@example.com",
            Phone = "+1-555-0100",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        warehouse.Id.Should().NotBeEmpty();
        warehouse.Name.Should().Be("Main Warehouse");
        warehouse.Location.Should().Be("123 Storage St, City");
        warehouse.Capacity.Should().Be(10000);
        warehouse.ContactInfo.Should().Be("Manager: John Smith");
        warehouse.Email.Should().Be("warehouse@example.com");
        warehouse.Phone.Should().Be("+1-555-0100");
        warehouse.Inventories.Should().BeEmpty();
        warehouse.StockMovementsFrom.Should().BeEmpty();
        warehouse.StockMovementsTo.Should().BeEmpty();
        warehouse.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(50000)]
    [InlineData(1000000)]
    public void Warehouse_Capacity_AcceptsValidValues(int capacity)
    {
        // Arrange & Act
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = capacity
        };

        // Assert
        warehouse.Capacity.Should().Be(capacity);
    }

    [Fact]
    public void Warehouse_NavigationProperties_InitializeAsEmptyCollections()
    {
        // Arrange & Act
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Test Warehouse",
            Location = "Test Location"
        };

        // Assert
        warehouse.Inventories.Should().NotBeNull();
        warehouse.Inventories.Should().BeEmpty();
        warehouse.StockMovementsFrom.Should().NotBeNull();
        warehouse.StockMovementsFrom.Should().BeEmpty();
        warehouse.StockMovementsTo.Should().NotBeNull();
        warehouse.StockMovementsTo.Should().BeEmpty();
    }

    [Fact]
    public void Warehouse_OptionalFields_CanBeNull()
    {
        // Arrange & Act
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = 5000,
            ContactInfo = null,
            Email = null,
            Phone = null
        };

        // Assert
        warehouse.ContactInfo.Should().BeNull();
        warehouse.Email.Should().BeNull();
        warehouse.Phone.Should().BeNull();
    }

    [Fact]
    public void Warehouse_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Test Warehouse",
            Location = "Test Location",
            Capacity = 5000,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        warehouse.IsDeleted = true;
        warehouse.DeletedAt = DateTime.UtcNow;

        // Assert
        warehouse.IsDeleted.Should().BeTrue();
        warehouse.DeletedAt.Should().NotBeNull();
        warehouse.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Warehouse_AllProperties_CanBeUpdated()
    {
        // Arrange
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Location = "Old Location",
            Capacity = 5000,
            Email = "old@example.com"
        };

        // Act
        warehouse.Name = "New Name";
        warehouse.Location = "New Location";
        warehouse.Capacity = 10000;
        warehouse.Email = "new@example.com";

        // Assert
        warehouse.Name.Should().Be("New Name");
        warehouse.Location.Should().Be("New Location");
        warehouse.Capacity.Should().Be(10000);
        warehouse.Email.Should().Be("new@example.com");
    }
}

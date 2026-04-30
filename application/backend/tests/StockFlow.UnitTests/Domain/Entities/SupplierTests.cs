using FluentAssertions;
using StockFlow.Domain.Entities;
using Xunit;

namespace StockFlow.UnitTests.Domain.Entities;

public class SupplierTests
{
    [Fact]
    public void Supplier_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "ABC Suppliers Inc.",
            ContactInfo = "Contact: Jane Doe",
            Email = "contact@abcsuppliers.com",
            Phone = "+1-555-0200",
            LeadTimeDays = 14,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        supplier.Id.Should().NotBeEmpty();
        supplier.Name.Should().Be("ABC Suppliers Inc.");
        supplier.ContactInfo.Should().Be("Contact: Jane Doe");
        supplier.Email.Should().Be("contact@abcsuppliers.com");
        supplier.Phone.Should().Be("+1-555-0200");
        supplier.LeadTimeDays.Should().Be(14);
        supplier.PurchaseOrders.Should().BeEmpty();
        supplier.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(14)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(90)]
    public void Supplier_LeadTimeDays_AcceptsValidValues(int leadTimeDays)
    {
        // Arrange & Act
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier",
            LeadTimeDays = leadTimeDays
        };

        // Assert
        supplier.LeadTimeDays.Should().Be(leadTimeDays);
    }

    [Fact]
    public void Supplier_PurchaseOrders_InitializesAsEmptyCollection()
    {
        // Arrange & Act
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier"
        };

        // Assert
        supplier.PurchaseOrders.Should().NotBeNull();
        supplier.PurchaseOrders.Should().BeEmpty();
    }

    [Fact]
    public void Supplier_OptionalFields_CanBeNull()
    {
        // Arrange & Act
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier",
            ContactInfo = null,
            Email = null,
            Phone = null,
            LeadTimeDays = 7
        };

        // Assert
        supplier.ContactInfo.Should().BeNull();
        supplier.Email.Should().BeNull();
        supplier.Phone.Should().BeNull();
    }

    [Fact]
    public void Supplier_SoftDelete_SetsDeletedProperties()
    {
        // Arrange
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Test Supplier",
            LeadTimeDays = 14,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        supplier.IsDeleted = true;
        supplier.DeletedAt = DateTime.UtcNow;

        // Assert
        supplier.IsDeleted.Should().BeTrue();
        supplier.DeletedAt.Should().NotBeNull();
        supplier.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Supplier_AllProperties_CanBeUpdated()
    {
        // Arrange
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Old Supplier",
            Email = "old@example.com",
            Phone = "123-456-7890",
            LeadTimeDays = 7
        };

        // Act
        supplier.Name = "New Supplier";
        supplier.Email = "new@example.com";
        supplier.Phone = "098-765-4321";
        supplier.LeadTimeDays = 14;

        // Assert
        supplier.Name.Should().Be("New Supplier");
        supplier.Email.Should().Be("new@example.com");
        supplier.Phone.Should().Be("098-765-4321");
        supplier.LeadTimeDays.Should().Be(14);
    }

    [Fact]
    public void Supplier_WithZeroLeadTime_IsValid()
    {
        // Arrange & Act
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Fast Supplier",
            LeadTimeDays = 0
        };

        // Assert
        supplier.LeadTimeDays.Should().Be(0);
    }
}

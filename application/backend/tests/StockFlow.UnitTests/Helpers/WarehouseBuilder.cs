using StockFlow.Domain.Entities;

namespace StockFlow.UnitTests.Helpers;

/// <summary>
/// Test data builder for Warehouse entities
/// </summary>
public class WarehouseBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Warehouse";
    private string _location = "Test Location";
    private int _capacity = 10000;
    private string? _contactInfo;
    private string? _email;
    private string? _phone;
    private DateTime _createdAt = DateTime.UtcNow;

    public WarehouseBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public WarehouseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public WarehouseBuilder WithLocation(string location)
    {
        _location = location;
        return this;
    }

    public WarehouseBuilder WithCapacity(int capacity)
    {
        _capacity = capacity;
        return this;
    }

    public WarehouseBuilder WithContactInfo(string? contactInfo)
    {
        _contactInfo = contactInfo;
        return this;
    }

    public WarehouseBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public WarehouseBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public Warehouse Build()
    {
        return new Warehouse
        {
            Id = _id,
            Name = _name,
            Location = _location,
            Capacity = _capacity,
            ContactInfo = _contactInfo,
            Email = _email,
            Phone = _phone,
            CreatedAt = _createdAt
        };
    }

    public static WarehouseBuilder Create() => new();
}

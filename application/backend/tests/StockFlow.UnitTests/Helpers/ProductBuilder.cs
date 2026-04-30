using StockFlow.Domain.Entities;

namespace StockFlow.UnitTests.Helpers;

/// <summary>
/// Test data builder for Product entities
/// </summary>
public class ProductBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Product";
    private string _sku = "TEST-001";
    private string? _category = "Electronics";
    private decimal _unitPrice = 99.99m;
    private string? _description = "Test description";
    private DateTime _createdAt = DateTime.UtcNow;
    private bool _isDeleted = false;
    private DateTime? _deletedAt = null;

    public ProductBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithSKU(string sku)
    {
        _sku = sku;
        return this;
    }

    public ProductBuilder WithCategory(string? category)
    {
        _category = category;
        return this;
    }

    public ProductBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public ProductBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ProductBuilder ThatIsDeleted()
    {
        _isDeleted = true;
        _deletedAt = DateTime.UtcNow;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = _name,
            SKU = _sku,
            Category = _category,
            UnitPrice = _unitPrice,
            Description = _description,
            CreatedAt = _createdAt,
            IsDeleted = _isDeleted,
            DeletedAt = _deletedAt
        };
    }

    public static ProductBuilder Create() => new();
}

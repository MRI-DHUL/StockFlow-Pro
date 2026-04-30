using StockFlow.Domain.Entities;

namespace StockFlow.UnitTests.Helpers;

/// <summary>
/// Test data builder for Inventory entities
/// </summary>
public class InventoryBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _productId = Guid.NewGuid();
    private Guid _warehouseId = Guid.NewGuid();
    private int _quantity = 100;
    private int _threshold = 20;
    private DateTime _lastUpdated = DateTime.UtcNow;
    private DateTime _createdAt = DateTime.UtcNow;
    private Product? _product;
    private Warehouse? _warehouse;

    public InventoryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public InventoryBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public InventoryBuilder WithWarehouseId(Guid warehouseId)
    {
        _warehouseId = warehouseId;
        return this;
    }

    public InventoryBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public InventoryBuilder WithThreshold(int threshold)
    {
        _threshold = threshold;
        return this;
    }

    public InventoryBuilder WithLastUpdated(DateTime lastUpdated)
    {
        _lastUpdated = lastUpdated;
        return this;
    }

    public InventoryBuilder WithProduct(Product product)
    {
        _product = product;
        _productId = product.Id;
        return this;
    }

    public InventoryBuilder WithWarehouse(Warehouse warehouse)
    {
        _warehouse = warehouse;
        _warehouseId = warehouse.Id;
        return this;
    }

    public InventoryBuilder ThatIsLowStock()
    {
        _quantity = _threshold - 5;
        return this;
    }

    public Inventory Build()
    {
        var inventory = new Inventory
        {
            Id = _id,
            ProductId = _productId,
            WarehouseId = _warehouseId,
            Quantity = _quantity,
            Threshold = _threshold,
            LastUpdated = _lastUpdated,
            CreatedAt = _createdAt
        };

        if (_product != null)
            inventory.Product = _product;

        if (_warehouse != null)
            inventory.Warehouse = _warehouse;

        return inventory;
    }

    public static InventoryBuilder Create() => new();
}

using Mapster;
using StockFlow.Application.DTOs;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Product mappings
        config.NewConfig<Product, ProductDto>();
        config.NewConfig<CreateProductDto, Product>();
        config.NewConfig<UpdateProductDto, Product>()
            .IgnoreNullValues(true);

        // Inventory mappings
        config.NewConfig<Domain.Entities.Inventory, InventoryDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSKU, src => src.Product.SKU)
            .Map(dest => dest.WarehouseName, src => src.Warehouse.Name);
        config.NewConfig<CreateInventoryDto, Domain.Entities.Inventory>();
        config.NewConfig<UpdateInventoryDto, Domain.Entities.Inventory>()
            .IgnoreNullValues(true);

        // Warehouse mappings
        config.NewConfig<Domain.Entities.Warehouse, WarehouseDto>();
        config.NewConfig<CreateWarehouseDto, Domain.Entities.Warehouse>();
        config.NewConfig<UpdateWarehouseDto, Domain.Entities.Warehouse>()
            .IgnoreNullValues(true);

        // Order mappings
        config.NewConfig<Domain.Entities.Order, OrderDto>();
        config.NewConfig<Domain.Entities.OrderItem, OrderItemDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSKU, src => src.Product.SKU);

        // Supplier mappings
        config.NewConfig<Domain.Entities.Supplier, SupplierDto>();
        config.NewConfig<CreateSupplierDto, Domain.Entities.Supplier>();
        config.NewConfig<UpdateSupplierDto, Domain.Entities.Supplier>()
            .IgnoreNullValues(true);

        // PurchaseOrder mappings
        config.NewConfig<Domain.Entities.PurchaseOrder, PurchaseOrderDto>()
            .Map(dest => dest.SupplierName, src => src.Supplier.Name);
        config.NewConfig<Domain.Entities.PurchaseOrderItem, PurchaseOrderItemDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSKU, src => src.Product.SKU);

        // StockMovement mappings
        config.NewConfig<Domain.Entities.StockMovement, StockMovementDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSKU, src => src.Product.SKU)
            .Map(dest => dest.FromWarehouseName, src => src.FromWarehouse != null ? src.FromWarehouse.Name : null)
            .Map(dest => dest.ToWarehouseName, src => src.ToWarehouse != null ? src.ToWarehouse.Name : null);
        config.NewConfig<CreateStockMovementDto, Domain.Entities.StockMovement>();
    }
}

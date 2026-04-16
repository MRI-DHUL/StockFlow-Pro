using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Services;
using System.Reflection;

namespace StockFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Services (3-tier architecture)
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IStockMovementService, StockMovementService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        // Add Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}

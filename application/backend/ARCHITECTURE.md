# Architecture Documentation

## Overview

StockFlow Pro Backend uses a **3-Tier Architecture** pattern for clean separation of concerns and maintainability.

## Architecture Pattern: 3-Tier

### Layers

```
┌─────────────────────────────────────────────┐
│         Controller Layer (API)               │
│  - HTTP Request/Response handling            │
│  - Route mapping                             │
│  - Model binding                             │
│  - Returns DTOs                              │
└─────────────────┬───────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────────┐
│          Service Layer (Business)            │
│  - Business logic execution                  │
│  - Direct validation (FluentValidation)      │
│  - Data transformation                       │
│  - Orchestrates repositories                 │
│  - Caching logic                             │
└─────────────────┬───────────────────────────┘
                  │
                  ↓
┌─────────────────────────────────────────────┐
│      Repository Layer (Data Access)          │
│  - Entity Framework Core                     │
│  - Database operations                       │
│  - Unit of Work pattern                      │
│  - Query composition                         │
└─────────────────────────────────────────────┘
```

## Request Flow

### Example: Create Product

```
1. HTTP POST /api/v1/products
   ↓
2. ProductsController.Create(CreateProductDto dto)
   ↓
3. _productService.CreateAsync(dto)
   ↓
4. Validate DTO using FluentValidation
   ↓
5. Map DTO to Entity using Mapster
   ↓
6. _unitOfWork.Products.AddAsync(product)
   ↓
7. _unitOfWork.SaveChangesAsync()
   ↓
8. Invalidate cache
   ↓
9. Map Entity back to DTO
   ↓
10. Return 201 Created with ProductDto
```

## Service Layer

### Purpose
- Encapsulates all business logic
- Coordinates between controllers and repositories
- Handles validation explicitly
- Manages caching strategy
- Provides clean separation of concerns

### Services

| Service | Responsibility |
|---------|----------------|
| **ProductService** | Product CRUD, caching, search, pagination |
| **OrderService** | Order creation, status updates, calculations |
| **InventoryService** | Stock tracking, low stock detection, multi-warehouse |
| **SupplierService** | Supplier management |
| **WarehouseService** | Warehouse operations |
| **StockMovementService** | Inventory movements, automatic adjustments |
| **PurchaseOrderService** | PO management, supplier integration |
| **AuditLogService** | Audit trail queries |

### Service Pattern

```csharp
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IValidator<CreateProductDto> _validator;

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        // 1. Validate
        await _validator.ValidateAndThrowAsync(dto, ct);
        
        // 2. Map
        var product = _mapper.Map<Product>(dto);
        
        // 3. Persist
        await _unitOfWork.Products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        // 4. Cache management
        await _cacheService.RemoveAsync("products_all", ct);
        
        // 5. Return
        return _mapper.Map<ProductDto>(product);
    }
}
```

## Repository Layer

### Pattern
- Generic Repository for common operations
- Specialized repositories for complex queries
- Unit of Work for transaction management

### Structure

```csharp
// Generic repository
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    IQueryable<T> Query();
}

// Specialized repository
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySKUAsync(string sku, CancellationToken ct = default);
    Task<bool> IsSKUUniqueAsync(string sku, Guid? excludeId, CancellationToken ct = default);
}

// Unit of Work
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IInventoryRepository Inventories { get; }
    IRepository<Order> Orders { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

## Validation Strategy

### Direct Validation in Services

**Why:**
- Clear visibility of validation logic
- No hidden pipeline behaviors
- Explicit control over when validation occurs
- Easier to debug and test
- Simpler architecture

**Implementation:**

```csharp
// In Service
public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
{
    // Explicit validation
    await _createValidator.ValidateAndThrowAsync(dto, ct);
    
    // Business logic
    var order = new Order { /* ... */ };
    await _unitOfWork.Orders.AddAsync(order, ct);
    await _unitOfWork.SaveChangesAsync(ct);
    
    return _mapper.Map<OrderDto>(order);
}
```

## Caching Strategy

### Implementation
- Redis for distributed caching
- In-memory fallback if Redis unavailable
- Service layer manages cache lifecycle

### Patterns

```csharp
// Read-through caching
public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
{
    // Try cache
    var cached = await _cacheService.GetAsync<List<ProductDto>>("products_all", ct);
    if (cached != null) return cached;
    
    // Load from DB
    var products = await _productRepository.GetAllAsync(ct);
    var dtos = _mapper.Map<List<ProductDto>>(products);
    
    // Update cache
    await _cacheService.SetAsync("products_all", dtos, TimeSpan.FromMinutes(15), ct);
    
    return dtos;
}

// Cache invalidation on write
public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
{
    // ... create logic ...
    
    // Invalidate cache
    await _cacheService.RemoveAsync("products_all", ct);
    
    return result;
}
```

## Benefits of 3-Tier Architecture

### ✅ Simplicity
- Straightforward request flow
- No hidden pipeline behaviors
- Easy to understand for new developers

### ✅ Maintainability
- Clear separation of concerns
- Service layer is the single source for business logic
- Easy to modify without affecting other layers

### ✅ Testability
- Services can be unit tested independently
- Mock repositories easily
- No complex mediator patterns to test

### ✅ Performance
- Reduced overhead (no MediatR)
- Direct method calls
- Efficient caching at service level

### ✅ Debugging
- Stack traces are clear
- No hidden pipeline steps
- Easy to set breakpoints

## Dependency Injection

### Registration (DependencyInjection.cs)

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Register Services
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<IOrderService, OrderService>();
    services.AddScoped<IInventoryService, InventoryService>();
    services.AddScoped<ISupplierService, SupplierService>();
    services.AddScoped<IWarehouseService, WarehouseService>();
    services.AddScoped<IStockMovementService, StockMovementService>();
    services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    services.AddScoped<IAuditLogService, AuditLogService>();
    
    // Mapster
    services.AddSingleton(TypeAdapterConfig.GlobalSettings);
    services.AddScoped<IMapper, ServiceMapper>();
    
    // FluentValidation
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    
    return services;
}
```

## Comparison: Before (CQRS) vs After (3-Tier)

| Aspect | CQRS with MediatR | 3-Tier with Services |
|--------|-------------------|----------------------|
| **Complexity** | High (Commands, Queries, Handlers) | Low (Just Services) |
| **Files** | ~50+ handlers | 8 services |
| **Validation** | Pipeline behavior (hidden) | Explicit in services |
| **Dependencies** | MediatR required | No extra packages |
| **Learning Curve** | Steep | Easy |
| **Debugging** | Complex (pipeline) | Simple (direct calls) |
| **Performance** | Slightly slower (mediator overhead) | Faster (direct calls) |
| **Testability** | Complex (mock mediator) | Simple (mock repositories) |

## Best Practices

### ✅ DO
- Keep business logic in services
- Use DTOs for all API communication
- Validate at service entry points
- Use dependency injection for all dependencies
- Implement caching at service level
- Use async/await throughout

### ❌ DON'T
- Put business logic in controllers
- Bypass services to call repositories directly
- Skip validation
- Use entities directly in controllers
- Hardcode dependencies
- Mix concerns between layers

## Future Scalability

This architecture supports:
- ✅ Horizontal scaling (stateless services)
- ✅ Microservices migration (services become APIs)
- ✅ Event sourcing (add at service level)
- ✅ CQRS (can be added back if needed)
- ✅ Multi-tenancy (add tenant context in services)
- ✅ Feature flags (implement in services)

## Conclusion

The 3-tier architecture provides a **clean, maintainable, and performant** foundation for StockFlow Pro. It balances simplicity with enterprise-grade patterns, making it suitable for both current needs and future growth.

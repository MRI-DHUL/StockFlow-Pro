# Project Structure - StockFlow Pro Backend

```
StockFlow-Pro/application/backend/
│
├── StockFlow.sln                          # Solution file
│
├── src/
│   │
│   ├── StockFlow.Domain/                  # Domain Layer (Core Business Entities)
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs             # Base class for all entities
│   │   │   └── IAuditableEntity.cs       # Interface for auditable entities
│   │   │
│   │   ├── Entities/
│   │   │   ├── Product.cs                # Product entity
│   │   │   ├── Warehouse.cs              # Warehouse entity
│   │   │   ├── Inventory.cs              # Inventory entity
│   │   │   ├── Order.cs                  # Order entity
│   │   │   ├── OrderItem.cs              # Order item entity
│   │   │   ├── Supplier.cs               # Supplier entity
│   │   │   ├── PurchaseOrder.cs          # Purchase order entity
│   │   │   ├── PurchaseOrderItem.cs      # Purchase order item entity
│   │   │   └── StockMovement.cs          # Stock movement entity
│   │   │
│   │   └── Enums/
│   │       ├── OrderStatus.cs            # Order status enumeration
│   │       ├── PurchaseOrderStatus.cs    # PO status enumeration
│   │       └── MovementType.cs           # Stock movement types
│   │
│   ├── StockFlow.Application/             # Application Layer (Use Cases, CQRS)
│   │   ├── Commands/                      # Command handlers (to be created)
│   │   ├── Queries/                       # Query handlers (to be created)
│   │   ├── DTOs/                          # Data Transfer Objects (to be created)
│   │   ├── Interfaces/                    # Repository interfaces (to be created)
│   │   └── Behaviors/                     # Pipeline behaviors (to be created)
│   │
│   ├── StockFlow.Infrastructure/          # Infrastructure Layer (Data Access, External Services)
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs   # EF Core DbContext
│   │   │   ├── Configurations/           # Entity configurations
│   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   ├── WarehouseConfiguration.cs
│   │   │   │   ├── InventoryConfiguration.cs
│   │   │   │   ├── OrderConfiguration.cs
│   │   │   │   ├── OrderItemConfiguration.cs
│   │   │   │   ├── SupplierConfiguration.cs
│   │   │   │   ├── PurchaseOrderConfiguration.cs
│   │   │   │   ├── PurchaseOrderItemConfiguration.cs
│   │   │   │   └── StockMovementConfiguration.cs
│   │   │   │
│   │   │   └── Migrations/
│   │   │       ├── 20260415105950_InitialCreate.cs
│   │   │       ├── 20260415105950_InitialCreate.Designer.cs
│   │   │       └── ApplicationDbContextModelSnapshot.cs
│   │   │
│   │   ├── Repositories/                  # Repository implementations (to be created)
│   │   ├── Services/                      # External service implementations (to be created)
│   │   └── DependencyInjection.cs        # DI registration for infrastructure
│   │
│   └── StockFlow.API/                     # API Layer (Controllers, Middleware)
│       ├── Controllers/                   # API controllers (to be created)
│       ├── Middleware/                    # Custom middleware (to be created)
│       ├── Program.cs                     # Application entry point
│       ├── appsettings.json              # Production settings
│       └── appsettings.Development.json  # Development settings
│
├── tests/                                 # Test projects (to be created)
│   ├── StockFlow.UnitTests/
│   └── StockFlow.IntegrationTests/
│
├── README.md                              # Main backend documentation
└── DATABASE_SETUP.md                      # Database setup guide
```

---

## Layer Dependencies

```
API Layer
   ↓ depends on
Application Layer
   ↓ depends on
Domain Layer
   ↑ implements
Infrastructure Layer
```

**Key Principle:** Domain layer has NO dependencies on other layers.

---

## Clean Architecture Flow

1. **Request comes in** → API Controller
2. **Controller** → Calls Application Command/Query Handler (via MediatR)
3. **Handler** → Uses Domain Entities and Repository Interfaces
4. **Infrastructure** → Implements Repository Interfaces using EF Core
5. **Response flows back** → Handler → Controller → Client

---

## Current Status

✅ **Completed:**
- Project structure created
- Domain entities defined
- Entity configurations completed
- Database migrations created
- Connection string configured

⏭️ **Next Steps:**
1. Create repository interfaces in Application layer
2. Implement repositories in Infrastructure layer
3. Set up MediatR for CQRS
4. Create command and query handlers
5. Build API controllers
6. Add authentication & authorization
7. Implement validation with FluentValidation
8. Add logging with Serilog

---

## Database Tables Created

All tables from the initial migration:
- ✅ Products
- ✅ Warehouses
- ✅ Inventories
- ✅ Orders
- ✅ OrderItems
- ✅ Suppliers
- ✅ PurchaseOrders
- ✅ PurchaseOrderItems
- ✅ StockMovements

---

## Configuration Files

- `appsettings.json` - Production configuration
- `appsettings.Development.json` - Development configuration
- Connection string configured for PostgreSQL (local or Supabase)

---

## Commands Reference

### Build
```bash
dotnet build
```

### Run API
```bash
cd src/StockFlow.API
dotnet run
```

### Create Migration
```bash
dotnet ef migrations add MigrationName --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### Apply Migrations
```bash
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

**Ready to build features! 🚀**

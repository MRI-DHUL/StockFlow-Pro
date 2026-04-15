# StockFlow Pro - Backend

---

## 📌 Overview

ASP.NET Core 8.0 Web API implementing Clean Architecture with CQRS pattern for enterprise-grade inventory and supply chain management.

---

## 🏗️ Architecture

### Clean Architecture Layers

```
/src
  /StockFlow.API           → Presentation Layer (Controllers, Middleware)
  /StockFlow.Application   → Application Layer (Use Cases, CQRS)
  /StockFlow.Domain        → Domain Layer (Entities, Business Rules)
  /StockFlow.Infrastructure → Infrastructure Layer (DB, External Services)
```

### Design Principles

* **SOLID Principles** - Enforced across all layers
* **Clean Architecture** - Dependency inversion, domain isolation
* **CQRS** - Command Query Responsibility Segregation via MediatR
* **Domain-Driven Design** - Entities, Value Objects, Aggregates
* **Dependency Injection** - All dependencies injected via ASP.NET Core DI

---

## ⚙️ Tech Stack

### Core Framework
* **ASP.NET Core 8.0** - Web API framework
* **Entity Framework Core 8.0** - ORM for database access
* **MediatR** - CQRS implementation

### Database
* **PostgreSQL** - Primary database (hosted on Supabase)
* **Entity Framework Core Migrations** - Schema versioning

### Caching
* **Redis** - Distributed caching (Upstash)
* Cache frequently accessed inventory and product data
* TTL: 5-10 minutes

### Messaging & Events
* **RabbitMQ** - Event-driven communication (CloudAMQP)
* Background event processing
* Decoupled service integration

### Real-time Notifications
* **Pusher** - Real-time notifications to frontend
* WebSocket-based updates
* Live inventory and alert broadcasting

### Background Jobs
* **Hangfire** - Recurring tasks and job scheduling
* Daily prediction recalculation
* Notification retries
* Log cleanup

### Authentication & Authorization
* **JWT** - Token-based authentication
* **Role-based access control** - Admin, Manager roles
* Future: Auth0 integration

### Logging & Monitoring
* **Serilog** - Structured logging
* Log levels: Information, Warning, Error, Critical
* Request/response logging middleware

### Testing
* **xUnit** - Unit and integration testing
* **Moq** - Mocking dependencies
* **FluentAssertions** - Readable assertions

---

## 🗄️ Database Schema

### Core Tables

#### Products
```sql
Id (UUID), Name, SKU, Category, UnitPrice, CreatedAt, UpdatedAt
```

#### Warehouses
```sql
Id (UUID), Name, Location, ContactInfo, CreatedAt
```

#### Inventory
```sql
Id (UUID), ProductId (FK), WarehouseId (FK), Quantity, Threshold, LastUpdated
```

#### Orders
```sql
Id (UUID), OrderNumber, CustomerId, Status, TotalAmount, CreatedAt, CompletedAt
```

#### OrderItems
```sql
Id (UUID), OrderId (FK), ProductId (FK), Quantity, UnitPrice, Subtotal
```

#### Suppliers
```sql
Id (UUID), Name, ContactInfo, Email, Phone, LeadTimeDays
```

#### PurchaseOrders
```sql
Id (UUID), SupplierId (FK), Status, ExpectedDeliveryDate, CreatedAt
```

#### StockMovements
```sql
Id (UUID), ProductId (FK), FromWarehouseId (FK), ToWarehouseId (FK), Quantity, Type (IN/OUT/TRANSFER), Timestamp
```

---

## 🔌 API Endpoints

### Authentication
* `POST /api/auth/register` - Register new user
* `POST /api/auth/login` - Login and receive JWT token
* `POST /api/auth/refresh` - Refresh access token

### Products
* `GET /api/products` - Get all products (paginated)
* `GET /api/products/{id}` - Get product by ID
* `POST /api/products` - Create new product
* `PUT /api/products/{id}` - Update product
* `DELETE /api/products/{id}` - Delete product

### Inventory
* `GET /api/inventory` - Get inventory across all warehouses
* `GET /api/inventory/{warehouseId}` - Get inventory for specific warehouse
* `POST /api/inventory/update` - Update stock levels
* `POST /api/inventory/transfer` - Transfer stock between warehouses
* `GET /api/inventory/low-stock` - Get low stock items

### Orders
* `POST /api/orders` - Create new order
* `GET /api/orders` - Get all orders (paginated)
* `GET /api/orders/{id}` - Get order details
* `PUT /api/orders/{id}/status` - Update order status
* `DELETE /api/orders/{id}` - Cancel order

### Suppliers
* `GET /api/suppliers` - Get all suppliers
* `GET /api/suppliers/{id}` - Get supplier by ID
* `POST /api/suppliers` - Create supplier
* `PUT /api/suppliers/{id}` - Update supplier
* `DELETE /api/suppliers/{id}` - Delete supplier

### Warehouses
* `GET /api/warehouses` - Get all warehouses
* `POST /api/warehouses` - Create warehouse
* `PUT /api/warehouses/{id}` - Update warehouse

### Predictions
* `GET /api/predictions/demand/{productId}` - Get demand forecast
* `GET /api/predictions/depletion/{productId}` - Stock depletion estimate
* `POST /api/predictions/recalculate` - Trigger recalculation

---

## 🔗 Event-Driven Architecture

### Domain Events (RabbitMQ)

#### Published Events
* `OrderPlacedEvent` - When order is created
* `StockUpdatedEvent` - When inventory changes
* `LowStockDetectedEvent` - When stock falls below threshold
* `StockTransferInitiatedEvent` - When transfer starts
* `SupplierOrderCreatedEvent` - When purchase order is created

#### Event Handlers
* **OrderPlacedHandler** - Deducts inventory, updates stock
* **StockUpdatedHandler** - Triggers prediction recalculation
* **LowStockHandler** - Creates notification, triggers Pusher alert

### Event Flow Example
```
1. Order Created → OrderPlacedEvent published to RabbitMQ
2. OrderPlacedHandler consumes event → Updates inventory
3. Inventory updated → StockUpdatedEvent published
4. StockUpdatedHandler checks threshold
5. If low → LowStockDetectedEvent published
6. LowStockHandler triggers Pusher notification
7. Frontend receives real-time alert
```

---

## ⚡ Caching Strategy

### Cached Data
* Product catalog (10 min TTL)
* Inventory levels (5 min TTL)
* Warehouse list (30 min TTL)
* User sessions (session-based)

### Cache Invalidation
* Inventory updates → Clear inventory cache
* Product changes → Clear product cache
* Manual cache clear endpoint for admins

---

## 🧵 Background Jobs (Hangfire)

### Recurring Jobs
* **Daily Prediction Recalculation** - Every day at 2 AM
* **Low Stock Check** - Every 6 hours
* **Failed Notification Retry** - Every 30 minutes
* **Log Cleanup** - Weekly, remove logs older than 30 days

### On-Demand Jobs
* Bulk inventory updates
* Report generation
* Data exports

---

## 🔐 Authentication & Authorization

### JWT Configuration
* Access token expiry: 1 hour
* Refresh token expiry: 7 days
* Algorithm: HS256

### Roles
* **Admin** - Full system access
* **Manager** - Inventory and order management
* **Viewer** - Read-only access

### Protected Endpoints
* All endpoints require authentication except `/api/auth/login` and `/api/auth/register`
* Role-based authorization on sensitive operations

---

## 🚀 Getting Started

### Prerequisites
* .NET 8.0 SDK
* PostgreSQL (or Supabase account)
* Redis (local or Upstash)
* RabbitMQ (local or CloudAMQP)

### Installation

1. **Clone and navigate**
   ```bash
   cd backend
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure settings**
   Update `src/StockFlow.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=stockflow;Username=postgres;Password=yourpassword"
     },
     "JwtSettings": {
       "Secret": "your-secret-key-min-32-chars",
       "Issuer": "StockFlowAPI",
       "Audience": "StockFlowClient"
     },
     "Redis": {
       "Configuration": "localhost:6379"
     },
     "RabbitMQ": {
       "HostName": "localhost",
       "UserName": "guest",
       "Password": "guest"
     },
     "Pusher": {
       "AppId": "your-app-id",
       "Key": "your-key",
       "Secret": "your-secret",
       "Cluster": "your-cluster"
     }
   }
   ```

4. **Run migrations**
   ```bash
   dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
   ```

5. **Run the application**
   ```bash
   cd src/StockFlow.API
   dotnet run
   ```

6. **Access Swagger UI**
   Navigate to: `https://localhost:5001/swagger`

### Development Commands

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Create new migration
dotnet ef migrations add MigrationName --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API

# Update database
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API

# Drop database
dotnet ef database drop --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

## 🧪 Testing

### Test Structure
```
/tests
  /StockFlow.UnitTests        → Domain and Application layer tests
  /StockFlow.IntegrationTests → API endpoint tests
```

### Running Tests
```bash
# All tests
dotnet test

# Specific project
dotnet test tests/StockFlow.UnitTests

# With coverage
dotnet test /p:CollectCoverage=true
```

### Test Coverage Goals
* Domain layer: 90%+
* Application layer: 85%+
* API layer: 70%+

---

## 📜 Logging

### Log Levels
* **Information** - Normal operations, successful requests
* **Warning** - Unusual but handled scenarios
* **Error** - Handled errors, validation failures
* **Critical** - Unhandled exceptions, system failures

### Log Enrichment
* Request ID
* User ID
* Timestamp
* Source context

### Log Outputs
* Console (development)
* File (production)
* Seq/ELK (optional monitoring)

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

```yaml
# .github/workflows/backend-ci.yml
1. Restore dependencies
2. Build solution
3. Run unit tests
4. Run integration tests
5. Publish artifacts
6. Deploy to Azure App Service
```

### Deployment Environments
* **Development** - Auto-deploy from `develop` branch
* **Staging** - Auto-deploy from `main` branch
* **Production** - Manual approval required

---

## 📦 Project Structure

```
/backend
  /src
    /StockFlow.API
      Controllers/
      Middleware/
      Program.cs
      appsettings.json
    /StockFlow.Application
      Commands/
      Queries/
      Handlers/
      Interfaces/
      DTOs/
    /StockFlow.Domain
      Entities/
      ValueObjects/
      Events/
      Interfaces/
    /StockFlow.Infrastructure
      Persistence/
      Repositories/
      Services/
      Configurations/
  /tests
    /StockFlow.UnitTests
    /StockFlow.IntegrationTests
```

---

## 🛠️ Coding Standards

### Naming Conventions
* **Classes**: PascalCase
* **Methods**: PascalCase
* **Variables**: camelCase
* **Constants**: UPPER_CASE
* **Interfaces**: IPascalCase

### Best Practices
* Keep methods small and focused
* Use async/await for I/O operations
* Implement proper error handling
* Write XML documentation for public APIs
* Follow SOLID principles
* Use dependency injection

---

## 🔧 Configuration

### Environment Variables
* `ASPNETCORE_ENVIRONMENT` - Development/Staging/Production
* `DATABASE_URL` - PostgreSQL connection string
* `REDIS_URL` - Redis connection string
* `RABBITMQ_URL` - RabbitMQ connection string
* `JWT_SECRET` - JWT signing key
* `PUSHER_*` - Pusher credentials

---

## 📚 Additional Resources

* [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
* [Entity Framework Core](https://docs.microsoft.com/ef/core)
* [MediatR](https://github.com/jbogard/MediatR)
* [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 🚨 Troubleshooting

### Common Issues

**Database connection fails**
- Verify connection string in appsettings.json
- Ensure PostgreSQL is running
- Check firewall settings

**Migrations not applying**
- Delete Migrations folder and recreate
- Ensure correct project paths in commands
- Check database permissions

**Redis connection timeout**
- Verify Redis is running
- Check connection string
- Ensure network connectivity

**RabbitMQ connection refused**
- Verify RabbitMQ service is running
- Check credentials and hostname
- Ensure correct port (5672)

---

## 📄 License

This project is part of StockFlow Pro portfolio demonstration.

---

**Backend Version**: 1.0.0  
**Last Updated**: April 2026

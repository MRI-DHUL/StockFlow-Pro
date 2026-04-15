# StockFlow Pro

---

## 📌 Project Organization

This application is organized into two main folders:

### 📁 Backend (`/backend`)
ASP.NET Core 8.0 Web API with Clean Architecture
- **Technology**: .NET 8.0, EF Core, PostgreSQL
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Pattern**: CQRS with MediatR
- **Documentation**: See [backend/README.md](backend/README.md)

### 📁 Frontend (`/frontend`)  
Angular application with Tailwind CSS *(to be implemented)*
- **Technology**: Angular, Tailwind CSS, TypeScript
- **Features**: Real-time dashboard, inventory management
- **Documentation**: See [frontend/README.md](frontend/README.md)

---

## 🚀 Quick Start

### Backend Setup

1. **Navigate to backend**:
   ```bash
   cd backend
   ```

2. **Update database connection** in `src/StockFlow.API/appsettings.json`

3. **Run migrations**:
   ```bash
   dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
   ```

4. **Run the API**:
   ```bash
   cd src/StockFlow.API
   dotnet run
   ```

5. **Access Swagger**: https://localhost:5001/swagger

### Frontend Setup *(Coming Soon)*

```bash
cd frontend
npm install
ng serve
```

---

## 📌 1. Overview

**StockFlow Pro** is a full-stack, enterprise-grade application designed to manage inventory, optimize supply chain operations, and provide predictive insights for stock planning.

This system is intentionally built to reflect **7+ years engineering maturity** by emphasizing:

* Clean Architecture and SOLID principles
* Event-driven design
* Scalability and performance considerations
* Observability and reliability

---

## 🎯 2. Objectives

* Build a **production-ready system** (not a demo CRUD app)
* Demonstrate **backend depth with .NET**
* Implement **event-driven communication**
* Provide **inventory prediction/forecasting**
* Follow **clean code + SOLID + testability**
* Showcase **system design thinking**

---

## 🧱 3. Core Features

### Inventory Management

* Product CRUD
* Stock tracking per warehouse
* Threshold-based alerts

### Order Management

* Create orders (sales)
* Automatic stock deduction
* Order status lifecycle

### Supplier Management

* Supplier CRUD
* Lead time tracking
* Purchase orders

### Multi-Warehouse

* Inventory per location
* Stock transfer between warehouses

### Prediction Engine

* Demand forecasting (moving average / trend)
* Stock depletion estimation ("out in X days")

### Notifications

* Low stock alerts
* Real-time notifications using Pusher
* Event-triggered UI updates
* Instant dashboard updates via WebSockets (Pusher Channels)

---

## 🧠 4. Architecture Principles

### Mandatory Practices

* SOLID principles
* Clean Architecture
* Separation of Concerns
* Basic DDD (Entities, Value Objects)

### Layers

```
/src
  /API
  /Application
  /Domain
  /Infrastructure
```

### Responsibilities

* **API**: Controllers, request/response mapping
* **Application**: Use cases (CQRS via MediatR)
* **Domain**: Entities, business rules
* **Infrastructure**: DB, external services

---

## ⚙️ 5. Tech Stack

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* MediatR (CQRS)

### Frontend

* Angular
* Tailwind CSS

### Database

* PostgreSQL (Supabase)

### Caching

* Redis

### Messaging (Event Handling)

* RabbitMQ (for system events, NOT notifications)

### Real-time Communication

* Pusher (for real-time notifications and UI updates)

### Background Jobs

* Hangfire

### Authentication

* JWT
* Auth0 (optional)

### Logging

* Serilog

### CI/CD

* GitHub Actions

---

## ☁️ 6. Hosting Plan

* Backend: Azure App Service
* Database: Supabase (PostgreSQL)
* Frontend: InfinityFree (static hosting)
* Redis: Upstash (or equivalent)
* RabbitMQ: CloudAMQP (for backend events)
* Pusher: Real-time notification service

---

## 🗄️ 7. Database Schema (Initial)

### Tables

#### Products

* Id (UUID)
* Name
* SKU
* Category
* CreatedAt

#### Warehouses

* Id
* Name
* Location

#### Inventory

* Id
* ProductId (FK)
* WarehouseId (FK)
* Quantity
* Threshold

#### Orders

* Id
* OrderNumber
* Status
* CreatedAt

#### OrderItems

* Id
* OrderId (FK)
* ProductId (FK)
* Quantity

#### Suppliers

* Id
* Name
* ContactInfo

#### PurchaseOrders

* Id
* SupplierId
* ExpectedDeliveryDate

#### StockMovements

* Id
* ProductId
* FromWarehouseId
* ToWarehouseId
* Quantity
* Type (IN/OUT/TRANSFER)

---

## 🔌 8. API Design (Sample)

### Auth

* POST /api/auth/login

### Products

* GET /api/products
* POST /api/products
* PUT /api/products/{id}

### Inventory

* GET /api/inventory
* POST /api/inventory/update

### Orders

* POST /api/orders
* GET /api/orders/{id}

### Suppliers

* GET /api/suppliers
* POST /api/suppliers

---

## 🔗 9. Event-Driven Design

### Events (Handled via RabbitMQ)

* OrderPlaced
* StockUpdated
* LowStockDetected

### Flow

1. OrderPlaced → publish event (RabbitMQ)
2. Consumer updates inventory
3. StockUpdated → triggers prediction
4. LowStockDetected → triggers notification service

### Notification Flow (Pusher)

* Backend detects event (e.g., LowStockDetected)
* Backend triggers Pusher event
* Pusher broadcasts to subscribed channels
* Angular frontend listens and updates UI instantly

### Example Use Cases

* Low stock alert popups
* Live inventory updates on dashboard
* Order status updates in real-time

---

## 🧵 10. Background Jobs (Hangfire). Background Jobs (Hangfire)

* Recalculate predictions (daily)
* Retry failed notifications
* Cleanup logs

---

## ⚡ 11. Caching Strategy (Redis)

* Cache frequently accessed inventory
* Cache product list
* TTL: 5–10 minutes

---

## 🔐 12. Authentication Flow

### Phase 1

* JWT-based auth
* Role-based access (Admin, Manager)

### Phase 2 (optional)

* Auth0 integration (OAuth)

---

## 🧪 13. Testing Strategy

* Unit tests (xUnit)
* Integration tests (API level)
* Mock dependencies (Moq)

---

## 📜 14. Logging & Monitoring

* Serilog for structured logging
* Log levels: Info, Warning, Error
* Log important events and failures

---

## 🔄 15. CI/CD Pipeline

### GitHub Actions Steps

* Restore
* Build
* Test
* Publish
* Deploy to Azure

---

## 🛠️ 16. Development Phases

### Phase 1

* Setup architecture
* DB connection
* Auth

### Phase 2

* Inventory + Products
* Orders

### Phase 3

* Prediction engine
* Notifications

### Phase 4

* RabbitMQ integration
* Redis caching

### Phase 5

* Testing
* Deployment

---

## 🧾 17. Coding Standards

### Must Follow

* SOLID principles
* Clean Code
* DRY

### Practices

* Use interfaces
* Dependency Injection
* Proper naming conventions
* Small, testable methods

---

## 📦 18. Final Output

The system should:

* Be deployed and accessible
* Demonstrate event-driven architecture
* Include prediction engine
* Show clean architecture
* Handle real-world scenarios

---

## 📊 19. Portfolio Value

This project demonstrates:

* Backend expertise
* System design
* Scalability thinking
* Production readiness

---

## ⚠️ 20. Key Rules

* Do NOT reduce to CRUD
* Prioritize architecture over UI
* Build incrementally
* Think in systems, not features

---

## 🚀 Final Note

This is not just a project.

It is a **demonstration of engineering maturity (7+ years level)** through:

* Design decisions
* Code quality
* System thinking

---

**End of Document**

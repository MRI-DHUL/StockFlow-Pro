# StockFlow Pro API - Quick Start Guide

## 🚀 API Running
**Base URL:** http://localhost:5057/api  
**Swagger UI:** http://localhost:5057/swagger

---

## 🔐 Authentication

### Default Admin Account
```
Email: admin@stockflowpro.com
Password: Admin@123
```

### 1. Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password@123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1...",
  "refreshToken": "7B3F8A2C...",
  "expiresIn": 86400,
  "user": {
    "id": "...",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@stockflowpro.com",
  "password": "Admin@123"
}
```

### 3. Using JWT Token
Add to **Authorization** header in all protected endpoints:
```
Authorization: Bearer eyJhbGciOiJIUzI1...
```

**In Swagger UI:**
1. Click **Authorize** button (🔒)
2. Enter: `Bearer <your-token-here>`
3. Click **Authorize**

---

## 📦 Products API

### Get Products with Pagination & Filtering
```http
GET /api/products/paged?pageNumber=1&pageSize=10&sortBy=Name&sortDescending=false&searchTerm=laptop&category=Electronics&minPrice=500&maxPrice=2000
Authorization: Bearer <token>
```

**Response:**
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Create Product (Admin/Manager only)
```http
POST /api/products
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Laptop",
  "sku": "LAP-001",
  "description": "High-performance laptop",
  "category": "Electronics",
  "unitPrice": 999.99
}
```

---

## 📊 Inventory API

### Get Inventory with Filters
```http
GET /api/inventory/paged?pageNumber=1&pageSize=20&productId=<guid>&warehouseId=<guid>&minQuantity=10&sortBy=Quantity&sortDescending=true
Authorization: Bearer <token>
```

### Get Low Stock Items
```http
GET /api/inventory/low-stock
Authorization: Bearer <token>
```

---

## 📋 Orders API

### Get Orders with Date Range
```http
GET /api/orders/paged?pageNumber=1&pageSize=10&status=Pending&createdFrom=2026-01-01&createdTo=2026-12-31&minTotalAmount=100&sortBy=CreatedAt&sortDescending=true
Authorization: Bearer <token>
```

**Status values:** Pending, Processing, Shipped, Delivered, Cancelled

### Create Order
```http
POST /api/orders
Authorization: Bearer <token>
Content-Type: application/json

{
  "orderNumber": "ORD-2026-001",
  "customerId": "CUST-001",
  "customerName": "John Smith",
  "status": "Pending",
  "orderItems": [
    {
      "productId": "<product-guid>",
      "quantity": 2,
      "unitPrice": 999.99
    }
  ]
}
```

---

## 📦 Stock Movements API

### Create Stock Movement (Auto Updates Inventory)
```http
POST /api/stockmovements
Authorization: Bearer <token>
Content-Type: application/json

{
  "productId": "<product-guid>",
  "fromWarehouseId": "<warehouse-guid>",  // Required for Out/Transfer
  "toWarehouseId": "<warehouse-guid>",    // Required for In/Transfer
  "quantity": 50,
  "type": "In",  // In, Out, Transfer, Adjustment
  "reference": "PO-2026-001",
  "notes": "Stock received from supplier"
}
```

**Movement Types:**
- **In**: Adds stock to `toWarehouse` (receiving)
- **Out**: Removes stock from `fromWarehouse` (shipping)
- **Transfer**: Moves stock from `fromWarehouse` to `toWarehouse`
- **Adjustment**: Manual inventory correction

---

## 🎯 Key Features Implemented

### ✅ JWT Authentication
- Access token (24 hours)
- Refresh token (7 days)
- Role-based authorization (Admin, Manager, Staff)
- Secure password requirements

### ✅ Pagination
- Page number & page size
- Total count & total pages
- Has previous/next page indicators

### ✅ Filtering
- Products: Search, category, price range
- Inventory: Product/warehouse filter, quantity range
- Orders: Order number, customer, status, date range, amount

### ✅ Sorting
- Dynamic sorting on any property
- Ascending/descending order
- Default sort by relevant field

### ✅ Automatic Stock Updates
- Stock movements automatically update inventory quantities
- Validates warehouse requirements per movement type
- Creates inventory record if doesn't exist

---

## 🛡️ Role Permissions

| Endpoint | Admin | Manager | Staff |
|----------|-------|---------|-------|
| GET endpoints | ✅ | ✅ | ✅ |
| POST Products | ✅ | ✅ | ❌ |
| PUT Products | ✅ | ✅ | ❌ |
| DELETE Products | ✅ | ❌ | ❌ |

---

## ⚙️ Technical Stack
- **.NET 8.0** - Latest LTS version
- **Entity Framework Core 8.0.4** - Azure SQL Database
- **MediatR 12.2.0** - CQRS pattern
- **Mapster 7.4.0** - Object mapping
- **FluentValidation 11.9.1** - Request validation
- **ASP.NET Core Identity** - User management
- **JWT Bearer Authentication** - Secure token-based auth

---

## 📝 Build Status
**0 Warnings | 0 Errors** ✅

---

## 🔗 Useful Links
- **Swagger Documentation:** http://localhost:5057/swagger
- **Azure SQL Database:** stockflowpro.database.windows.net

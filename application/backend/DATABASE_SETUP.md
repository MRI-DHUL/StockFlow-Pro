# Database Setup Guide - StockFlow Pro

This guide will help you set up the PostgreSQL database for StockFlow Pro.

---

## Prerequisites

- PostgreSQL 13+ installed locally OR a Supabase account
- .NET 8.0 SDK installed
- Entity Framework Core tools (already installed in this project)

---

## Option 1: Local PostgreSQL Setup

### Step 1: Install PostgreSQL

Download and install PostgreSQL from: https://www.postgresql.org/download/

### Step 2: Create Database

Open PostgreSQL command line (psql) or pgAdmin and run:

```sql
CREATE DATABASE stockflow_dev;
```

### Step 3: Update Connection String

Update the connection string in `src/StockFlow.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=stockflow_dev;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

**Important:** Replace `YOUR_PASSWORD` with your PostgreSQL password.

### Step 4: Apply Migrations

From the backend directory, run:

```bash
# Set PATH for dotnet-ef (one-time per session)
$env:PATH += ";C:\Users\YOUR_USERNAME\.dotnet\tools"

# Apply migrations
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

## Option 2: Supabase (Cloud PostgreSQL)

### Step 1: Create Supabase Project

1. Go to https://supabase.com
2. Sign up/Login
3. Create a new project
4. Wait for database provisioning (1-2 minutes)

### Step 2: Get Connection String

1. In Supabase dashboard, go to **Settings** → **Database**
2. Copy the **Connection String** (URI format)
3. It looks like: `postgresql://postgres:[YOUR-PASSWORD]@db.xxx.supabase.co:5432/postgres`

### Step 3: Update Connection String

Update `src/StockFlow.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
  }
}
```

**Or use the full URI format:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://postgres:[YOUR-PASSWORD]@db.xxx.supabase.co:5432/postgres?sslmode=require"
  }
}
```

### Step 4: Apply Migrations

```bash
# Set PATH for dotnet-ef (one-time per session)
$env:PATH += ";C:\Users\YOUR_USERNAME\.dotnet\tools"

# Apply migrations
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

## Verify Database Setup

### Method 1: Using the API

1. Run the API:
   ```bash
   cd src/StockFlow.API
   dotnet run
   ```

2. Check the console output - you should see:
   ```
   info: Microsoft.EntityFrameworkCore.Database.Command[20101]
         Executed DbCommand (Xms) [Parameters=[], CommandType='Text', CommandTimeout='30']
   ```

### Method 2: Check Tables Directly

#### Local PostgreSQL (pgAdmin or psql)

```sql
\c stockflow_dev
\dt
```

You should see tables:
- Products
- Warehouses
- Inventories
- Orders
- OrderItems
- Suppliers
- PurchaseOrders
- PurchaseOrderItems
- StockMovements
- __EFMigrationsHistory

#### Supabase

1. Go to **Table Editor** in Supabase dashboard
2. You should see all the tables listed above

---

## Common Issues and Solutions

### Issue 1: Connection Refused

**Symptom:** `Npgsql.NpgsqlException: Connection refused`

**Solution:**
- Ensure PostgreSQL service is running
- Check if port 5432 is open
- Verify firewall settings

### Issue 2: Authentication Failed

**Symptom:** `password authentication failed for user "postgres"`

**Solution:**
- Double-check your password in the connection string
- Ensure the PostgreSQL user exists
- For Supabase, make sure you copied the correct password from the dashboard

### Issue 3: SSL Certificate Error (Supabase)

**Symptom:** `SSL connection error`

**Solution:**
- Add `SSL Mode=Require` to connection string
- Or add `Trust Server Certificate=true` (development only)

### Issue 4: Migration Already Applied

**Symptom:** `The migration 'InitialCreate' has already been applied to the database`

**Solution:**
- This is normal if you've already run migrations
- To reset: `dotnet ef database drop --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API`
- Then apply again: `dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API`

### Issue 5: dotnet-ef Not Found

**Symptom:** `dotnet-ef is not recognized`

**Solution:**
```bash
# Add tools to PATH (PowerShell)
$env:PATH += ";C:\Users\YOUR_USERNAME\.dotnet\tools"

# Or permanently add to Windows PATH:
# 1. Search "Environment Variables" in Windows
# 2. Edit PATH variable
# 3. Add: C:\Users\YOUR_USERNAME\.dotnet\tools
```

---

## Useful EF Core Commands

### Create a New Migration
```bash
dotnet ef migrations add MigrationName --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### Apply Migrations
```bash
dotnet ef database update --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### Rollback to Specific Migration
```bash
dotnet ef database update MigrationName --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### Remove Last Migration (before applying)
```bash
dotnet ef migrations remove --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### Drop Database
```bash
dotnet ef database drop --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

### View Migration SQL
```bash
dotnet ef migrations script --project src/StockFlow.Infrastructure --startup-project src/StockFlow.API
```

---

## Database Schema Overview

### Core Tables

| Table | Description | Key Fields |
|-------|-------------|-----------|
| **Products** | Product catalog | Name, SKU, UnitPrice, Category |
| **Warehouses** | Warehouse locations | Name, Location, ContactInfo |
| **Inventories** | Stock levels per warehouse | ProductId, WarehouseId, Quantity, Threshold |
| **Orders** | Sales orders | OrderNumber, Status, TotalAmount |
| **OrderItems** | Order line items | OrderId, ProductId, Quantity, UnitPrice |
| **Suppliers** | Supplier directory | Name, Email, LeadTimeDays |
| **PurchaseOrders** | Purchase orders from suppliers | PONumber, SupplierId, Status |
| **PurchaseOrderItems** | PO line items | PurchaseOrderId, ProductId, Quantity |
| **StockMovements** | Stock movement history | ProductId, FromWarehouseId, ToWarehouseId, Type |

### Relationships

- **Product** ↔ **Inventory** (One-to-Many)
- **Warehouse** ↔ **Inventory** (One-to-Many)
- **Order** ↔ **OrderItem** (One-to-Many)
- **Product** ↔ **OrderItem** (One-to-Many)
- **Supplier** ↔ **PurchaseOrder** (One-to-Many)
- **PurchaseOrder** ↔ **PurchaseOrderItem** (One-to-Many)
- **Product** ↔ **StockMovement** (One-to-Many)
- **Warehouse** ↔ **StockMovement** (One-to-Many, bidirectional)

---

## Next Steps

1. ✅ Database setup complete
2. ⏭️ Seed initial data (optional)
3. ⏭️ Create repository interfaces
4. ⏭️ Implement CQRS handlers
5. ⏭️ Create API controllers
6. ⏭️ Add authentication
7. ⏭️ Implement business logic

---

## Seeding Sample Data (Optional)

To add sample data for testing, you can create a seed method in `ApplicationDbContext.cs` or use a migration. Example:

```csharp
// Add this method to ApplicationDbContext.cs
public static void SeedData(ApplicationDbContext context)
{
    // Seed Warehouses
    if (!context.Warehouses.Any())
    {
        context.Warehouses.AddRange(
            new Warehouse { Name = "Main Warehouse", Location = "123 Main St, City" },
            new Warehouse { Name = "Secondary Warehouse", Location = "456 Second Ave, City" }
        );
        context.SaveChanges();
    }

    // Seed Products
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "Widget A", SKU = "WDG-001", UnitPrice = 29.99m, Category = "Electronics" },
            new Product { Name = "Widget B", SKU = "WDG-002", UnitPrice = 49.99m, Category = "Electronics" },
            new Product { Name = "Gadget X", SKU = "GAD-001", UnitPrice = 99.99m, Category = "Hardware" }
        );
        context.SaveChanges();
    }
}
```

Call this from `Program.cs` after building the app:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ApplicationDbContext.SeedData(context);
}
```

---

**Database setup is now complete! 🎉**

You can now start building the application logic, controllers, and business rules.

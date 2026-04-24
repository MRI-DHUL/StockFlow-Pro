-- ============================================
-- StockFlow Pro - Sample Data Seeding Script
-- ============================================
-- Run this script to populate your database with sample data
-- Make sure your database exists and migrations are applied first

USE StockFlowProDB;
GO

-- ============================================
-- SEED WAREHOUSES
-- ============================================
SET IDENTITY_INSERT Warehouses ON;

INSERT INTO Warehouses (Id, Name, Location, Capacity, ManagerId, IsActive, CreatedAt)
VALUES
    (1, 'Main Warehouse', 'Mumbai, Maharashtra', 10000, 'WH-MGR-001', 1, GETUTCDATE()),
    (2, 'North Zone Warehouse', 'Delhi, NCR', 8000, 'WH-MGR-002', 1, GETUTCDATE()),
    (3, 'South Zone Warehouse', 'Bangalore, Karnataka', 7500, 'WH-MGR-003', 1, GETUTCDATE()),
    (4, 'West Zone Warehouse', 'Pune, Maharashtra', 6000, 'WH-MGR-004', 1, GETUTCDATE()),
    (5, 'East Zone Warehouse', 'Kolkata, West Bengal', 5000, 'WH-MGR-005', 1, GETUTCDATE());

SET IDENTITY_INSERT Warehouses OFF;
GO

-- ============================================
-- SEED SUPPLIERS
-- ============================================
SET IDENTITY_INSERT Suppliers ON;

INSERT INTO Suppliers (Id, Name, ContactPerson, Email, Phone, Address, IsActive, CreatedAt)
VALUES
    (1, 'TechPro Electronics', 'Rajesh Kumar', 'rajesh@techpro.com', '+91-9876543210', '123 Tech Park, Bangalore', 1, GETUTCDATE()),
    (2, 'Global Supplies Co', 'Priya Sharma', 'priya@globalsupplies.com', '+91-9876543211', '456 Supply Street, Mumbai', 1, GETUTCDATE()),
    (3, 'Premium Parts Ltd', 'Amit Patel', 'amit@premiumparts.com', '+91-9876543212', '789 Industrial Area, Delhi', 1, GETUTCDATE()),
    (4, 'Quality Hardware', 'Sneha Reddy', 'sneha@qualityhardware.com', '+91-9876543213', '321 Hardware Plaza, Hyderabad', 1, GETUTCDATE()),
    (5, 'Swift Distributors', 'Arjun Singh', 'arjun@swiftdist.com', '+91-9876543214', '654 Distribution Center, Pune', 1, GETUTCDATE());

SET IDENTITY_INSERT Suppliers OFF;
GO

-- ============================================
-- SEED PRODUCTS
-- ============================================
SET IDENTITY_INSERT Products ON;

INSERT INTO Products (Id, Name, SKU, Description, Category, UnitPrice, ReorderLevel, QuantityPerUnit, IsActive, CreatedAt)
VALUES
    -- Electronics
    (1, 'Wireless Mouse', 'ELEC-001', 'Ergonomic wireless mouse with USB receiver', 'Electronics', 599.00, 50, '1 piece', 1, GETUTCDATE()),
    (2, 'Mechanical Keyboard', 'ELEC-002', 'RGB mechanical keyboard with blue switches', 'Electronics', 2499.00, 30, '1 piece', 1, GETUTCDATE()),
    (3, 'USB-C Hub', 'ELEC-003', '7-in-1 USB-C hub with HDMI and USB 3.0', 'Electronics', 1299.00, 40, '1 piece', 1, GETUTCDATE()),
    (4, 'Webcam HD', 'ELEC-004', '1080p HD webcam with noise-cancelling mic', 'Electronics', 3499.00, 25, '1 piece', 1, GETUTCDATE()),
    (5, 'External SSD 500GB', 'ELEC-005', 'Portable external SSD 500GB USB 3.1', 'Electronics', 4999.00, 20, '1 piece', 1, GETUTCDATE()),
    
    -- Office Supplies
    (6, 'A4 Paper Ream', 'OFF-001', '500 sheets 80 GSM A4 paper', 'Office Supplies', 350.00, 100, '1 ream', 1, GETUTCDATE()),
    (7, 'Ballpoint Pen Box', 'OFF-002', 'Box of 50 blue ballpoint pens', 'Office Supplies', 250.00, 80, '1 box', 1, GETUTCDATE()),
    (8, 'Stapler Heavy Duty', 'OFF-003', 'Heavy duty stapler with staples', 'Office Supplies', 450.00, 40, '1 piece', 1, GETUTCDATE()),
    (9, 'File Folders Pack', 'OFF-004', 'Pack of 25 A4 file folders', 'Office Supplies', 300.00, 60, '1 pack', 1, GETUTCDATE()),
    (10, 'Whiteboard Markers', 'OFF-005', 'Set of 4 whiteboard markers', 'Office Supplies', 180.00, 70, '1 set', 1, GETUTCDATE()),
    
    -- Furniture
    (11, 'Office Chair Ergonomic', 'FURN-001', 'Ergonomic office chair with lumbar support', 'Furniture', 12999.00, 15, '1 piece', 1, GETUTCDATE()),
    (12, 'Standing Desk', 'FURN-002', 'Adjustable height standing desk', 'Furniture', 18999.00, 10, '1 piece', 1, GETUTCDATE()),
    (13, 'Filing Cabinet 4-Drawer', 'FURN-003', '4-drawer steel filing cabinet', 'Furniture', 8999.00, 12, '1 piece', 1, GETUTCDATE()),
    (14, 'Conference Table', 'FURN-004', '8-seater conference table', 'Furniture', 25999.00, 5, '1 piece', 1, GETUTCDATE()),
    (15, 'Bookshelf 5-Tier', 'FURN-005', 'Wooden 5-tier bookshelf', 'Furniture', 6999.00, 15, '1 piece', 1, GETUTCDATE()),
    
    -- Hardware
    (16, 'Screwdriver Set', 'HARD-001', '20-piece screwdriver set with case', 'Hardware', 899.00, 35, '1 set', 1, GETUTCDATE()),
    (17, 'Power Drill', 'HARD-002', 'Cordless power drill 18V', 'Hardware', 4599.00, 20, '1 piece', 1, GETUTCDATE()),
    (18, 'Measuring Tape', 'HARD-003', '25ft measuring tape', 'Hardware', 299.00, 50, '1 piece', 1, GETUTCDATE()),
    (19, 'Tool Box', 'HARD-004', 'Professional tool box with organizer', 'Hardware', 1599.00, 25, '1 piece', 1, GETUTCDATE()),
    (20, 'LED Work Light', 'HARD-005', 'Rechargeable LED work light', 'Hardware', 1299.00, 30, '1 piece', 1, GETUTCDATE()),
    
    -- Packaging
    (21, 'Cardboard Boxes Small', 'PACK-001', 'Pack of 50 small cardboard boxes', 'Packaging', 1200.00, 45, '1 pack', 1, GETUTCDATE()),
    (22, 'Bubble Wrap Roll', 'PACK-002', '100m bubble wrap roll', 'Packaging', 850.00, 40, '1 roll', 1, GETUTCDATE()),
    (23, 'Packing Tape', 'PACK-003', '6-pack packing tape with dispenser', 'Packaging', 420.00, 60, '1 pack', 1, GETUTCDATE()),
    (24, 'Shipping Labels', 'PACK-004', '500 self-adhesive shipping labels', 'Packaging', 650.00, 50, '1 pack', 1, GETUTCDATE()),
    (25, 'Stretch Film', 'PACK-005', 'Industrial stretch film 500mm', 'Packaging', 980.00, 35, '1 roll', 1, GETUTCDATE());

SET IDENTITY_INSERT Products OFF;
GO

-- ============================================
-- SEED INVENTORY
-- ============================================
SET IDENTITY_INSERT Inventory ON;

INSERT INTO Inventory (Id, ProductId, WarehouseId, Quantity, Threshold, LastUpdated)
VALUES
    -- Main Warehouse (Mumbai) - High volume, well-stocked
    (1, 1, 1, 150, 30, GETUTCDATE()),  -- Wireless Mouse: 150 units, alert at 30
    (2, 2, 1, 75, 15, GETUTCDATE()),   -- Mechanical Keyboard: 75 units, alert at 15
    (3, 3, 1, 120, 25, GETUTCDATE()),  -- USB-C Hub: 120 units, alert at 25
    (4, 6, 1, 500, 100, GETUTCDATE()), -- Notebook Set: 500 units, alert at 100
    (5, 7, 1, 300, 50, GETUTCDATE()),  -- Gel Pens Pack: 300 units, alert at 50
    (6, 11, 1, 45, 10, GETUTCDATE()),  -- Laptop Stand: 45 units, alert at 10
    (7, 16, 1, 80, 20, GETUTCDATE()),  -- Water Bottle: 80 units, alert at 20
    (8, 21, 1, 200, 40, GETUTCDATE()), -- Wall Clock: 200 units, alert at 40
    
    -- North Zone (Delhi) - Medium volume
    (9, 1, 2, 100, 30, GETUTCDATE()),  -- Wireless Mouse: 100 units
    (10, 4, 2, 60, 15, GETUTCDATE()),  -- HD Webcam: 60 units, alert at 15
    (11, 5, 2, 45, 10, GETUTCDATE()),  -- Portable SSD: 45 units, alert at 10
    (12, 8, 2, 90, 20, GETUTCDATE()),  -- Desk Organizer: 90 units
    (13, 12, 2, 25, 8, GETUTCDATE()),  -- Ergonomic Chair: 25 units, alert at 8
    (14, 17, 2, 50, 12, GETUTCDATE()),  -- Backpack: 50 units
    
    -- South Zone (Bangalore) - Tech hub distribution
    (15, 2, 3, 55, 15, GETUTCDATE()),  -- Mechanical Keyboard: 55 units
    (16, 3, 3, 95, 25, GETUTCDATE()),  -- USB-C Hub: 95 units
    (17, 9, 3, 140, 30, GETUTCDATE()),  -- File Holder: 140 units
    (18, 13, 3, 30, 8, GETUTCDATE()),  -- Monitor: 30 units
    (19, 18, 3, 110, 25, GETUTCDATE()),  -- Lunch Box: 110 units
    (20, 22, 3, 85, 20, GETUTCDATE()),  -- Plant Pot: 85 units
    
    -- West Zone (Pune) - Regional center
    (21, 6, 4, 400, 100, GETUTCDATE()),  -- Notebook Set: 400 units
    (22, 10, 4, 180, 40, GETUTCDATE()),  -- Whiteboard: 180 units
    (23, 14, 4, 15, 5, GETUTCDATE()),  -- Standing Desk: 15 units, alert at 5 (LOW STOCK)
    (24, 19, 4, 65, 15, GETUTCDATE()),  -- Coffee Mug: 65 units
    (25, 23, 4, 150, 35, GETUTCDATE()),  -- Calendar: 150 units
    
    -- East Zone (Hyderabad) - Growing distribution
    (26, 7, 5, 250, 50, GETUTCDATE()),  -- Gel Pens Pack: 250 units
    (27, 15, 5, 35, 10, GETUTCDATE()),  -- Desk Lamp: 35 units
    (28, 20, 5, 70, 15, GETUTCDATE()),  -- Phone Stand: 70 units
    (29, 24, 5, 125, 30, GETUTCDATE()),  -- Sticky Notes: 125 units
    (30, 25, 5, 90, 20, GETUTCDATE()),  -- Paper Clips Box: 90 units
    
    -- Low stock items for testing (Mumbai)
    (31, 4, 1, 8, 15, GETUTCDATE()),   -- HD Webcam: 8 units (BELOW THRESHOLD - LOW STOCK)
    (32, 5, 1, 12, 20, GETUTCDATE()),  -- Portable SSD: 12 units (BELOW THRESHOLD - LOW STOCK)
    (33, 14, 1, 3, 5, GETUTCDATE());   -- Standing Desk: 3 units (CRITICAL LOW STOCK)

SET IDENTITY_INSERT Inventory OFF;
GO

-- ============================================
-- SEED ORDERS
-- ============================================
SET IDENTITY_INSERT Orders ON;

INSERT INTO Orders (Id, OrderNumber, OrderDate, CustomerId, CustomerName, Status, TotalAmount, ShippingAddress, Notes, CreatedAt)
VALUES
    (1, 'ORD-2024-001', DATEADD(DAY, -5, GETUTCDATE()), 'CUST-001', 'Acme Corporation', 3, 15498.00, '123 Business Park, Mumbai, Maharashtra - 400001', 'Urgent delivery required', DATEADD(DAY, -5, GETUTCDATE())),
    (2, 'ORD-2024-002', DATEADD(DAY, -3, GETUTCDATE()), 'CUST-002', 'Tech Solutions Pvt Ltd', 2, 28497.00, '456 IT Hub, Bangalore, Karnataka - 560001', 'Standard delivery', DATEADD(DAY, -3, GETUTCDATE())),
    (3, 'ORD-2024-003', DATEADD(DAY, -2, GETUTCDATE()), 'CUST-003', 'Global Enterprises', 1, 9748.00, '789 Commercial Street, Delhi - 110001', 'Contact before delivery', DATEADD(DAY, -2, GETUTCDATE())),
    (4, 'ORD-2024-004', DATEADD(DAY, -1, GETUTCDATE()), 'CUST-004', 'Innovative Systems', 0, 12596.00, '321 Tech Valley, Pune, Maharashtra - 411001', 'Office hours delivery only', DATEADD(DAY, -1, GETUTCDATE())),
    (5, 'ORD-2024-005', GETUTCDATE(), 'CUST-005', 'Smart Solutions Ltd', 0, 18947.00, '654 Business District, Hyderabad - 500001', 'New customer', GETUTCDATE());

SET IDENTITY_INSERT Orders OFF;
GO

-- ============================================
-- SEED ORDER ITEMS
-- ============================================
SET IDENTITY_INSERT OrderItems ON;

INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, UnitPrice, TotalPrice)
VALUES
    -- Order 1
    (1, 1, 1, 10, 599.00, 5990.00),
    (2, 1, 2, 3, 2499.00, 7497.00),
    (3, 1, 6, 6, 350.00, 2100.00),
    
    -- Order 2
    (4, 2, 11, 2, 12999.00, 25998.00),
    (5, 2, 3, 5, 1299.00, 6495.00),
    
    -- Order 3
    (6, 3, 7, 8, 250.00, 2000.00),
    (7, 3, 8, 10, 450.00, 4500.00),
    (8, 3, 9, 12, 300.00, 3600.00),
    
    -- Order 4
    (9, 4, 16, 5, 899.00, 4495.00),
    (10, 4, 17, 2, 4599.00, 9198.00),
    
    -- Order 5
    (11, 5, 12, 1, 18999.00, 18999.00),
    (12, 5, 15, 1, 6999.00, 6999.00);

SET IDENTITY_INSERT OrderItems OFF;
GO

-- ============================================
-- SEED PURCHASE ORDERS
-- ============================================
SET IDENTITY_INSERT PurchaseOrders ON;

INSERT INTO PurchaseOrders (Id, PONumber, SupplierId, OrderDate, ExpectedDeliveryDate, Status, TotalAmount, Notes, CreatedAt)
VALUES
    (1, 'PO-2024-001', 1, DATEADD(DAY, -10, GETUTCDATE()), DATEADD(DAY, -3, GETUTCDATE()), 3, 45000.00, 'Regular stock replenishment', DATEADD(DAY, -10, GETUTCDATE())),
    (2, 'PO-2024-002', 2, DATEADD(DAY, -7, GETUTCDATE()), GETUTCDATE(), 2, 32500.00, 'Urgent order for high-demand items', DATEADD(DAY, -7, GETUTCDATE())),
    (3, 'PO-2024-003', 3, DATEADD(DAY, -4, GETUTCDATE()), DATEADD(DAY, 6, GETUTCDATE()), 1, 28000.00, 'Quarterly furniture order', DATEADD(DAY, -4, GETUTCDATE()));

SET IDENTITY_INSERT PurchaseOrders OFF;
GO

-- ============================================
-- SEED PURCHASE ORDER ITEMS
-- ============================================
SET IDENTITY_INSERT PurchaseOrderItems ON;

INSERT INTO PurchaseOrderItems (Id, PurchaseOrderId, ProductId, Quantity, UnitPrice, TotalPrice)
VALUES
    (1, 1, 1, 100, 450.00, 45000.00),
    (2, 2, 6, 200, 280.00, 56000.00),
    (3, 2, 7, 150, 200.00, 30000.00),
    (4, 3, 11, 20, 10000.00, 200000.00),
    (5, 3, 15, 30, 5500.00, 165000.00);

SET IDENTITY_INSERT PurchaseOrderItems OFF;
GO

-- ============================================
-- SEED STOCK MOVEMENTS
-- ============================================
SET IDENTITY_INSERT StockMovements ON;

INSERT INTO StockMovements (Id, ProductId, FromWarehouseId, ToWarehouseId, Quantity, MovementType, MovementDate, Reason, UserId, CreatedAt)
VALUES
    (1, 1, 1, 2, 50, 0, DATEADD(DAY, -6, GETUTCDATE()), 'Stock transfer to meet demand', 'admin@stockflowpro.com', DATEADD(DAY, -6, GETUTCDATE())),
    (2, 2, 1, 3, 25, 0, DATEADD(DAY, -4, GETUTCDATE()), 'Warehouse rebalancing', 'admin@stockflowpro.com', DATEADD(DAY, -4, GETUTCDATE())),
    (3, 6, 1, 4, 100, 0, DATEADD(DAY, -2, GETUTCDATE()), 'Regional distribution', 'admin@stockflowpro.com', DATEADD(DAY, -2, GETUTCDATE())),
    (4, 7, 1, 5, 80, 0, DATEADD(DAY, -1, GETUTCDATE()), 'Quarterly redistribution', 'admin@stockflowpro.com', DATEADD(DAY, -1, GETUTCDATE()));

SET IDENTITY_INSERT StockMovements OFF;
GO

-- ============================================
-- VERIFY DATA
-- ============================================
PRINT '=== Data Seeding Complete ===';
PRINT '';
PRINT 'Warehouses: ' + CAST((SELECT COUNT(*) FROM Warehouses WHERE Id BETWEEN 1 AND 5) AS VARCHAR);
PRINT 'Suppliers: ' + CAST((SELECT COUNT(*) FROM Suppliers WHERE Id BETWEEN 1 AND 5) AS VARCHAR);
PRINT 'Products: ' + CAST((SELECT COUNT(*) FROM Products WHERE Id BETWEEN 1 AND 25) AS VARCHAR);
PRINT 'Inventory Records: ' + CAST((SELECT COUNT(*) FROM Inventory WHERE Id BETWEEN 1 AND 33) AS VARCHAR);
PRINT 'Orders: ' + CAST((SELECT COUNT(*) FROM Orders WHERE Id BETWEEN 1 AND 5) AS VARCHAR);
PRINT 'Order Items: ' + CAST((SELECT COUNT(*) FROM OrderItems WHERE Id BETWEEN 1 AND 12) AS VARCHAR);
PRINT 'Purchase Orders: ' + CAST((SELECT COUNT(*) FROM PurchaseOrders WHERE Id BETWEEN 1 AND 3) AS VARCHAR);
PRINT 'Purchase Order Items: ' + CAST((SELECT COUNT(*) FROM PurchaseOrderItems WHERE Id BETWEEN 1 AND 5) AS VARCHAR);
PRINT 'Stock Movements: ' + CAST((SELECT COUNT(*) FROM StockMovements WHERE Id BETWEEN 1 AND 4) AS VARCHAR);
PRINT '';
PRINT 'Database seeded successfully!';
GO

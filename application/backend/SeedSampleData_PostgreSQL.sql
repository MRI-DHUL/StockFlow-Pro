-- ============================================
-- StockFlow Pro - Sample Data Seeding Script (PostgreSQL)
-- ============================================

-- Seed Warehouses
INSERT INTO "Warehouses" ("Id", "Name", "Location", "Capacity", "ContactInfo", "Email", "Phone", "IsActive", "CreatedAt")
VALUES
    (1, 'Main Warehouse', 'Mumbai, Maharashtra', 10000, 'Manager: Rajesh Kumar', 'wh-mumbai@stockflowpro.com', '+91-9876543210', true, NOW()),
    (2, 'North Zone Warehouse', 'Delhi, NCR', 8000, 'Manager: Priya Sharma', 'wh-delhi@stockflowpro.com', '+91-9876543211', true, NOW()),
    (3, 'South Zone Warehouse', 'Bangalore, Karnataka', 7500, 'Manager: Amit Patel', 'wh-bangalore@stockflowpro.com', '+91-9876543212', true, NOW()),
    (4, 'West Zone Warehouse', 'Pune, Maharashtra', 6000, 'Manager: Sneha Reddy', 'wh-pune@stockflowpro.com', '+91-9876543213', true, NOW()),
    (5, 'East Zone Warehouse', 'Kolkata, West Bengal', 5000, 'Manager: Arjun Singh', 'wh-kolkata@stockflowpro.com', '+91-9876543214', true, NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Seed Suppliers  
INSERT INTO "Suppliers" ("Id", "Name", "ContactPerson", "Email", "Phone", "Address", "IsActive", "CreatedAt")
VALUES
    (1, 'TechPro Electronics', 'Rajesh Kumar', 'rajesh@techpro.com', '+91-9876543210', '123 Tech Park, Bangalore', true, NOW()),
    (2, 'Global Supplies Co', 'Priya Sharma', 'priya@globalsupplies.com', '+91-9876543211', '456 Supply Street, Mumbai', true, NOW()),
    (3, 'Premium Parts Ltd', 'Amit Patel', 'amit@premiumparts.com', '+91-9876543212', '789 Industrial Area, Delhi', true, NOW()),
    (4, 'Quality Hardware', 'Sneha Reddy', 'sneha@qualityhardware.com', '+91-9876543213', '321 Hardware Plaza, Hyderabad', true, NOW()),
    (5, 'Swift Distributors', 'Arjun Singh', 'arjun@swiftdist.com', '+91-9876543214', '654 Distribution Center, Pune', true, NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Seed Products
INSERT INTO "Products" ("Id", "Name", "SKU", "Description", "Category", "UnitPrice", "ReorderLevel", "QuantityPerUnit", "IsActive", "CreatedAt")
VALUES
    (1, 'Wireless Mouse', 'ELEC-001', 'Ergonomic wireless mouse with USB receiver', 'Electronics', 599.00, 50, '1 piece', true, NOW()),
    (2, 'Mechanical Keyboard', 'ELEC-002', 'RGB mechanical keyboard with blue switches', 'Electronics', 2499.00, 30, '1 piece', true, NOW()),
    (3, 'USB-C Hub', 'ELEC-003', '7-in-1 USB-C hub with HDMI and USB 3.0', 'Electronics', 1299.00, 40, '1 piece', true, NOW()),
    (4, 'Webcam HD', 'ELEC-004', '1080p HD webcam with noise-cancelling mic', 'Electronics', 3499.00, 25, '1 piece', true, NOW()),
    (5, 'External SSD 500GB', 'ELEC-005', 'Portable external SSD 500GB USB 3.1', 'Electronics', 4999.00, 20, '1 piece', true, NOW()),
    (6, 'A4 Paper Ream', 'OFF-001', '500 sheets 80 GSM A4 paper', 'Office Supplies', 350.00, 100, '1 ream', true, NOW()),
    (7, 'Ballpoint Pen Box', 'OFF-002', 'Box of 50 blue ballpoint pens', 'Office Supplies', 250.00, 80, '1 box', true, NOW()),
    (8, 'Stapler Heavy Duty', 'OFF-003', 'Heavy duty stapler with staples', 'Office Supplies', 450.00, 40, '1 piece', true, NOW()),
    (9, 'File Folders Pack', 'OFF-004', 'Pack of 25 A4 file folders', 'Office Supplies', 300.00, 60, '1 pack', true, NOW()),
    (10, 'Whiteboard Markers', 'OFF-005', 'Set of 4 whiteboard markers', 'Office Supplies', 180.00, 70, '1 set', true, NOW()),
    (11, 'Office Chair Ergonomic', 'FURN-001', 'Ergonomic office chair with lumbar support', 'Furniture', 12999.00, 15, '1 piece', true, NOW()),
    (12, 'Standing Desk', 'FURN-002', 'Adjustable height standing desk', 'Furniture', 18999.00, 10, '1 piece', true, NOW()),
    (13, 'Filing Cabinet 4-Drawer', 'FURN-003', '4-drawer steel filing cabinet', 'Furniture', 8999.00, 12, '1 piece', true, NOW()),
    (14, 'Conference Table', 'FURN-004', '8-seater conference table', 'Furniture', 25999.00, 5, '1 piece', true, NOW()),
    (15, 'Bookshelf 5-Tier', 'FURN-005', 'Wooden 5-tier bookshelf', 'Furniture', 6999.00, 15, '1 piece', true, NOW()),
    (16, 'Screwdriver Set', 'HARD-001', '20-piece screwdriver set with case', 'Hardware', 899.00, 35, '1 set', true, NOW()),
    (17, 'Power Drill', 'HARD-002', 'Cordless power drill 18V', 'Hardware', 4599.00, 20, '1 piece', true, NOW()),
    (18, 'Measuring Tape', 'HARD-003', '25ft measuring tape', 'Hardware', 299.00, 50, '1 piece', true, NOW()),
    (19, 'Tool Box', 'HARD-004', 'Professional tool box with organizer', 'Hardware', 1599.00, 25, '1 piece', true, NOW()),
    (20, 'LED Work Light', 'HARD-005', 'Rechargeable LED work light', 'Hardware', 1299.00, 30, '1 piece', true, NOW()),
    (21, 'Cardboard Boxes Small', 'PACK-001', 'Pack of 50 small cardboard boxes', 'Packaging', 1200.00, 45, '1 pack', true, NOW()),
    (22, 'Bubble Wrap Roll', 'PACK-002', '100m bubble wrap roll', 'Packaging', 850.00, 40, '1 roll', true, NOW()),
    (23, 'Packing Tape', 'PACK-003', '6-pack packing tape with dispenser', 'Packaging', 420.00, 60, '1 pack', true, NOW()),
    (24, 'Shipping Labels', 'PACK-004', '500 self-adhesive shipping labels', 'Packaging', 650.00, 50, '1 pack', true, NOW()),
    (25, 'Stretch Film', 'PACK-005', 'Industrial stretch film 500mm', 'Packaging', 980.00, 35, '1 roll', true, NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Seed Inventory
INSERT INTO "Inventory" ("Id", "ProductId", "WarehouseId", "Quantity", "Threshold", "LastUpdated")
VALUES
    -- Mumbai Warehouse (High volume, well-stocked)
    (1, 1, 1, 150, 30, NOW()),  -- Wireless Mouse: 150 units, alert at 30
    (2, 2, 1, 75, 15, NOW()),   -- Mechanical Keyboard: 75 units, alert at 15
    (3, 3, 1, 120, 25, NOW()),  -- USB-C Hub: 120 units, alert at 25
    (4, 6, 1, 500, 100, NOW()), -- Notebook Set: 500 units, alert at 100
    (5, 7, 1, 300, 50, NOW()),  -- Gel Pens Pack: 300 units, alert at 50
    (6, 11, 1, 45, 10, NOW()),  -- Laptop Stand: 45 units, alert at 10
    (7, 16, 1, 80, 20, NOW()),  -- Water Bottle: 80 units, alert at 20
    (8, 21, 1, 200, 40, NOW()), -- Wall Clock: 200 units, alert at 40
    
    -- Delhi Warehouse (Medium volume)
    (9, 1, 2, 100, 30, NOW()),  -- Wireless Mouse: 100 units
    (10, 4, 2, 60, 15, NOW()),  -- HD Webcam: 60 units, alert at 15
    (11, 5, 2, 45, 10, NOW()),  -- Portable SSD: 45 units, alert at 10
    (12, 8, 2, 90, 20, NOW()),  -- Desk Organizer: 90 units
    (13, 12, 2, 25, 8, NOW()),  -- Ergonomic Chair: 25 units, alert at 8
    (14, 17, 2, 50, 12, NOW()),  -- Backpack: 50 units
    
    -- Bangalore Warehouse (Tech hub distribution)
    (15, 2, 3, 55, 15, NOW()),  -- Mechanical Keyboard: 55 units
    (16, 3, 3, 95, 25, NOW()),  -- USB-C Hub: 95 units
    (17, 9, 3, 140, 30, NOW()),  -- File Holder: 140 units
    (18, 13, 3, 30, 8, NOW()),  -- Monitor: 30 units
    (19, 18, 3, 110, 25, NOW()),  -- Lunch Box: 110 units
    (20, 22, 3, 85, 20, NOW()),  -- Plant Pot: 85 units
    
    -- Pune Warehouse (Regional center)
    (21, 6, 4, 400, 100, NOW()),  -- Notebook Set: 400 units
    (22, 10, 4, 180, 40, NOW()),  -- Whiteboard: 180 units
    (23, 14, 4, 15, 5, NOW()),  -- Standing Desk: 15 units, alert at 5 (LOW STOCK)
    (24, 19, 4, 65, 15, NOW()),  -- Coffee Mug: 65 units
    (25, 23, 4, 150, 35, NOW()),  -- Calendar: 150 units
    
    -- Hyderabad Warehouse (Growing distribution)
    (26, 7, 5, 250, 50, NOW()),  -- Gel Pens Pack: 250 units
    (27, 15, 5, 35, 10, NOW()),  -- Desk Lamp: 35 units
    (28, 20, 5, 70, 15, NOW()),  -- Phone Stand: 70 units
    (29, 24, 5, 125, 30, NOW()),  -- Sticky Notes: 125 units
    (30, 25, 5, 90, 20, NOW()),  -- Paper Clips Box: 90 units
    
    -- Low Stock Items for Testing (Mumbai)
    (31, 4, 1, 8, 15, NOW()),   -- HD Webcam: 8 units (BELOW THRESHOLD - LOW STOCK)
    (32, 5, 1, 12, 20, NOW()),  -- Portable SSD: 12 units (BELOW THRESHOLD - LOW STOCK)
    (33, 14, 1, 3, 5, NOW())    -- Standing Desk: 3 units (CRITICAL LOW STOCK)
ON CONFLICT ("Id") DO NOTHING;

-- Seed Orders
INSERT INTO "Orders" ("Id", "OrderNumber", "OrderDate", "CustomerId", "CustomerName", "Status", "TotalAmount", "ShippingAddress", "Notes", "CreatedAt")
VALUES
    (1, 'ORD-2024-001', NOW() - INTERVAL '5 days', 'CUST-001', 'Acme Corporation', 3, 15498.00, '123 Business Park, Mumbai, Maharashtra - 400001', 'Urgent delivery required', NOW() - INTERVAL '5 days'),
    (2, 'ORD-2024-002', NOW() - INTERVAL '3 days', 'CUST-002', 'Tech Solutions Pvt Ltd', 2, 28497.00, '456 IT Hub, Bangalore, Karnataka - 560001', 'Standard delivery', NOW() - INTERVAL '3 days'),
    (3, 'ORD-2024-003', NOW() - INTERVAL '2 days', 'CUST-003', 'Global Enterprises', 1, 9748.00, '789 Commercial Street, Delhi - 110001', 'Contact before delivery', NOW() - INTERVAL '2 days'),
    (4, 'ORD-2024-004', NOW() - INTERVAL '1 day', 'CUST-004', 'Innovative Systems', 0, 12596.00, '321 Tech Valley, Pune, Maharashtra - 411001', 'Office hours delivery only', NOW() - INTERVAL '1 day'),
    (5, 'ORD-2024-005', NOW(), 'CUST-005', 'Smart Solutions Ltd', 0, 18947.00, '654 Business District, Hyderabad - 500001', 'New customer', NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Seed Order Items
INSERT INTO "OrderItems" ("Id", "OrderId", "ProductId", "Quantity", "UnitPrice", "TotalPrice")
VALUES
    (1, 1, 1, 10, 599.00, 5990.00), (2, 1, 2, 3, 2499.00, 7497.00), (3, 1, 6, 6, 350.00, 2100.00),
    (4, 2, 11, 2, 12999.00, 25998.00), (5, 2, 3, 5, 1299.00, 6495.00), (6, 3, 7, 8, 250.00, 2000.00),
    (7, 3, 8, 10, 450.00, 4500.00), (8, 3, 9, 12, 300.00, 3600.00), (9, 4, 16, 5, 899.00, 4495.00),
    (10, 4, 17, 2, 4599.00, 9198.00), (11, 5, 12, 1, 18999.00, 18999.00), (12, 5, 15, 1, 6999.00, 6999.00)
ON CONFLICT ("Id") DO NOTHING;

-- Seed Purchase Orders
INSERT INTO "PurchaseOrders" ("Id", "PONumber", "SupplierId", "OrderDate", "ExpectedDeliveryDate", "Status", "TotalAmount", "Notes", "CreatedAt")
VALUES
    (1, 'PO-2024-001', 1, NOW() - INTERVAL '10 days', NOW() - INTERVAL '3 days', 3, 45000.00, 'Regular stock replenishment', NOW() - INTERVAL '10 days'),
    (2, 'PO-2024-002', 2, NOW() - INTERVAL '7 days', NOW(), 2, 32500.00, 'Urgent order for high-demand items', NOW() - INTERVAL '7 days'),
    (3, 'PO-2024-003', 3, NOW() - INTERVAL '4 days', NOW() + INTERVAL '6 days', 1, 28000.00, 'Quarterly furniture order', NOW() - INTERVAL '4 days')
ON CONFLICT ("Id") DO NOTHING;

-- Seed Purchase Order Items
INSERT INTO "PurchaseOrderItems" ("Id", "PurchaseOrderId", "ProductId", "Quantity", "UnitPrice", "TotalPrice")
VALUES
    (1, 1, 1, 100, 450.00, 45000.00), (2, 2, 6, 200, 280.00, 56000.00), (3, 2, 7, 150, 200.00, 30000.00),
    (4, 3, 11, 20, 10000.00, 200000.00), (5, 3, 15, 30, 5500.00, 165000.00)
ON CONFLICT ("Id") DO NOTHING;

-- Seed Stock Movements
INSERT INTO "StockMovements" ("Id", "ProductId", "FromWarehouseId", "ToWarehouseId", "Quantity", "MovementType", "MovementDate", "Reason", "UserId", "CreatedAt")
VALUES
    (1, 1, 1, 2, 50, 0, NOW() - INTERVAL '6 days', 'Stock transfer to meet demand', 'admin@stockflowpro.com', NOW() - INTERVAL '6 days'),
    (2, 2, 1, 3, 25, 0, NOW() - INTERVAL '4 days', 'Warehouse rebalancing', 'admin@stockflowpro.com', NOW() - INTERVAL '4 days'),
    (3, 6, 1, 4, 100, 0, NOW() - INTERVAL '2 days', 'Regional distribution', 'admin@stockflowpro.com', NOW() - INTERVAL '2 days'),
    (4, 7, 1, 5, 80, 0, NOW() - INTERVAL '1 day', 'Quarterly redistribution', 'admin@stockflowpro.com', NOW() - INTERVAL '1 day')
ON CONFLICT ("Id") DO NOTHING;

-- Verify Data
SELECT 'Database seeded successfully!' AS status;
SELECT COUNT(*) AS warehouses FROM "Warehouses" WHERE "Id" BETWEEN 1 AND 5;
SELECT COUNT(*) AS suppliers FROM "Suppliers" WHERE "Id" BETWEEN 1 AND 5;
SELECT COUNT(*) AS products FROM "Products" WHERE "Id" BETWEEN 1 AND 25;
SELECT COUNT(*) AS inventory_records FROM "Inventory" WHERE "Id" BETWEEN 1 AND 33;
SELECT COUNT(*) AS orders FROM "Orders" WHERE "Id" BETWEEN 1 AND 5;

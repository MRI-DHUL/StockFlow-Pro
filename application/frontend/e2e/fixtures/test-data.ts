/**
 * Test data fixtures for E2E tests
 */

export const TEST_USERS = {
  admin: {
    email: 'admin@stockflowpro.com',
    password: 'Admin@123',
    role: 'Admin'
  },
  testUser: {
    email: 'test@example.com',
    password: 'Test@123',
    firstName: 'Test',
    lastName: 'User',
    role: 'Staff'
  }
};

export const TEST_PRODUCT = {
  name: 'Test Product',
  sku: 'TEST-001',
  description: 'This is a test product',
  category: 'Electronics',
  unitPrice: 99.99,
  // Inventory fields for creation
  warehouse: 'Main Warehouse',
  initialQuantity: 50,
  threshold: 10
};

export const TEST_PRODUCT_UPDATE = {
  name: 'Updated Test Product',
  description: 'This is an updated test product',
  category: 'Electronics',
  unitPrice: 149.99,
  // Inventory update fields
  currentQuantity: 100,
  threshold: 15
};

export const TEST_WAREHOUSE = {
  name: 'Test Warehouse',
  location: 'Test Location',
  capacity: 1000,
  managerId: 'test-manager-id'
};

export const TEST_SUPPLIER = {
  name: 'Test Supplier',
  contactPerson: 'John Doe',
  email: 'supplier@example.com',
  phone: '1234567890',
  address: '123 Test Street'
};

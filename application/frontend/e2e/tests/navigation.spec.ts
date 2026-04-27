import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../fixtures/test-data';

test.describe('Navigation', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 30000 });
  });

  test('should navigate to all main pages', async ({ page }) => {
    // Dashboard
    await page.locator('a[routerlink="/dashboard"]').click();
    await expect(page).toHaveURL(/.*dashboard/);
    
    // Products
    await page.locator('a[routerlink="/products"]').click();
    await expect(page).toHaveURL(/.*products/);
    
    // Inventory
    await page.locator('a[routerlink="/inventory"]').click();
    await expect(page).toHaveURL(/.*inventory/);
    
    // Warehouses
    await page.locator('a[routerlink="/warehouses"]').click();
    await expect(page).toHaveURL(/.*warehouses/);
    
    // Suppliers
    await page.locator('a[routerlink="/suppliers"]').click();
    await expect(page).toHaveURL(/.*suppliers/);
    
    // Orders
    await page.locator('a[routerlink="/orders"]').click();
    await expect(page).toHaveURL(/.*orders/);
    
    // Purchase Orders
    await page.locator('a[routerlink="/purchase-orders"]').click();
    await expect(page).toHaveURL(/.*purchase-orders/);
    
    // Stock Movements
    await page.locator('a[routerlink="/stock-movements"]').click();
    await expect(page).toHaveURL(/.*stock-movements/);
    
    // Audit Logs
    await page.locator('a[routerlink="/audit-logs"]').click();
    await expect(page).toHaveURL(/.*audit-logs/);
  });

  test('should highlight active navigation item', async ({ page }) => {
    // Navigate to products
    await page.locator('a[routerlink="/products"]').click();
    
    // Check if products link has active class
    const productsLink = page.locator('a[routerlink="/products"]');
    await expect(productsLink).toHaveClass(/active-link|active/);
  });

  test('should display sidebar navigation', async ({ page }) => {
    // Check if sidebar is visible
    await expect(page.locator('mat-sidenav')).toBeVisible();
    
    // Check for navigation sections
    await expect(page.locator('text=MAIN')).toBeVisible();
    await expect(page.locator('text=INVENTORY')).toBeVisible();
  });
});

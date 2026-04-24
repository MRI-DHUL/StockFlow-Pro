import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../fixtures/test-data';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 10000 });
  });

  test('should display dashboard with statistics', async ({ page }) => {
    // Check if we're on dashboard
    await expect(page).toHaveURL(/.*dashboard/);
    
    // Check for main heading
    await expect(page.locator('h1')).toContainText('Welcome back');
    
    // Check for stat cards
    const statCards = page.locator('mat-card.stat-card');
    await expect(statCards).toHaveCount(4);
    
    // Verify stat card labels
    await expect(page.locator('text=Total Products')).toBeVisible();
    await expect(page.locator('text=Total Warehouses')).toBeVisible();
    await expect(page.locator('text=Low Stock Alerts')).toBeVisible();
    await expect(page.locator('text=Pending Orders')).toBeVisible();
  });

  test('should display quick actions', async ({ page }) => {
    // Check for quick actions section
    await expect(page.locator('text=Quick Actions')).toBeVisible();
    
    // Check for action buttons
    await expect(page.locator('button:has-text("Add New Product")')).toBeVisible();
    await expect(page.locator('button:has-text("Update Inventory")')).toBeVisible();
    await expect(page.locator('button:has-text("Create Order")')).toBeVisible();
    await expect(page.locator('button:has-text("Transfer Stock")')).toBeVisible();
  });

  test('should navigate to products page from quick action', async ({ page }) => {
    // Click on "Add New Product" button
    await page.locator('button:has-text("Add New Product")').first().click();
    
    // Verify navigation to products page
    await expect(page).toHaveURL(/.*products/);
  });

  test('should display recent notifications section', async ({ page }) => {
    // Check for notifications section
    await expect(page.locator('text=Recent Activity')).toBeVisible();
  });

  test('should display navigation menu', async ({ page }) => {
    // Check for navigation items
    await expect(page.locator('a[routerlink="/dashboard"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/products"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/inventory"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/warehouses"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/suppliers"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/orders"]')).toBeVisible();
  });

  test('should display StockFlow Pro branding', async ({ page }) => {
    // Check for logo/branding
    await expect(page.locator('text=StockFlow')).toBeVisible();
  });
});

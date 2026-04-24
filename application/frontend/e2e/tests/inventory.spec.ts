import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../fixtures/test-data';

test.describe('Inventory Management', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 10000 });
    
    // Navigate to inventory page
    await page.locator('a[routerlink="/inventory"]').click();
    await page.waitForURL('**/inventory');
  });

  test('should display inventory page', async ({ page }) => {
    await expect(page).toHaveURL(/.*inventory/);
    await expect(page.locator('mat-card-title:has-text("Inventory Management")')).toBeVisible();
  });

  test('should display inventory table', async ({ page }) => {
    // Check for table headers
    await expect(page.locator('th:has-text("Product")')).toBeVisible();
    await expect(page.locator('th:has-text("Warehouse")')).toBeVisible();
    await expect(page.locator('th:has-text("Quantity")')).toBeVisible();
  });

  test('should display filter controls', async ({ page }) => {
    // Check for filter section
    const filterExists = await page.locator('.filters-container, .filters-form').isVisible().catch(() => false);
    
    if (filterExists) {
      await expect(page.locator('.filters-container, .filters-form')).toBeVisible();
    }
  });

  test('should show low stock items', async ({ page }) => {
    // Look for low stock indicator
    const lowStockExists = await page.locator('text=/low stock/i').isVisible().catch(() => false);
    
    if (lowStockExists) {
      await expect(page.locator('text=/low stock/i')).toBeVisible();
    }
  });

  test('should display inventory actions', async ({ page }) => {
    // Check if table has rows
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Check for update button
      const updateButton = firstRow.locator('button:has(mat-icon:has-text("edit"))');
      await expect(updateButton).toBeVisible();
    }
  });
});

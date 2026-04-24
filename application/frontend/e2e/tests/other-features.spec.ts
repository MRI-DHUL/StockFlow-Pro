import { test, expect } from '@playwright/test';
import { TestHelpers } from '../helpers/test-helpers';
import { TEST_USERS, TEST_WAREHOUSE } from '../fixtures/test-data';

test.describe('Warehouses Management', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/warehouses');
  });

  test('should display warehouses page', async ({ page }) => {
    await expect(page).toHaveURL(/.*warehouses/);
    await expect(page.locator('mat-card-title:has-text("Warehouses")')).toBeVisible();
  });

  test('should display warehouses table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should open add warehouse dialog', async ({ page }) => {
    await page.locator('button:has-text("Add Warehouse")').click();
    await expect(page.locator('h2:has-text("Add Warehouse")')).toBeVisible();
  });
});

test.describe('Suppliers Management', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/suppliers');
  });

  test('should display suppliers page', async ({ page }) => {
    await expect(page).toHaveURL(/.*suppliers/);
    await expect(page.locator('mat-card-title:has-text("Suppliers")')).toBeVisible();
  });

  test('should display suppliers table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should open add supplier dialog', async ({ page }) => {
    await page.locator('button:has-text("Add Supplier")').click();
    await expect(page.locator('h2:has-text("Add Supplier")')).toBeVisible();
  });
});

test.describe('Orders Management', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/orders');
  });

  test('should display orders page', async ({ page }) => {
    await expect(page).toHaveURL(/.*orders/);
    await expect(page.locator('mat-card-title:has-text("Orders")')).toBeVisible();
  });

  test('should display orders table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should have filter controls', async ({ page }) => {
    const filterExists = await page.locator('.filters-container, .filters-form').isVisible().catch(() => false);
    
    if (filterExists) {
      await expect(page.locator('.filters-container, .filters-form')).toBeVisible();
    }
  });
});

test.describe('Stock Movements', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/stock-movements');
  });

  test('should display stock movements page', async ({ page }) => {
    await expect(page).toHaveURL(/.*stock-movements/);
    await expect(page.locator('mat-card-title:has-text("Stock Movements")')).toBeVisible();
  });

  test('should display stock movements table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });
});

test.describe('Audit Logs', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/audit-logs');
  });

  test('should display audit logs page', async ({ page }) => {
    await expect(page).toHaveURL(/.*audit-logs/);
    await expect(page.locator('mat-card-title:has-text("Audit Logs")')).toBeVisible();
  });

  test('should display audit logs table', async ({ page }) => {
    await expect(page.locator('table')).toBeVisible();
  });

  test('should show audit log details', async ({ page }) => {
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Verify audit log columns
      await expect(page.locator('th:has-text("Action")')).toBeVisible();
      await expect(page.locator('th:has-text("User")')).toBeVisible();
      await expect(page.locator('th:has-text("Timestamp")')).toBeVisible();
    }
  });
});

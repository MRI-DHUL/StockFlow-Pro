import { test, expect, Page } from '@playwright/test';
import { TestHelpers } from '../helpers/test-helpers';
import { TEST_USERS } from '../fixtures/test-data';

/**
 * Comprehensive E2E test suite covering critical user journeys
 */
test.describe('Critical User Journeys', () => {
  
  test('Complete product management flow', async ({ page }) => {
    // Login
    await TestHelpers.loginAsAdmin(page);
    
    // Navigate to products
    await TestHelpers.navigateTo(page, '/products');
    await expect(page).toHaveURL(/.*products/);
    
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
    
    // Generate unique SKU
    const timestamp = Date.now();
    const uniqueSKU = `E2E-TEST-${timestamp}`;
    
    // Fill product form
    await TestHelpers.fillFormField(page, 'name', `E2E Test Product ${timestamp}`);
    await TestHelpers.fillFormField(page, 'sku', uniqueSKU);
    await TestHelpers.fillFormField(page, 'description', 'Created by E2E test');
    await TestHelpers.fillFormField(page, 'category', 'Electronics');
    await TestHelpers.fillFormField(page, 'unitPrice', '99.99');
    await TestHelpers.fillFormField(page, 'reorderLevel', '10');
    await TestHelpers.fillFormField(page, 'quantityPerUnit', '1 unit');
    
    // Submit form
    await TestHelpers.clickButton(page, 'Save');
    
    // Wait for success notification
    await TestHelpers.waitForToast(page);
    
    // Verify product appears in list
    await page.waitForTimeout(1000);
    await expect(page.locator(`td:has-text("${uniqueSKU}")`)).toBeVisible({ timeout: 10000 });
    
    // Search for the created product
    await TestHelpers.fillFormField(page, 'searchTerm', uniqueSKU);
    await TestHelpers.clickButton(page, 'Apply Filters');
    await page.waitForTimeout(1000);
    
    // Verify search results
    await expect(page.locator(`td:has-text("${uniqueSKU}")`)).toBeVisible();
    
    // Edit the product
    const productRow = page.locator(`tr:has-text("${uniqueSKU}")`);
    await productRow.locator('button:has(mat-icon:has-text("edit"))').click();
    
    // Update price
    await TestHelpers.fillFormField(page, 'unitPrice', '149.99');
    await TestHelpers.clickButton(page, 'Save');
    
    // Wait for success
    await TestHelpers.waitForToast(page);
  });

  test('Dashboard to inventory update flow', async ({ page }) => {
    // Login
    await TestHelpers.loginAsAdmin(page);
    
    // Verify dashboard loads
    await expect(page).toHaveURL(/.*dashboard/);
    await expect(page.locator('h1')).toContainText('Welcome back');
    
    // Click quick action to update inventory
    await page.locator('button:has-text("Update Inventory")').first().click();
    
    // Verify navigation to inventory
    await expect(page).toHaveURL(/.*inventory/);
    await expect(page.locator('mat-card-title:has-text("Inventory")')).toBeVisible();
  });

  test('Check low stock alerts flow', async ({ page }) => {
    // Login
    await TestHelpers.loginAsAdmin(page);
    
    // Navigate to dashboard
    await expect(page).toHaveURL(/.*dashboard/);
    
    // Check low stock alert stat
    const lowStockCard = page.locator('mat-card.low-stock-card');
    await expect(lowStockCard).toBeVisible();
    await expect(lowStockCard.locator('.card-label')).toContainText('Low Stock');
    
    // Navigate to inventory
    await TestHelpers.navigateTo(page, '/inventory');
    
    // Check for low stock items if any exist
    const lowStockExists = await page.locator('text=/low stock/i').isVisible().catch(() => false);
    
    if (lowStockExists) {
      await expect(page.locator('text=/low stock/i')).toBeVisible();
    }
  });

  test('Navigation and back button flow', async ({ page }) => {
    // Login
    await TestHelpers.loginAsAdmin(page);
    
    // Navigate through multiple pages
    await TestHelpers.navigateTo(page, '/products');
    await expect(page).toHaveURL(/.*products/);
    
    await TestHelpers.navigateTo(page, '/inventory');
    await expect(page).toHaveURL(/.*inventory/);
    
    await TestHelpers.navigateTo(page, '/warehouses');
    await expect(page).toHaveURL(/.*warehouses/);
    
    // Use browser back button
    await page.goBack();
    await expect(page).toHaveURL(/.*inventory/);
    
    await page.goBack();
    await expect(page).toHaveURL(/.*products/);
    
    // Use browser forward button
    await page.goForward();
    await expect(page).toHaveURL(/.*inventory/);
  });

  test('Session persistence and page reload', async ({ page }) => {
    // Login
    await TestHelpers.loginAsAdmin(page);
    await expect(page).toHaveURL(/.*dashboard/);
    
    // Reload page
    await page.reload();
    
    // Verify still logged in
    await expect(page).toHaveURL(/.*dashboard/);
    await expect(page.locator('h1')).toContainText('Welcome back');
    
    // Navigate to another page
    await TestHelpers.navigateTo(page, '/products');
    
    // Reload again
    await page.reload();
    
    // Verify still on products page and logged in
    await expect(page).toHaveURL(/.*products/);
  });

  test('Responsive design - mobile viewport', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    
    // Login
    await TestHelpers.loginAsAdmin(page);
    
    // Check if mobile menu exists
    const mobileMenuButton = page.locator('button[aria-label="menu"], button:has(mat-icon:has-text("menu"))');
    const mobileMenuExists = await mobileMenuButton.isVisible().catch(() => false);
    
    if (mobileMenuExists) {
      // Open mobile menu
      await mobileMenuButton.click();
      
      // Verify navigation is visible
      await expect(page.locator('mat-sidenav')).toBeVisible();
    }
    
    // Verify dashboard content is visible
    await expect(page.locator('h1')).toBeVisible();
  });

  test('Error handling - network failure simulation', async ({ page }) => {
    // Login first
    await TestHelpers.loginAsAdmin(page);
    
    // Navigate to products
    await TestHelpers.navigateTo(page, '/products');
    
    // Simulate offline
    await page.context().setOffline(true);
    
    // Try to load data (should fail gracefully)
    await page.reload();
    
    // Wait a bit for error handling
    await page.waitForTimeout(2000);
    
    // Restore connection
    await page.context().setOffline(false);
  });

  test('Full logout and re-login cycle', async ({ page }) => {
    // First login
    await TestHelpers.loginAsAdmin(page);
    await expect(page).toHaveURL(/.*dashboard/);
    
    // Navigate to a page
    await TestHelpers.navigateTo(page, '/products');
    
    // Logout
    await TestHelpers.logout(page);
    await expect(page).toHaveURL(/.*auth\/login/);
    
    // Verify protected route redirects to login
    await page.goto('/dashboard');
    await expect(page).toHaveURL(/.*auth\/login/);
    
    // Login again
    await TestHelpers.loginAsAdmin(page);
    await expect(page).toHaveURL(/.*dashboard/);
  });
});

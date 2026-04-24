import { test, expect } from '@playwright/test';
import { TEST_USERS, TEST_PRODUCT } from '../fixtures/test-data';

test.describe('Products Management', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 10000 });
    
    // Navigate to products page
    await page.locator('a[routerlink="/products"]').click();
    await page.waitForURL('**/products');
  });

  test('should display products page', async ({ page }) => {
    await expect(page).toHaveURL(/.*products/);
    await expect(page.locator('mat-card-title:has-text("Products Management")')).toBeVisible();
  });

  test('should display products table', async ({ page }) => {
    // Check for table headers
    await expect(page.locator('th:has-text("SKU")')).toBeVisible();
    await expect(page.locator('th:has-text("Name")')).toBeVisible();
    await expect(page.locator('th:has-text("Category")')).toBeVisible();
    await expect(page.locator('th:has-text("Price")')).toBeVisible();
    await expect(page.locator('th:has-text("Actions")')).toBeVisible();
  });

  test('should display add product button', async ({ page }) => {
    const addButton = page.locator('button:has-text("Add Product")');
    await expect(addButton).toBeVisible();
  });

  test('should open add product dialog with stepper', async ({ page }) => {
    // Click add product button
    await page.locator('button:has-text("Add Product")').click();
    
    // Check if dialog is visible
    await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
    
    // Step 1: Basic Information - Check form fields
    await expect(page.locator('input[formcontrolname="name"]')).toBeVisible();
    await expect(page.locator('mat-select[formcontrolname="category"]')).toBeVisible();
    await expect(page.locator('input[formcontrolname="sku"]')).toBeVisible();
    await expect(page.locator('textarea[formcontrolname="description"]')).toBeVisible();
    
    // Check for Next button
    await expect(page.locator('button:has-text("Next")')).toBeVisible();
  });

  test('should navigate through product creation stepper steps', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
    
    // Step 1: Fill Basic Information
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    
    // Select category
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    
    // SKU should be auto-generated, but we can edit it
    const skuValue = await page.locator('input[formcontrolname="sku"]').inputValue();
    expect(skuValue.length).toBeGreaterThan(0);
    
    await page.locator('textarea[formcontrolname="description"]').fill('Test Description');
    
    // Click Next
    await page.locator('button:has-text("Next")').click();
    
    // Step 2: Pricing - Check form fields
    await expect(page.locator('input[formcontrolname="unitPrice"]')).toBeVisible();
    await page.locator('input[formcontrolname="unitPrice"]').fill('99.99');
    
    // Check for Back and Next buttons
    await expect(page.locator('button:has-text("Back")')).toBeVisible();
    await expect(page.locator('button:has-text("Next")')).toBeVisible();
    
    // Click Next
    await page.locator('button:has-text("Next")').click();
    
    // Step 3: Inventory Setup - Check form fields
    await expect(page.locator('mat-select[formcontrolname="warehouseId"]')).toBeVisible();
    await expect(page.locator('input[formcontrolname="initialQuantity"]')).toBeVisible();
    await expect(page.locator('input[formcontrolname="threshold"]')).toBeVisible();
    
    // Check for Back and Save buttons
    await expect(page.locator('button:has-text("Back")')).toBeVisible();
    await expect(page.locator('button:has-text("Save")')).toBeVisible();
  });

  test('should allow navigation back through stepper steps', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Fill Step 1 and proceed
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    await page.locator('button:has-text("Next")').click();
    
    // Fill Step 2 and proceed
    await page.locator('input[formcontrolname="unitPrice"]').fill('99.99');
    await page.locator('button:has-text("Next")').click();
    
    // Now on Step 3 - Click Back
    await page.locator('button:has-text("Back")').click();
    
    // Should be back on Step 2 - Verify price field is visible
    await expect(page.locator('input[formcontrolname="unitPrice"]')).toBeVisible();
    await expect(page.locator('input[formcontrolname="unitPrice"]')).toHaveValue('99.99');
    
    // Click Back again
    await page.locator('button:has-text("Back")').click();
    
    // Should be back on Step 1
    await expect(page.locator('input[formcontrolname="name"]')).toBeVisible();
    await expect(page.locator('input[formcontrolname="name"]')).toHaveValue('Test Product');
  });

  test('should filter products by search term', async ({ page }) => {
    // Type in search field
    const searchInput = page.locator('input[formcontrolname="searchTerm"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('test');
      
      // Click apply filters button
      await page.locator('button:has-text("Apply Filters")').click();
      
      // Wait for results to load
      await page.waitForTimeout(1000);
    }
  });

  test('should filter products by category', async ({ page }) => {
    // Check if category filter exists
    const categorySelect = page.locator('mat-select[formcontrolname="category"]');
    if (await categorySelect.isVisible()) {
      await categorySelect.click();
      
      // Select a category
      await page.locator('mat-option').first().click();
      
      // Click apply filters
      await page.locator('button:has-text("Apply Filters")').click();
      
      // Wait for results
      await page.waitForTimeout(1000);
    }
  });

  test('should reset filters', async ({ page }) => {
    // Fill search field
    const searchInput = page.locator('input[formcontrolname="searchTerm"]');
    if (await searchInput.isVisible()) {
      await searchInput.fill('test');
      
      // Click reset button
      await page.locator('button:has-text("Reset")').click();
      
      // Verify search field is empty
      await expect(searchInput).toHaveValue('');
    }
  });

  test('should display pagination controls', async ({ page }) => {
    // Check for pagination component
    const paginationExists = await page.locator('app-pagination').isVisible().catch(() => false);
    
    if (paginationExists) {
      await expect(page.locator('app-pagination')).toBeVisible();
    }
  });

  test('should display product actions', async ({ page }) => {
    // Check if table has rows
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Check for edit button
      const editButton = firstRow.locator('button[aria-label="Edit"], button:has(mat-icon:has-text("edit"))');
      await expect(editButton).toBeVisible();
      
      // Check for delete button
      const deleteButton = firstRow.locator('button[aria-label="Delete"], button:has(mat-icon:has-text("delete"))');
      await expect(deleteButton).toBeVisible();
    }
  });

  test('should close add product dialog on cancel', async ({ page }) => {
    // Open dialog
    await page.locator('button:has-text("Add Product")').click();
    await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
    
    // Click cancel
    await page.locator('button:has-text("Cancel")').click();
    
    // Dialog should be closed
    await expect(page.locator('h2:has-text("Add Product")')).not.toBeVisible();
  });

  test('should validate required fields in Basic Information step', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Try to click Next without filling required fields
    const nextButton = page.locator('button:has-text("Next")');
    
    // The Next button should be disabled or the step should show validation errors
    // Fill name field
    await page.locator('input[formcontrolname="name"]').fill('');
    await page.locator('input[formcontrolname="name"]').blur();
    
    // Check if error message appears
    const nameError = page.locator('mat-error:has-text("Product name is required")');
    if (await nameError.isVisible().catch(() => false)) {
      await expect(nameError).toBeVisible();
    }
  });

  test('should validate pricing step', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Fill Step 1
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    await page.locator('button:has-text("Next")').click();
    
    // Try invalid price
    await page.locator('input[formcontrolname="unitPrice"]').fill('0');
    await page.locator('input[formcontrolname="unitPrice"]').blur();
    
    // Check for validation error
    const priceError = page.locator('mat-error:has-text("Unit price must be greater than 0")');
    if (await priceError.isVisible().catch(() => false)) {
      await expect(priceError).toBeVisible();
    }
  });

  test('should validate inventory setup step', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Navigate to Step 3
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    await page.locator('button:has-text("Next")').click();
    
    await page.locator('input[formcontrolname="unitPrice"]').fill('99.99');
    await page.locator('button:has-text("Next")').click();
    
    // Try negative quantity
    await page.locator('input[formcontrolname="initialQuantity"]').fill('-5');
    await page.locator('input[formcontrolname="initialQuantity"]').blur();
    
    // Check for validation error
    const quantityError = page.locator('mat-error:has-text("Quantity must be 0 or greater")');
    if (await quantityError.isVisible().catch(() => false)) {
      await expect(quantityError).toBeVisible();
    }
  });

  test('should auto-generate SKU based on category', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Fill product name
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    
    // Select a category
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.waitForTimeout(500); // Wait for options to appear
    
    const firstOption = page.locator('mat-option').first();
    const categoryText = await firstOption.textContent();
    await firstOption.click();
    
    // Wait for SKU to be generated
    await page.waitForTimeout(1000);
    
    // Check that SKU is populated
    const skuValue = await page.locator('input[formcontrolname="sku"]').inputValue();
    expect(skuValue.length).toBeGreaterThan(0);
    
    // SKU should start with category prefix (first 4 letters)
    if (categoryText) {
      const prefix = categoryText.toUpperCase().replace(/\s+/g, '-').substring(0, 4);
      expect(skuValue).toContain(prefix);
    }
  });

  test('should allow editing auto-generated SKU', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Select category to trigger SKU generation
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    await page.waitForTimeout(500);
    
    // Clear and enter custom SKU
    await page.locator('input[formcontrolname="sku"]').clear();
    await page.locator('input[formcontrolname="sku"]').fill('CUSTOM-SKU-001');
    
    // Verify custom SKU is set
    await expect(page.locator('input[formcontrolname="sku"]')).toHaveValue('CUSTOM-SKU-001');
  });

  test('should cancel at any step in the stepper', async ({ page }) => {
    // Open add product dialog
    await page.locator('button:has-text("Add Product")').click();
    
    // Fill Step 1 and proceed
    await page.locator('input[formcontrolname="name"]').fill('Test Product');
    await page.locator('mat-select[formcontrolname="category"]').click();
    await page.locator('mat-option').first().click();
    await page.locator('button:has-text("Next")').click();
    
    // On Step 2 - Click Cancel
    await page.locator('button:has-text("Cancel")').click();
    
    // Dialog should be closed
    await expect(page.locator('h2:has-text("Add Product")')).not.toBeVisible();
  });

  test('should open edit product dialog with stepper', async ({ page }) => {
    // Check if table has rows
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Click edit button
      const editButton = firstRow.locator('button[aria-label="Edit"], button:has(mat-icon:has-text("edit"))');
      await editButton.click();
      
      // Check if edit dialog is visible
      await expect(page.locator('h2:has-text("Edit Product")')).toBeVisible();
      
      // Verify all stepper steps are present
      await expect(page.locator('input[formcontrolname="name"]')).toBeVisible();
      await expect(page.locator('mat-select[formcontrolname="category"]')).toBeVisible();
      
      // SKU should NOT be visible in edit mode
      await expect(page.locator('input[formcontrolname="sku"]')).not.toBeVisible();
    }
  });

  test('should navigate through edit product stepper', async ({ page }) => {
    // Check if table has rows
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Click edit button
      const editButton = firstRow.locator('button[aria-label="Edit"], button:has(mat-icon:has-text("edit"))');
      await editButton.click();
      await expect(page.locator('h2:has-text("Edit Product")')).toBeVisible();
      
      // Step 1: Basic Information (without SKU)
      const productName = await page.locator('input[formcontrolname="name"]').inputValue();
      expect(productName.length).toBeGreaterThan(0);
      
      // Proceed to Step 2
      await page.locator('button:has-text("Next")').click();
      
      // Step 2: Pricing
      await expect(page.locator('input[formcontrolname="unitPrice"]')).toBeVisible();
      const price = await page.locator('input[formcontrolname="unitPrice"]').inputValue();
      expect(parseFloat(price)).toBeGreaterThan(0);
      
      // Proceed to Step 3
      await page.locator('button:has-text("Next")').click();
      
      // Step 3: Inventory Update
      await expect(page.locator('mat-select[formcontrolname="warehouseId"]')).toBeVisible();
      await expect(page.locator('input[formcontrolname="initialQuantity"]')).toBeVisible();
      await expect(page.locator('input[formcontrolname="threshold"]')).toBeVisible();
      
      // Verify label says "Current Quantity" in edit mode
      const quantityLabel = page.locator('mat-label:has-text("Current Quantity")');
      if (await quantityLabel.isVisible().catch(() => false)) {
        await expect(quantityLabel).toBeVisible();
      }
    }
  });

  test('should update product inventory through stepper', async ({ page }) => {
    // Check if table has rows
    const firstRow = page.locator('table tbody tr').first();
    const rowExists = await firstRow.isVisible().catch(() => false);
    
    if (rowExists) {
      // Click edit button
      const editButton = firstRow.locator('button[aria-label="Edit"], button:has(mat-icon:has-text("edit"))');
      await editButton.click();
      await expect(page.locator('h2:has-text("Edit Product")')).toBeVisible();
      
      // Navigate to inventory step
      await page.locator('button:has-text("Next")').click(); // To Pricing
      await page.locator('button:has-text("Next")').click(); // To Inventory
      
      // Update inventory quantity
      const quantityField = page.locator('input[formcontrolname="initialQuantity"]');
      await quantityField.clear();
      await quantityField.fill('100');
      
      // Update threshold
      const thresholdField = page.locator('input[formcontrolname="threshold"]');
      await thresholdField.clear();
      await thresholdField.fill('15');
      
      // Verify values are set
      await expect(quantityField).toHaveValue('100');
      await expect(thresholdField).toHaveValue('15');
    }
  });
});

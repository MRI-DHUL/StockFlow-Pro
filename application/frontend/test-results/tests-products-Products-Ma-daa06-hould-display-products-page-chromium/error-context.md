# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: tests\products.spec.ts >> Products Management >> should display products page
- Location: e2e\tests\products.spec.ts:18:7

# Error details

```
TimeoutError: page.waitForURL: Timeout 10000ms exceeded.
=========================== logs ===========================
waiting for navigation to "**/dashboard" until "load"
============================================================
```

# Page snapshot

```yaml
- generic [ref=e6]:
  - generic [ref=e7]:
    - heading "StockFlow Pro" [level=1] [ref=e8]
    - paragraph [ref=e9]: Welcome back! Please login to your account.
  - generic [ref=e10]: An internal server error occurred. Please contact support.
  - generic [ref=e11]:
    - generic [ref=e12]:
      - generic [ref=e13]: Email
      - textbox "Email" [ref=e14]:
        - /placeholder: Enter your email
        - text: admin@stockflowpro.com
    - generic [ref=e15]:
      - generic [ref=e16]: Password
      - textbox "Password" [ref=e17]:
        - /placeholder: Enter your password
        - text: Admin@123
    - button "Login" [ref=e18] [cursor=pointer]
  - paragraph [ref=e20]:
    - text: Don't have an account?
    - link "Register here" [ref=e21] [cursor=pointer]:
      - /url: /auth/register
```

# Test source

```ts
  1   | import { test, expect } from '@playwright/test';
  2   | import { TEST_USERS, TEST_PRODUCT } from '../fixtures/test-data';
  3   | 
  4   | test.describe('Products Management', () => {
  5   |   test.beforeEach(async ({ page }) => {
  6   |     // Login before each test
  7   |     await page.goto('/auth/login');
  8   |     await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
  9   |     await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
  10  |     await page.locator('button[type="submit"]').click();
> 11  |     await page.waitForURL('**/dashboard', { timeout: 10000 });
      |                ^ TimeoutError: page.waitForURL: Timeout 10000ms exceeded.
  12  |     
  13  |     // Navigate to products page
  14  |     await page.locator('a[routerlink="/products"]').click();
  15  |     await page.waitForURL('**/products');
  16  |   });
  17  | 
  18  |   test('should display products page', async ({ page }) => {
  19  |     await expect(page).toHaveURL(/.*products/);
  20  |     await expect(page.locator('mat-card-title:has-text("Products Management")')).toBeVisible();
  21  |   });
  22  | 
  23  |   test('should display products table', async ({ page }) => {
  24  |     // Check for table headers
  25  |     await expect(page.locator('th:has-text("SKU")')).toBeVisible();
  26  |     await expect(page.locator('th:has-text("Name")')).toBeVisible();
  27  |     await expect(page.locator('th:has-text("Category")')).toBeVisible();
  28  |     await expect(page.locator('th:has-text("Price")')).toBeVisible();
  29  |     await expect(page.locator('th:has-text("Actions")')).toBeVisible();
  30  |   });
  31  | 
  32  |   test('should display add product button', async ({ page }) => {
  33  |     const addButton = page.locator('button:has-text("Add Product")');
  34  |     await expect(addButton).toBeVisible();
  35  |   });
  36  | 
  37  |   test('should open add product dialog with stepper', async ({ page }) => {
  38  |     // Click add product button
  39  |     await page.locator('button:has-text("Add Product")').click();
  40  |     
  41  |     // Check if dialog is visible
  42  |     await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
  43  |     
  44  |     // Step 1: Basic Information - Check form fields
  45  |     await expect(page.locator('input[formcontrolname="name"]')).toBeVisible();
  46  |     await expect(page.locator('mat-select[formcontrolname="category"]')).toBeVisible();
  47  |     await expect(page.locator('input[formcontrolname="sku"]')).toBeVisible();
  48  |     await expect(page.locator('textarea[formcontrolname="description"]')).toBeVisible();
  49  |     
  50  |     // Check for Next button
  51  |     await expect(page.locator('button:has-text("Next")')).toBeVisible();
  52  |   });
  53  | 
  54  |   test('should navigate through product creation stepper steps', async ({ page }) => {
  55  |     // Open add product dialog
  56  |     await page.locator('button:has-text("Add Product")').click();
  57  |     await expect(page.locator('h2:has-text("Add Product")')).toBeVisible();
  58  |     
  59  |     // Step 1: Fill Basic Information
  60  |     await page.locator('input[formcontrolname="name"]').fill('Test Product');
  61  |     
  62  |     // Select category
  63  |     await page.locator('mat-select[formcontrolname="category"]').click();
  64  |     await page.locator('mat-option').first().click();
  65  |     
  66  |     // SKU should be auto-generated, but we can edit it
  67  |     const skuValue = await page.locator('input[formcontrolname="sku"]').inputValue();
  68  |     expect(skuValue.length).toBeGreaterThan(0);
  69  |     
  70  |     await page.locator('textarea[formcontrolname="description"]').fill('Test Description');
  71  |     
  72  |     // Click Next
  73  |     await page.locator('button:has-text("Next")').click();
  74  |     
  75  |     // Step 2: Pricing - Check form fields
  76  |     await expect(page.locator('input[formcontrolname="unitPrice"]')).toBeVisible();
  77  |     await page.locator('input[formcontrolname="unitPrice"]').fill('99.99');
  78  |     
  79  |     // Check for Back and Next buttons
  80  |     await expect(page.locator('button:has-text("Back")')).toBeVisible();
  81  |     await expect(page.locator('button:has-text("Next")')).toBeVisible();
  82  |     
  83  |     // Click Next
  84  |     await page.locator('button:has-text("Next")').click();
  85  |     
  86  |     // Step 3: Inventory Setup - Check form fields
  87  |     await expect(page.locator('mat-select[formcontrolname="warehouseId"]')).toBeVisible();
  88  |     await expect(page.locator('input[formcontrolname="initialQuantity"]')).toBeVisible();
  89  |     await expect(page.locator('input[formcontrolname="threshold"]')).toBeVisible();
  90  |     
  91  |     // Check for Back and Save buttons
  92  |     await expect(page.locator('button:has-text("Back")')).toBeVisible();
  93  |     await expect(page.locator('button:has-text("Save")')).toBeVisible();
  94  |   });
  95  | 
  96  |   test('should allow navigation back through stepper steps', async ({ page }) => {
  97  |     // Open add product dialog
  98  |     await page.locator('button:has-text("Add Product")').click();
  99  |     
  100 |     // Fill Step 1 and proceed
  101 |     await page.locator('input[formcontrolname="name"]').fill('Test Product');
  102 |     await page.locator('mat-select[formcontrolname="category"]').click();
  103 |     await page.locator('mat-option').first().click();
  104 |     await page.locator('button:has-text("Next")').click();
  105 |     
  106 |     // Fill Step 2 and proceed
  107 |     await page.locator('input[formcontrolname="unitPrice"]').fill('99.99');
  108 |     await page.locator('button:has-text("Next")').click();
  109 |     
  110 |     // Now on Step 3 - Click Back
  111 |     await page.locator('button:has-text("Back")').click();
```
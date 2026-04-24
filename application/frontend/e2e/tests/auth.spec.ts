import { test, expect } from '@playwright/test';
import { TEST_USERS } from '../fixtures/test-data';

test.describe('Authentication', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('should redirect to login page when not authenticated', async ({ page }) => {
    await expect(page).toHaveURL(/.*auth\/login/);
    await expect(page.locator('h1')).toContainText('Welcome Back');
  });

  test('should display login form', async ({ page }) => {
    await page.goto('/auth/login');
    
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
  });

  test('should show validation errors for empty fields', async ({ page }) => {
    await page.goto('/auth/login');
    
    // Click submit without filling fields
    await page.locator('button[type="submit"]').click();
    
    // Check for validation messages
    await expect(page.locator('mat-error')).toHaveCount(2);
  });

  test('should login successfully with valid credentials', async ({ page }) => {
    await page.goto('/auth/login');
    
    // Fill in login form
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    
    // Submit form
    await page.locator('button[type="submit"]').click();
    
    // Wait for navigation to dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 });
    
    // Verify we're on dashboard
    await expect(page).toHaveURL(/.*dashboard/);
    await expect(page.locator('h1')).toContainText('Welcome back');
  });

  test('should show error message for invalid credentials', async ({ page }) => {
    await page.goto('/auth/login');
    
    // Fill in login form with invalid credentials
    await page.locator('input[type="email"]').fill('invalid@example.com');
    await page.locator('input[type="password"]').fill('wrongpassword');
    
    // Submit form
    await page.locator('button[type="submit"]').click();
    
    // Wait for error message
    await expect(page.locator('.error-message, .mat-error, [role="alert"]')).toBeVisible({ timeout: 5000 });
  });

  test('should navigate to register page', async ({ page }) => {
    await page.goto('/auth/login');
    
    // Click register link
    await page.locator('a[href*="register"]').click();
    
    // Verify we're on register page
    await expect(page).toHaveURL(/.*auth\/register/);
    await expect(page.locator('h1')).toContainText('Create Account');
  });

  test('should logout successfully', async ({ page }) => {
    // Login first
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard');
    
    // Logout
    await page.locator('button[aria-label="Logout"], button:has-text("Logout")').click();
    
    // Verify redirected to login
    await expect(page).toHaveURL(/.*auth\/login/);
  });
});

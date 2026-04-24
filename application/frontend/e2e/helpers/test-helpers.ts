import { Page } from '@playwright/test';
import { TEST_USERS } from '../fixtures/test-data';

/**
 * Helper class for common E2E test actions
 */
export class TestHelpers {
  /**
   * Login with admin credentials
   */
  static async loginAsAdmin(page: Page): Promise<void> {
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(TEST_USERS.admin.email);
    await page.locator('input[type="password"]').fill(TEST_USERS.admin.password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 10000 });
  }

  /**
   * Login with custom credentials
   */
  static async login(page: Page, email: string, password: string): Promise<void> {
    await page.goto('/auth/login');
    await page.locator('input[type="email"]').fill(email);
    await page.locator('input[type="password"]').fill(password);
    await page.locator('button[type="submit"]').click();
    await page.waitForURL('**/dashboard', { timeout: 10000 });
  }

  /**
   * Logout from application
   */
  static async logout(page: Page): Promise<void> {
    await page.locator('button[aria-label="Logout"], button:has-text("Logout")').click();
    await page.waitForURL('**/auth/login');
  }

  /**
   * Navigate to a specific page
   */
  static async navigateTo(page: Page, route: string): Promise<void> {
    await page.locator(`a[routerlink="${route}"]`).click();
    await page.waitForURL(`**${route}`);
  }

  /**
   * Wait for loading to complete
   */
  static async waitForLoading(page: Page): Promise<void> {
    // Wait for loading spinner to disappear
    const spinner = page.locator('app-loading-spinner, .loading-spinner, mat-spinner');
    const spinnerExists = await spinner.isVisible().catch(() => false);
    
    if (spinnerExists) {
      await spinner.waitFor({ state: 'hidden', timeout: 10000 });
    }
  }

  /**
   * Fill form field by name
   */
  static async fillFormField(page: Page, fieldName: string, value: string): Promise<void> {
    await page.locator(`input[formcontrolname="${fieldName}"], textarea[formcontrolname="${fieldName}"]`).fill(value);
  }

  /**
   * Click button by text
   */
  static async clickButton(page: Page, buttonText: string): Promise<void> {
    await page.locator(`button:has-text("${buttonText}")`).click();
  }

  /**
   * Wait for toast/notification
   */
  static async waitForToast(page: Page, message?: string): Promise<void> {
    const toastSelector = '.toast-container, .mat-snack-bar-container, [role="alert"]';
    const toast = page.locator(toastSelector);
    await toast.waitFor({ state: 'visible', timeout: 5000 });
    
    if (message) {
      await page.locator(`${toastSelector}:has-text("${message}")`).waitFor({ state: 'visible' });
    }
  }

  /**
   * Confirm dialog action
   */
  static async confirmDialog(page: Page): Promise<void> {
    await page.locator('button:has-text("Confirm"), button:has-text("Yes"), button:has-text("OK")').click();
  }

  /**
   * Cancel dialog action
   */
  static async cancelDialog(page: Page): Promise<void> {
    await page.locator('button:has-text("Cancel"), button:has-text("No")').click();
  }

  /**
   * Check if element has specific class
   */
  static async hasClass(page: Page, selector: string, className: string): Promise<boolean> {
    const element = page.locator(selector);
    const classes = await element.getAttribute('class');
    return classes?.includes(className) ?? false;
  }

  /**
   * Get table row count
   */
  static async getTableRowCount(page: Page): Promise<number> {
    const rows = page.locator('table tbody tr');
    return await rows.count();
  }

  /**
   * Get pagination info
   */
  static async getPaginationInfo(page: Page): Promise<{ current: number; total: number }> {
    const paginationText = await page.locator('.pagination-info, app-pagination').textContent();
    // Parse pagination text like "Page 1 of 5"
    const match = paginationText?.match(/(\d+)\s*of\s*(\d+)/);
    
    return {
      current: match ? parseInt(match[1]) : 1,
      total: match ? parseInt(match[2]) : 1
    };
  }

  /**
   * Take screenshot with timestamp
   */
  static async takeScreenshot(page: Page, name: string): Promise<void> {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    await page.screenshot({ path: `screenshots/${name}-${timestamp}.png`, fullPage: true });
  }
}

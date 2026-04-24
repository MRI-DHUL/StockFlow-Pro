# E2E Tests with Playwright

This directory contains end-to-end tests for the StockFlow Pro frontend application using Playwright.

## Structure

```
e2e/
├── fixtures/          # Test data and fixtures
├── helpers/           # Helper functions and utilities
├── tests/            # Test files
│   ├── auth.spec.ts         # Authentication tests
│   ├── dashboard.spec.ts    # Dashboard tests
│   ├── products.spec.ts     # Products management tests
│   ├── inventory.spec.ts    # Inventory management tests
│   └── navigation.spec.ts   # Navigation tests
└── README.md
```

## Running Tests

### Run all tests
```bash
npm run test:e2e
```

### Run tests in headed mode (see browser)
```bash
npm run test:e2e:headed
```

### Run tests in UI mode (interactive)
```bash
npm run test:e2e:ui
```

### Run specific test file
```bash
npx playwright test auth.spec.ts
```

### Run tests in specific browser
```bash
npx playwright test --project=chromium
npx playwright test --project=firefox
npx playwright test --project=webkit
```

### Debug tests
```bash
npm run test:e2e:debug
```

## Test Reports

After running tests, view the HTML report:
```bash
npx playwright show-report
```

## Configuration

Test configuration is in `playwright.config.ts`. Key settings:

- **Base URL**: http://localhost:4200
- **Browsers**: Chromium, Firefox, WebKit, Mobile Chrome, Mobile Safari
- **Retries**: 2 on CI, 0 locally
- **Screenshots**: On failure
- **Videos**: On failure
- **Traces**: On first retry

## Writing Tests

### Example test structure:

```typescript
import { test, expect } from '@playwright/test';
import { TestHelpers } from '../helpers/test-helpers';

test.describe('Feature Name', () => {
  test.beforeEach(async ({ page }) => {
    await TestHelpers.loginAsAdmin(page);
    await TestHelpers.navigateTo(page, '/feature');
  });

  test('should do something', async ({ page }) => {
    // Arrange
    await page.locator('button').click();
    
    // Act
    await TestHelpers.clickButton(page, 'Submit');
    
    // Assert
    await expect(page.locator('.success-message')).toBeVisible();
  });
});
```

## Best Practices

1. **Use data-testid attributes** for stable selectors
2. **Wait for elements** before interacting
3. **Use helper functions** for common actions
4. **Keep tests independent** - each test should be runnable in isolation
5. **Use descriptive test names** that explain what is being tested
6. **Clean up test data** after tests
7. **Use fixtures** for test data

## Debugging Tips

1. Use `page.pause()` to pause execution and inspect
2. Enable `--headed` mode to see browser
3. Use `--debug` flag for step-by-step debugging
4. Check screenshots and videos for failed tests
5. Use trace viewer: `npx playwright show-trace trace.zip`

## CI/CD Integration

Tests are configured to run in CI with:
- Parallel execution disabled
- 2 retries
- Only on Chromium browser
- Screenshot and video capture on failure

## Environment Variables

- `CI`: Set to true to enable CI mode
- `BASE_URL`: Override base URL (default: http://localhost:4200)

## Prerequisites

- Node.js 18+
- Angular dev server running on http://localhost:4200
- Backend API running and accessible

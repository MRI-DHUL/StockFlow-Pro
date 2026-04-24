# Product Stepper E2E Tests Documentation

## Overview
This document describes the Playwright E2E tests for the new multi-step product form with stepper navigation.

## Test Coverage

### 1. Product Creation Flow Tests

#### Test: `should open add product dialog with stepper`
- **Purpose**: Verify the product dialog opens with all Step 1 fields visible
- **Verifies**:
  - Dialog title shows "Add Product"
  - Basic Information fields are visible: name, category, SKU, description
  - Next button is present

#### Test: `should navigate through product creation stepper steps`
- **Purpose**: Test complete stepper navigation through all 3 steps
- **Flow**:
  1. **Step 1 - Basic Information**: Fill name, select category, verify auto-generated SKU, fill description → Click Next
  2. **Step 2 - Pricing**: Fill unit price, verify Back/Next buttons → Click Next
  3. **Step 3 - Inventory Setup**: Verify warehouse, quantity, threshold fields, verify Back/Save buttons

#### Test: `should allow navigation back through stepper steps`
- **Purpose**: Test backward navigation preserves form data
- **Flow**:
  1. Fill Step 1 → Next
  2. Fill Step 2 → Next
  3. On Step 3 → Back
  4. Verify Step 2 data is preserved
  5. Back again → Verify Step 1 data is preserved

### 2. Validation Tests

#### Test: `should validate required fields in Basic Information step`
- **Purpose**: Test required field validation on Step 1
- **Verifies**: Product name required validation message

#### Test: `should validate pricing step`
- **Purpose**: Test pricing validation rules
- **Verifies**: Unit price must be greater than 0

#### Test: `should validate inventory setup step`
- **Purpose**: Test inventory field validation
- **Verifies**: Quantity must be 0 or greater

### 3. SKU Generation Tests

#### Test: `should auto-generate SKU based on category`
- **Purpose**: Verify SKU auto-generation functionality
- **Verifies**:
  - SKU is automatically populated when category is selected
  - SKU follows format: `{CATEGORY_PREFIX}-{NUMBER}`
  - Category prefix is first 4 letters of category name

#### Test: `should allow editing auto-generated SKU`
- **Purpose**: Verify users can override auto-generated SKU
- **Flow**:
  1. Select category to trigger SKU generation
  2. Clear SKU field
  3. Enter custom SKU
  4. Verify custom SKU is retained

### 4. Product Editing Tests

#### Test: `should open edit product dialog with stepper`
- **Purpose**: Verify edit dialog opens with product data
- **Verifies**:
  - Dialog title shows "Edit Product"
  - Step 1 fields are visible with product data
  - SKU field is NOT visible in edit mode

#### Test: `should navigate through edit product stepper`
- **Purpose**: Test stepper navigation in edit mode
- **Flow**:
  1. **Step 1**: Verify product name and category are pre-filled
  2. **Step 2**: Verify unit price is pre-filled
  3. **Step 3**: Verify inventory data is loaded (warehouse, quantity, threshold)
  4. Verify label shows "Current Quantity" instead of "Initial Quantity"

#### Test: `should update product inventory through stepper`
- **Purpose**: Test inventory update capability
- **Flow**:
  1. Navigate to inventory step (Step 3)
  2. Update quantity to 100
  3. Update threshold to 15
  4. Verify values are set correctly

### 5. User Experience Tests

#### Test: `should close add product dialog on cancel`
- **Purpose**: Verify Cancel button closes dialog from Step 1
- **Flow**: Open dialog → Click Cancel → Verify dialog is closed

#### Test: `should cancel at any step in the stepper`
- **Purpose**: Verify Cancel works from any step
- **Flow**: Navigate to Step 2 → Click Cancel → Verify dialog is closed

## Form Structure

### Step 1: Basic Information
- **Fields**:
  - Product Name (required, text input)
  - Category (required, select dropdown)
  - SKU (required in create mode only, auto-generated but editable)
  - Description (optional, textarea)
- **Actions**: Cancel, Next

### Step 2: Pricing
- **Fields**:
  - Unit Price (required, number input, must be > 0)
- **Actions**: Back, Cancel, Next

### Step 3: Inventory Setup/Update
- **Fields**:
  - Warehouse (required, select dropdown)
  - Initial Quantity / Current Quantity (required, number input, must be >= 0)
  - Low Stock Threshold (required, number input, must be >= 0)
- **Actions**: Back, Cancel, Save

## Test Data

### Test Fixtures (from `test-data.ts`)
```typescript
export const TEST_PRODUCT = {
  name: 'Test Product',
  sku: 'TEST-001',
  description: 'This is a test product',
  category: 'Electronics',
  unitPrice: 99.99,
  warehouse: 'Main Warehouse',
  initialQuantity: 50,
  threshold: 10
};

export const TEST_PRODUCT_UPDATE = {
  name: 'Updated Test Product',
  description: 'This is an updated test product',
  category: 'Electronics',
  unitPrice: 149.99,
  currentQuantity: 100,
  threshold: 15
};
```

## Key Differences: Create vs Edit Mode

| Aspect | Create Mode | Edit Mode |
|--------|-------------|-----------|
| Dialog Title | "Add Product" | "Edit Product" |
| SKU Field | Visible (auto-generated, editable) | Hidden (not editable) |
| Quantity Label | "Initial Quantity" | "Current Quantity" |
| Step 3 Label | "Inventory Setup" | "Inventory Update" |
| Submit Button | "Save" | "Save" |

## Running the Tests

### Run all product tests
```bash
npx playwright test products.spec.ts
```

### Run specific test
```bash
npx playwright test products.spec.ts -g "should navigate through product creation stepper steps"
```

### Run in headed mode (see browser)
```bash
npx playwright test products.spec.ts --headed
```

### Run in UI mode (interactive debugging)
```bash
npx playwright test products.spec.ts --ui
```

### Debug a specific test
```bash
npx playwright test products.spec.ts --debug -g "should navigate"
```

## Expected Behavior

1. **Stepper Header**: Hidden via CSS (no visible step numbers or progress bar)
2. **Linear Navigation**: Users must complete each step before proceeding to next
3. **Form Validation**: Each step validates independently
4. **Data Persistence**: Form data is retained when navigating back/forward
5. **Cancel Anytime**: Users can cancel from any step
6. **API Calls**: 
   - Create mode: Product creation followed by inventory creation (chained)
   - Edit mode: Product update followed by inventory update (chained)

## Notes

- All tests check for element visibility before interacting to avoid timing issues
- Tests include fallback checks with `.catch(() => false)` for optional elements
- Wait timeouts are used sparingly (500-1000ms) only for async operations like SKU generation
- Tests verify both UI state and data persistence

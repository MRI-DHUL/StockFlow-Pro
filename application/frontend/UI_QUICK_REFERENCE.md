# 🎨 StockFlow Pro - Quick UI Reference

Quick reference guide for implementing and maintaining the StockFlow Pro UI design system.

---

## 🚀 Quick Start

### Adding a New Component
When creating a new component, follow this template:

```scss
mat-card {
  border-radius: 16px;
  box-shadow: 0 8px 16px rgba(0,0,0,0.1);
  margin: 24px;
  background: white;
  animation: fadeIn 0.4s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

mat-card-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 24px;
  margin: -16px -16px 24px -16px;
  border-radius: 16px 16px 0 0;

  mat-card-title {
    font-size: 1.8rem;
    font-weight: 600;
    margin: 0;
  }
}
```

---

## 🎨 Color Codes (Copy-Paste Ready)

### Primary Gradients
```scss
// Main Brand
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

// Accent
background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);

// Page Background
background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
```

### Feature-Specific Gradients
```scss
// Green (Warehouses, Success)
background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);

// Blue (Purchase Orders, Info)
background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);

// Purple (Orders)
background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);

// Orange (Warnings, Low Stock)
background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);

// Red (Errors, Cancelled)
background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);

// Teal (Stock Movements)
background: linear-gradient(135deg, #1abc9c 0%, #16a085 100%);

// Pink (Suppliers)
background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);

// Dark (Audit, Sidebar)
background: linear-gradient(135deg, #34495e 0%, #2c3e50 100%);
```

---

## 🏷️ Status Badge Component

### HTML Template
```html
<span [class]="getStatusClass(item.status)">
  {{ getStatusLabel(item.status) }}
</span>
```

### SCSS Styles
```scss
.status-active {
  color: #27ae60;
  font-weight: 700;
  background: rgba(46, 204, 113, 0.1);
  padding: 6px 14px;
  border-radius: 16px;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
}

.status-inactive {
  color: #e74c3c;
  font-weight: 700;
  background: rgba(231, 76, 60, 0.1);
  padding: 6px 14px;
  border-radius: 16px;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
}
```

---

## 📊 Table Styling

### Standard Table Template
```scss
.table-container {
  overflow-x: auto;
  margin: 20px 0;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.08);
}

table {
  width: 100%;
  background: white;

  th {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    font-weight: 600;
    font-size: 0.95rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    padding: 16px !important;
  }

  td {
    padding: 14px !important;
    font-size: 0.95rem;
    color: #2c3e50;
  }

  tr {
    transition: all 0.3s ease;

    &:hover:not(mat-header-row) {
      background: #f8f9fa;
      transform: scale(1.01);
    }
  }

  mat-icon {
    transition: all 0.3s ease;
    
    &:hover {
      transform: scale(1.2);
    }
  }
}
```

---

## 🔘 Button Styles

### Primary Button (Gradient)
```html
<button mat-raised-button color="primary">
  <mat-icon>add</mat-icon>
  Add Item
</button>
```

```scss
button {
  border-radius: 8px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  }
}
```

### Button on Gradient Header (White)
```scss
mat-card-header {
  button {
    background: white !important;
    color: #27ae60 !important;
    box-shadow: 0 4px 12px rgba(0,0,0,0.2);

    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 16px rgba(0,0,0,0.3);
    }
  }
}
```

---

## 📝 Form Field Styling

### Filter Container
```scss
.filters-container {
  background: white;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  margin-bottom: 24px;
}

.filters-form {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
  align-items: center;

  mat-form-field {
    min-width: 220px;
    flex: 1;
  }
}
```

### Input Fields (Login/Forms)
```scss
.form-control {
  width: 100%;
  padding: 14px 18px;
  border: 2px solid #ecf0f1;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: #f8f9fa;

  &:focus {
    outline: none;
    border-color: #667eea;
    background: white;
    box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.1);
  }

  &.invalid {
    border-color: #e74c3c;
    background: rgba(231, 76, 60, 0.05);
  }
}
```

---

## 🎭 Animation Templates

### Fade In (Page Load)
```scss
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.component {
  animation: fadeIn 0.4s ease-out;
}
```

### Slide Up (Modals)
```scss
@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

.modal {
  animation: slideUp 0.6s ease-out;
}
```

### Pulse (Alerts)
```scss
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.alert {
  animation: pulse 2s ease-in-out infinite;
}
```

### Shake (Errors)
```scss
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-10px); }
  75% { transform: translateX(10px); }
}

.error {
  animation: shake 0.5s ease-in-out;
}
```

---

## 🎯 Common Use Cases

### Adding a Status Chip (Angular Material)
```html
<mat-chip [class]="'status-' + order.status.toLowerCase()">
  {{ order.status }}
</mat-chip>
```

```scss
mat-chip.status-pending {
  background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%) !important;
  color: white;
  font-weight: 700;
  padding: 8px 16px !important;
  border-radius: 16px !important;
  text-transform: uppercase;
  font-size: 0.85rem !important;
}
```

### Adding Hover Effect to Icons
```scss
mat-icon {
  transition: all 0.3s ease;
  
  &:hover {
    transform: scale(1.2);
    color: #764ba2;
  }
}
```

### Creating a Gradient Text Header
```html
<h1 class="gradient-text">StockFlow Pro</h1>
```

```scss
.gradient-text {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}
```

---

## 📦 Component Gradient Mapping

Choose the right gradient for your component:

| Component Type | Gradient | Use Case |
|---------------|----------|----------|
| Main Features | Purple-Blue | Products, Inventory, Dashboard |
| Storage | Green | Warehouses, Storage locations |
| Partnerships | Pink | Suppliers, Vendors |
| Transactions | Purple | Orders, Sales |
| Procurement | Blue | Purchase Orders, Buying |
| Movement | Teal | Stock movements, Transfers |
| Security | Dark | Audit logs, Security |
| Warnings | Orange | Low stock, Alerts |
| Errors | Red | Cancelled, Errors |
| Success | Green | Active, Completed |

---

## 🔧 Maintenance Tips

### Updating a Component's Theme
1. Choose gradient from palette
2. Update `mat-card-header` background
3. Update table `th` background
4. Update status chips if applicable
5. Test hover states

### Adding New Status Types
1. Define status enum/constants
2. Create `getStatusClass()` method
3. Add SCSS class with appropriate color
4. Use badge template pattern

### Responsive Considerations
```scss
@media (max-width: 768px) {
  mat-card {
    padding: 32px 24px;
    margin: 16px;
  }

  mat-card-header {
    padding: 20px;
  }

  mat-card-title {
    font-size: 1.5rem;
  }
}
```

---

## ✅ Checklist for New Components

- [ ] Card styling with 16px radius
- [ ] Gradient header with theme color
- [ ] Fade-in animation
- [ ] Table with gradient header (if applicable)
- [ ] Row hover effects
- [ ] Status badges/chips
- [ ] Icon button hover animations
- [ ] Responsive padding adjustments
- [ ] Consistent spacing (16px gaps, 24px margins)
- [ ] Filter container (if applicable)

---

## 🎨 Color Variable Reference

```scss
// Text Colors
$text-primary: #2c3e50;
$text-secondary: #7f8c8d;
$text-light: #95a5a6;

// Border & Background
$border-color: #ecf0f1;
$card-bg: #ffffff;
$input-bg: #f8f9fa;
$hover-bg: #f8f9fa;

// Status Colors
$success: #27ae60;
$info: #3498db;
$warning: #f39c12;
$error: #e74c3c;
```

---

## 📸 Before/After Comparison

### Before
- Flat colors (#4CAF50, #F44336)
- Simple shadows
- Basic hover states
- Standard spacing

### After
- Gradient backgrounds
- Multi-layered shadows
- Animated hover effects
- Professional spacing
- Status badges with capsule design
- Smooth transitions
- Themed components

---

## 🚀 Performance Tips

1. **Use CSS transforms** (not top/left) for animations
2. **Limit animations** to opacity and transform
3. **Use will-change** sparingly for complex animations
4. **Debounce hover effects** on large lists
5. **Lazy load** heavy gradients on scroll

---

## 📚 Additional Resources

- [Material Design 3](https://m3.material.io/)
- [CSS Gradient Generator](https://cssgradient.io/)
- [Color Palette Tool](https://coolors.co/)
- [Animation Timing Functions](https://easings.net/)

---

**Last Updated**: April 22, 2026  
**Maintained By**: StockFlow Pro Development Team  
**Version**: 1.0

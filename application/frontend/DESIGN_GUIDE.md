# 🎨 StockFlow Pro - UI Design Guide

Professional color scheme and styling guidelines for the application.

## 🌈 Color Palette

### Primary Colors
- **Primary Gradient**: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`
  - Purple-Blue gradient used for main actions, headers, and branding
  - Color codes: #667eea (Bright Blue) → #764ba2 (Deep Purple)

- **Secondary Gradient**: `linear-gradient(135deg, #f093fb 0%, #f5576c 100%)`
  - Pink gradient for accent elements
  - Color codes: #f093fb (Light Pink) → #f5576c (Coral Pink)

### Status Colors

#### Success/Active
- **Color**: #27ae60 (Emerald Green)
- **Usage**: Active status, success messages
- **Background**: `rgba(39, 174, 96, 0.1)`

#### Warning/Low Stock
- **Color**: #f39c12 (Orange)
- **Usage**: Warning messages, low stock alerts
- **Background**: `rgba(243, 156, 18, 0.1)`

#### Error/Inactive
- **Color**: #e74c3c (Red)
- **Usage**: Error messages, inactive status
- **Background**: `rgba(231, 76, 60, 0.1)`

#### Info
- **Color**: #3498db (Blue)
- **Usage**: Information messages, general highlights
- **Background**: `rgba(52, 152, 219, 0.1)`

### Neutral Colors
- **Text Primary**: #2c3e50 (Dark Gray-Blue)
- **Text Secondary**: #7f8c8d (Medium Gray)
- **Text Light**: #95a5a6 (Light Gray)
- **Background**: `linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)`
- **Card Background**: #ffffff (White)
- **Border**: #ecf0f1 (Very Light Gray)

## 🎭 Component Styling

### Cards
```scss
- Border Radius: 16px
- Box Shadow: 0 8px 16px rgba(0,0,0,0.1)
- Hover Shadow: 0 12px 24px rgba(0,0,0,0.15)
- Transition: all 0.3s ease
- Hover Transform: translateY(-2px)
```

### Buttons
```scss
- Primary: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
- Accent: linear-gradient(135deg, #f093fb 0%, #f5576c 100%)
- Border Radius: 8px
- Padding: 12px 24px
- Font Weight: 600
- Letter Spacing: 0.5px
- Hover Transform: translateY(-2px)
- Box Shadow: 0 4px 12px rgba(0,0,0,0.2)
```

### Tables
```scss
- Header Background: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
- Header Text: White, font-weight 600
- Row Hover: #f8f9fa
- Alternating Rows: #fafbfc
- Border Radius: 12px
- Hover Transform: scale(1.01)
```

### Forms
```scss
- Input Border: 2px solid #ecf0f1
- Input Background: #f8f9fa
- Focus Border: #667eea
- Focus Shadow: 0 0 0 4px rgba(102, 126, 234, 0.1)
- Border Radius: 12px
- Padding: 14px 18px
```

### Sidebar Navigation
```scss
- Background: linear-gradient(180deg, #2c3e50 0%, #34495e 100%)
- Active Item: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
- Active Border Left: 4px solid #fff
- Hover Background: rgba(255,255,255,0.1)
- Hover Transform: translateX(4px)
```

## 📊 Dashboard Stats Cards

### Product Card
- **Border Left**: #3498db (Blue)
- **Icon Background**: `linear-gradient(135deg, #3498db 0%, #2980b9 100%)`
- **Card Background**: `linear-gradient(135deg, rgba(52, 152, 219, 0.05) 0%, white 100%)`

### Warehouse Card
- **Border Left**: #2ecc71 (Green)
- **Icon Background**: `linear-gradient(135deg, #2ecc71 0%, #27ae60 100%)`
- **Card Background**: `linear-gradient(135deg, rgba(46, 204, 113, 0.05) 0%, white 100%)`

### Low Stock Card
- **Border Left**: #f39c12 (Orange)
- **Icon Background**: `linear-gradient(135deg, #f39c12 0%, #e67e22 100%)`
- **Card Background**: `linear-gradient(135deg, rgba(243, 156, 18, 0.05) 0%, white 100%)`

### Orders Card
- **Border Left**: #9b59b6 (Purple)
- **Icon Background**: `linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%)`
- **Card Background**: `linear-gradient(135deg, rgba(155, 89, 182, 0.05) 0%, white 100%)`

## 🎬 Animations

### Fade In
```scss
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
Duration: 0.4s ease-out
```

### Slide Up
```scss
@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}
Duration: 0.6s ease-out
```

### Pulse (Background)
```scss
@keyframes pulse {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(-10%, -10%) scale(1.1); }
}
Duration: 15s ease-in-out infinite
```

## 🔤 Typography

### Headings
- **Font Family**: 'Roboto', "Helvetica Neue", sans-serif
- **H1**: 2.5rem, font-weight 700, gradient text
- **H2**: 1.8rem, font-weight 600
- **H3**: 1.5rem, font-weight 600
- **H4**: 1.2rem, font-weight 500

### Body Text
- **Primary**: 1rem, #2c3e50
- **Secondary**: 0.95rem, #7f8c8d
- **Small**: 0.85rem, #95a5a6

### Font Weights
- **Light**: 300
- **Regular**: 400
- **Medium**: 500
- **Semi-Bold**: 600
- **Bold**: 700

## 🎯 Status Badges

### Active
```scss
- Color: #27ae60
- Background: rgba(39, 174, 96, 0.1)
- Padding: 6px 14px
- Border Radius: 16px
- Font Weight: 600
```

### Inactive
```scss
- Color: #e74c3c
- Background: rgba(231, 76, 60, 0.1)
- Padding: 6px 14px
- Border Radius: 16px
- Font Weight: 600
```

## 🔔 Toast Notifications

### Success
- **Background**: `linear-gradient(135deg, #11998e 0%, #38ef7d 100%)`

### Error
- **Background**: `linear-gradient(135deg, #eb3349 0%, #f45c43 100%)`

### Info
- **Background**: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`

### Warning
- **Background**: `linear-gradient(135deg, #f093fb 0%, #f5576c 100%)`

## 📱 Responsive Breakpoints

```scss
@media (max-width: 768px) {
  - Reduced padding
  - Single column layouts
  - Smaller font sizes
  - Simplified navigation
}
```

## ✨ Special Effects

### Scrollbar
- **Width**: 10px
- **Track**: #f1f3f5, border-radius 10px
- **Thumb**: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`
- **Hover**: Reversed gradient

### Box Shadows
- **Light**: `0 2px 4px rgba(0,0,0,0.08)`
- **Medium**: `0 4px 12px rgba(0,0,0,0.1)`
- **Heavy**: `0 8px 16px rgba(0,0,0,0.1)`
- **Extra Heavy**: `0 12px 24px rgba(0,0,0,0.15)`

### Hover Effects
- **Cards**: translateY(-2px to -4px)
- **Buttons**: translateY(-2px) + shadow increase
- **Nav Items**: translateX(4px)
- **Icons**: scale(1.1 to 1.2)

## 🎨 Design Principles

1. **Gradient-First**: Use gradients for primary elements to create depth
2. **Soft Shadows**: Subtle shadows for elevation hierarchy
3. **Smooth Transitions**: 0.3s ease for all interactive elements
4. **Rounded Corners**: 8px-16px for modern, friendly feel
5. **Consistent Spacing**: 8px grid system (8, 16, 24, 32, 40px)
6. **High Contrast**: Ensure text readability with proper contrast ratios
7. **Hover Feedback**: Always provide visual feedback on interactive elements
8. **Animation**: Subtle entrance animations for smooth UX

## 🖌️ Gradient Library

### Primary Gradients
```scss
Blue-Purple: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
Pink-Coral: linear-gradient(135deg, #f093fb 0%, #f5576c 100%)
Blue-Light: linear-gradient(135deg, #3498db 0%, #2980b9 100%)
Green-Emerald: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%)
Orange-Dark: linear-gradient(135deg, #f39c12 0%, #e67e22 100%)
Purple-Dark: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%)
```

### Success/Error Gradients
```scss
Success: linear-gradient(135deg, #11998e 0%, #38ef7d 100%)
Error: linear-gradient(135deg, #eb3349 0%, #f45c43 100%)
Warning: linear-gradient(135deg, #fa709a 0%, #fee140 100%)
```

### Background Gradients
```scss
Page: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)
Sidebar: linear-gradient(180deg, #2c3e50 0%, #34495e 100%)
```

## 📋 Usage Examples

### Creating a New Card
```html
<mat-card class="card-hover">
  <mat-card-header>
    <mat-card-title>Title</mat-card-title>
  </mat-card-header>
  <mat-card-content>Content</mat-card-content>
</mat-card>
```

### Creating a Status Badge
```html
<span class="status-active">Active</span>
<span class="status-inactive">Inactive</span>
```

### Using Gradient Text
```html
<h1 class="gradient-text">StockFlow Pro</h1>
```

---

**Last Updated**: April 22, 2026  
**Version**: 1.0  
**Design System**: Material Design 3 with custom theming

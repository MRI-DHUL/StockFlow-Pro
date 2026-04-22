# 🎯 StockFlow Pro - Frontend

Modern Angular 21 inventory management system with professional Material Design UI and gradient theming.

![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript)
![Material](https://img.shields.io/badge/Material-21-0081CB?logo=material-design)
![Status](https://img.shields.io/badge/Status-Production_Ready-success)

---

## ✨ Key Features

### 🎨 **Modern UI/UX**
- **Professional Gradient Theme** - Purple-blue gradient (#667eea → #764ba2) with accent colors
- **Smooth Animations** - Fade-in, slide-up, hover effects, and transitions
- **Component-Specific Themes** - Color-coded modules (green for warehouses, pink for suppliers, etc.)
- **Status Badges** - Gradient chips for order status, PO status, movement types
- **Responsive Design** - Mobile-friendly layouts with breakpoints

### 📦 **Core Modules**
- **Dashboard** - Real-time statistics with animated stat cards and notifications
- **Products** - Full CRUD with advanced filtering (search, category, price range, status)
- **Inventory** - Stock management with low stock alerts and pulsing indicators
- **Warehouses** - Multi-warehouse support with capacity tracking
- **Suppliers** - Supplier management with contact information
- **Orders** - Order tracking with 5 status types (Pending, Confirmed, Shipped, Delivered, Cancelled)
- **Purchase Orders** - PO management with supplier tracking
- **Stock Movements** - Complete stock movement history (In, Out, Transfer, Adjustment)
- **Audit Logs** - System activity tracking with action badges

### 🔐 **Security & Authentication**
- ✅ JWT Authentication with refresh tokens
- ✅ Auth guards for protected routes
- ✅ HTTP interceptors for token injection
- ✅ Secure login/register with validation
- ✅ Auto-logout on token expiration

### ⚡ **Technical Excellence**
- ✅ Real-time notifications (Pusher WebSockets)
- ✅ Material Design 21 components
- ✅ Standalone components architecture
- ✅ Lazy loading for performance
- ✅ RxJS observables & signals
- ✅ Toast notifications (ngx-toastr)
- ✅ Pagination & advanced filtering
- ✅ Form validation with reactive forms
- ✅ TypeScript strict mode

---

## 📋 Prerequisites

- **Node.js** 20+ and npm
- **Angular CLI** 21+
- **Backend API** running at https://stockflow-pro.up.railway.app

---

## 🚀 Quick Start

### Installation
```bash
# Navigate to frontend directory
cd application/frontend

# Install dependencies
npm install

# Start development server
npm start
```

Access the app at `http://localhost:4200`

### Default Credentials
```
Email: admin@stockflow.com
Password: Admin@123
```

---

## 🔧 Configuration

### Environment Files

#### **Development** (`src/environments/environment.ts`)
export const environment = {
  production: false,
  apiUrl: 'https://stockflow-pro.up.railway.app/api/v1', // Uses deployed backend
  pusher: {
    appKey: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    channel: 'stockflow-notifications'
  }
};
```

**Production** (`src/environments/environment.prod.ts`)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://stockflow-pro.up.railway.app/api/v1', // Uses deployed backend
  pusher: {
    appKey: '3a6db031b2381d1e78ec',
    cluster: 'ap2',
    channel: 'stockflow-notifications'
  }
};
```

> **Note**: The frontend uses the deployed backend on Railway, so you don't need to run the backend locally!

## 🏃‍♂️ Running the Application

### Development Mode
```bash
npm start
# Opens at http://localhost:4200 (or next available port)
```

> **✅ Backend Already Running**: The app connects to the deployed backend at https://stockflow-pro.up.railway.app  
> No need to run backend locally!

### Production Build
```bash
npm run build
# Output in dist/ folder
```

## 📁 Project Structure

```
src/
├── app/
│   ├── core/                 # Core services, guards, interceptors
│   │   ├── services/         # API services (8 services)
│   │   ├── guards/           # Route guards
│   │   ├── interceptors/     # HTTP interceptors
│   │   └── models/           # Core models
│   ├── features/             # Feature modules (lazy loaded)
│   │   ├── auth/            # Login & Register
│   │   ├── dashboard/       # Dashboard with stats
│   │   ├── products/        # Product management
│   │   ├── inventory/       # Inventory management
│   │   ├── warehouses/      # Warehouse management
│   │   ├── suppliers/       # Supplier management
│   │   ├── orders/          # Order management
│   │   ├── purchase-orders/ # PO management
│   │   ├── stock-movements/ # Stock movements
│   │   └── audit-logs/      # Audit logs
│   └── shared/              # Shared components & utilities
│       ├── components/      # Reusable components
│       ├── models/          # Domain models
│       └── pipes/           # Custom pipes
├── environments/            # Environment configs
└── styles.scss             # Global styles & Material theme
```

## 🔐 Authentication

Default credentials (from backend):
- **Email**: admin@stockflowpro.com
- **Password**: Admin@123

## 🎨 UI Components

Built with Angular Material:
- Mat-Table with sorting & pagination
- Mat-Dialog for forms
- Mat-Sidenav for navigation
- **Material Icons & Symbols** throughout the app

### Material Icons
The app uses both Material Icons and Material Symbols:
- **Icons**: Standard filled icons (e.g., `<mat-icon>home</mat-icon>`)
- **Symbols**: Outlined symbols with variable font (configurable weight, fill, etc.)

```html
<!-- Material Icon -->
<mat-icon>dashboard</mat-icon>

<!-- Material Symbol Outlined -->
<span class="material-symbols-outlined">inventory_2</span>
``` sections
- Mat-Form-Field for inputs
- Mat-Chips for status indicators
- Mat-Icons throughout

## 🔔 Real-Time Notifications

The app uses Pusher for real-time notifications:
- Low stock alerts
- Order updates
- Stock movement notifications

## 🧪 Testing

```bash
# Run unit tests
npm test
```

## 📦 Build & Deploy

```bash
# Production build
npm run build

# Output will be in dist/stockflow-app/
# Deploy the contents to your web server
```

## 🛡️ Security
The app uses the deployed backend at: https://stockflow-pro.up.railway.app

If you experience connection issues:
1. Check if backend is running: https://stockflow-pro.up.railway.app/health
2. Verify CORS is enabled on backend
3. Check browser console for errors
4. Clear browser cache and localStorage
- Protected routes with auth guards
- HTTP interceptor for token injection
- Token expiration handling

## 🐛 Troubleshooting

### Port already in use
Angular will prompt to use a different port automatically.

### API Connection Issues
1. Check backend is running at http://localhost:5057
2. Verify environment.ts has correct API URL
3. Check CORS settings in backend

### Build Errors
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
```

## 📝 Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run unit tests
- `npm run watch` - Build and watch for changes

## 🎨 Design System

### Color Palette
- **Primary Gradient**: #667eea → #764ba2 (Purple-Blue)
- **Accent Gradient**: #f093fb → #f5576c (Pink-Coral)
- **Success**: #27ae60 (Green)
- **Warning**: #f39c12 (Orange)
- **Error**: #e74c3c (Red)
- **Info**: #3498db (Blue)

### Component Themes
- **Products/Inventory**: Purple-Blue gradient
- **Warehouses**: Green gradient
- **Suppliers**: Pink gradient
- **Orders**: Purple gradient
- **Purchase Orders**: Blue gradient
- **Stock Movements**: Teal gradient
- **Audit Logs**: Dark gray gradient

### Typography
- **Font Family**: 'Roboto', sans-serif
- **Headings**: 1.8rem - 2.5rem, weights 600-700
- **Body**: 0.95rem - 1rem, weight 400-500

---

## 🔔 Real-Time Notifications

The app uses **Pusher WebSockets** for real-time updates:
- **Low stock alerts** when inventory reaches threshold
- **Order status updates** when orders change state
- **Stock movement notifications** for inventory changes
- **System notifications** for important events

```typescript
Pusher Configuration:
- Key: 3a6db031b2381d1e78ec
- Cluster: ap2
- Channel: stockflow-notifications
```

---

## 🧪 Testing

```bash
# Run unit tests
npm test

# Run tests in watch mode
npm run test:watch

# Run tests with coverage
npm run test:coverage
```

---

## 📦 Build & Deploy

### Production Build
```bash
npm run build
# Output: dist/stockflow-app/browser/
```

### Deployment Platforms
- ✅ **Vercel** (Recommended) - Zero-config deployment
- ✅ **Netlify** - Easy static hosting
- ✅ **Azure Static Web Apps**
- ✅ **Firebase Hosting**

See [FRONTEND_DEPLOYMENT.md](FRONTEND_DEPLOYMENT.md) for detailed deployment instructions.

---

## 🛡️ Security Features

- ✅ **JWT Authentication** with access & refresh tokens
- ✅ **Route Guards** protecting authenticated routes
- ✅ **HTTP Interceptors** for automatic token injection
- ✅ **Token Expiration** handling with auto-logout
- ✅ **CORS** properly configured
- ✅ **Input Validation** on all forms
- ✅ **XSS Protection** via Angular sanitization

---

## 🐛 Troubleshooting

### Backend Connection Issues
**Problem**: API calls failing  
**Solution**:
1. Check backend health: https://stockflow-pro.up.railway.app/health
2. Verify environment.ts has correct API URL
3. Check browser console for CORS errors
4. Clear localStorage and try again

### Port Already in Use
**Problem**: Port 4200 is occupied  
**Solution**: Angular CLI will automatically prompt for an alternative port

### Build Errors
**Problem**: TypeScript or dependency errors  
**Solution**:
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json dist
npm install
npm run build
```

### Icons Not Showing
**Problem**: Material Icons not displaying  
**Solution**: Verify CDN links in `index.html`:
```html
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet">
```

### Authentication Issues
**Problem**: Login not working or token expired  
**Solution**:
1. Clear browser localStorage: `localStorage.clear()`
2. Check backend is running and accessible
3. Verify credentials (admin@stockflowpro.com / Admin@123)
4. Check Network tab for API response

---

## 📝 Available Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Start development server (port 4200) |
| `npm run build` | Build for production |
| `npm test` | Run unit tests |
| `npm run watch` | Build and watch for changes |
| `npm run lint` | Lint TypeScript files |

---

## 📚 Documentation

### Core Documentation
- **[UI_ENHANCEMENT_SUMMARY.md](UI_ENHANCEMENT_SUMMARY.md)** - Complete UI design overview
- **[DESIGN_GUIDE.md](DESIGN_GUIDE.md)** - Design system reference
- **[UI_QUICK_REFERENCE.md](UI_QUICK_REFERENCE.md)** - Developer quick reference
- **[FRONTEND_DEPLOYMENT.md](FRONTEND_DEPLOYMENT.md)** - Deployment guide

### Backend Documentation
- **Backend API**: ../backend/README.md
- **API Swagger**: https://stockflow-pro.up.railway.app/swagger
- **Health Check**: https://stockflow-pro.up.railway.app/health

---

## 🎯 Feature Highlights

### Dashboard
- 4 animated stat cards (Products, Warehouses, Low Stock, Orders)
- Recent notifications list with real-time updates
- Quick action buttons for common tasks
- Color-coded gradients per metric

### Product Management
- Advanced filtering (search, category, price range, status)
- Inline editing via dialogs
- Status badges (Active/Inactive)
- Pagination with configurable page size

### Inventory Management
- Real-time stock levels
- Low stock indicators with pulsing animation
- Warehouse-specific filtering
- Quantity on hand, reserved, and available

### Order Tracking
- 5 status types with gradient chips
- Date range filtering
- Order details with line items
- Status history

---

## 🚀 Performance

### Optimization Techniques
- ✅ **Lazy Loading** - Feature modules loaded on demand
- ✅ **OnPush Change Detection** - Reduced change detection cycles
- ✅ **Tree Shaking** - Unused code removed
- ✅ **AOT Compilation** - Ahead-of-time compilation
- ✅ **Production Build** - Minified and optimized bundles

### Bundle Sizes (Production)
- Main bundle: ~500KB (gzipped: ~150KB)
- Vendor bundle: ~200KB (gzipped: ~60KB)
- Total initial load: ~700KB (gzipped: ~210KB)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

MIT License - see LICENSE file for details

---

## 👥 Authors

- **StockFlow Pro Development Team**

---

## 🙏 Acknowledgments

- **Angular Team** - For the amazing framework
- **Material Design** - For the beautiful components
- **Pusher** - For real-time capabilities
- **Railway** - For backend hosting

---

## 📞 Support

For issues or questions:
- 📧 Email: support@stockflowpro.com
- 🐛 Issues: GitHub Issues
- 📖 Docs: See documentation links above

---

**Status**: ✅ Production Ready  
**Version**: 1.0.0  
**Last Updated**: April 22, 2026  
**Angular**: 21.2.6  
**Material**: 21.2.6  
**TypeScript**: 5.9.2


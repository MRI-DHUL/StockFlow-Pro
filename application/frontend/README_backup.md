# StockFlow Pro - Frontend

---

## 📌 Overview

Modern Angular application with Tailwind CSS providing a responsive, real-time dashboard for inventory and supply chain management.

---

## ⚙️ Tech Stack

### Core Framework
* **Angular 17+** - Modern web framework
* **TypeScript** - Type-safe development
* **RxJS** - Reactive programming

### UI & Styling
* **Tailwind CSS** - Utility-first CSS framework
* **Angular Material** (optional) - Component library
* **Chart.js** / **ApexCharts** - Data visualization
* **Heroicons** - Icon library

### Real-time Communication
* **Pusher** - WebSocket-based real-time updates
* Live inventory updates
* Instant notifications
* Real-time dashboard metrics

### State Management
* **RxJS BehaviorSubjects** - Simple state management
* **NgRx** (optional) - Advanced state management for scaling

### HTTP Communication
* **Angular HttpClient** - API communication
* **Interceptors** - JWT token injection, error handling

### Authentication
* **JWT** - Token-based authentication
* **Auth Guards** - Route protection
* **Role-based access** - Admin, Manager, Viewer roles

---

## 🏗️ Architecture

### Project Structure

```
/frontend
  /src
    /app
      /core                    → Singleton services, guards, interceptors
        /services
          auth.service.ts
          api.service.ts
        /guards
          auth.guard.ts
          role.guard.ts
        /interceptors
          jwt.interceptor.ts
          error.interceptor.ts
      /shared                  → Shared components, pipes, directives
        /components
          navbar/
          sidebar/
          notification-toast/
        /pipes
        /directives
      /features                → Feature modules
        /dashboard
        /products
        /inventory
        /orders
        /suppliers
        /warehouses
        /predictions
        /settings
      /models                  → TypeScript interfaces and types
      /environments            → Environment configurations
    /assets                    → Static files (images, icons)
    /styles                    → Global styles
      styles.scss
      tailwind.css
```

### Design Patterns
* **Feature Modules** - Lazy-loaded modules per feature
* **Smart/Dumb Components** - Container and presentation components
* **Service Layer** - Business logic in services
* **Reactive Forms** - Form handling with validation
* **Guards** - Route protection and authorization

---

## 🎨 Features

### 1. Dashboard
* **Real-time metrics** - Live inventory levels, order counts
* **Charts & graphs** - Stock trends, order analytics
* **Low stock alerts** - Visual indicators for items below threshold
* **Recent activity feed** - Latest orders and stock movements
* **Pusher integration** - Instant updates without refresh

### 2. Product Management
* **Product listing** - Searchable, filterable table
* **CRUD operations** - Create, Read, Update, Delete products
* **Product details** - SKU, category, pricing, stock levels
* **Bulk import** - CSV upload for multiple products

### 3. Inventory Management
* **Multi-warehouse view** - Stock levels across all warehouses
* **Stock transfers** - Move inventory between locations
* **Stock adjustments** - Manual stock level corrections
* **Low stock notifications** - Real-time alerts via Pusher
* **Inventory history** - Stock movement timeline

### 4. Order Management
* **Order creation** - Create new sales orders
* **Order tracking** - Status updates and timeline
* **Order details** - Line items, totals, customer info
* **Order fulfillment** - Status progression workflow
* **Real-time updates** - Live order status changes

### 5. Supplier Management
* **Supplier directory** - Contact information, lead times
* **Purchase orders** - Create and manage POs
* **Supplier performance** - Delivery metrics

### 6. Prediction Engine UI
* **Demand forecast charts** - Visual demand predictions
* **Depletion estimates** - "Out of stock in X days"
* **Reorder recommendations** - Suggested purchase quantities

### 7. Notifications
* **Real-time toast notifications** - Pusher-powered alerts
* **Notification center** - Historical notification log
* **Configurable alerts** - User preferences for notifications

### 8. User Management
* **Login/Logout** - JWT authentication
* **User profile** - Account settings
* **Role-based UI** - Show/hide features by role

---

## 🚀 Getting Started

### Prerequisites
* **Node.js** 18+ and npm
* **Angular CLI** 17+
* Backend API running (see backend README)

### Installation

1. **Navigate to frontend**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   Update `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:5001/api',
     pusher: {
       key: 'your-pusher-key',
       cluster: 'your-cluster'
     }
   };
   ```

4. **Run development server**
   ```bash
   ng serve
   ```

5. **Access application**
   Navigate to: `http://localhost:4200`

### Development Commands

```bash
# Start dev server
ng serve

# Build for production
ng build --configuration production

# Run unit tests
ng test

# Run e2e tests
ng e2e

# Generate component
ng generate component features/products/product-list

# Generate service
ng generate service core/services/inventory

# Lint code
ng lint

# Format code (with Prettier)
npm run format
```

---

## 🔌 API Integration

### HTTP Service Structure

```typescript
// core/services/api.service.ts
@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`);
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data);
  }
  // ... put, delete methods
}
```

### Feature Services

```typescript
// features/products/services/product.service.ts
@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private api: ApiService) {}

  getProducts(): Observable<Product[]> {
    return this.api.get<Product[]>('products');
  }

  createProduct(product: Product): Observable<Product> {
    return this.api.post<Product>('products', product);
  }
  // ... other methods
}
```

---

## 🔔 Real-time Features (Pusher)

### Pusher Service

```typescript
// core/services/pusher.service.ts
import Pusher from 'pusher-js';

@Injectable({ providedIn: 'root' })
export class PusherService {
  private pusher: Pusher;

  constructor() {
    this.pusher = new Pusher(environment.pusher.key, {
      cluster: environment.pusher.cluster
    });
  }

  subscribe(channelName: string) {
    return this.pusher.subscribe(channelName);
  }

  bind(channel: any, eventName: string, callback: Function) {
    channel.bind(eventName, callback);
  }
}
```

### Usage Example

```typescript
// features/dashboard/dashboard.component.ts
export class DashboardComponent implements OnInit {
  constructor(private pusherService: PusherService) {}

  ngOnInit() {
    const channel = this.pusherService.subscribe('inventory-channel');
    
    this.pusherService.bind(channel, 'low-stock-alert', (data: any) => {
      this.showNotification(`Low stock alert: ${data.productName}`);
      this.refreshInventory();
    });
  }
}
```

### Pusher Channels & Events

| Channel | Event | Purpose |
|---------|-------|---------|
| `inventory-channel` | `low-stock-alert` | Stock below threshold |
| `inventory-channel` | `stock-updated` | Inventory level changed |
| `order-channel` | `order-placed` | New order created |
| `order-channel` | `order-status-changed` | Order status updated |
| `notification-channel` | `general-notification` | System notifications |

---

## 🎨 Styling with Tailwind CSS

### Configuration

```javascript
// tailwind.config.js
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        primary: '#3B82F6',
        secondary: '#10B981',
        danger: '#EF4444',
        warning: '#F59E0B'
      }
    }
  },
  plugins: []
};
```

### Component Styling Example

```html
<!-- Product card -->
<div class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
  <h3 class="text-xl font-bold text-gray-800">{{ product.name }}</h3>
  <p class="text-sm text-gray-500">SKU: {{ product.sku }}</p>
  <div class="mt-4 flex justify-between items-center">
    <span class="text-2xl font-semibold text-primary">${{ product.price }}</span>
    <button class="bg-primary text-white px-4 py-2 rounded hover:bg-blue-600">
      Edit
    </button>
  </div>
</div>
```

---

## 🔐 Authentication & Authorization

### Auth Service

```typescript
// core/services/auth.service.ts
@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  login(credentials: LoginDto): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/login', credentials).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        this.currentUserSubject.next(response.user);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }
}
```

### Auth Guard

```typescript
// core/guards/auth.guard.ts
@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.auth.isAuthenticated()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}
```

### Route Protection

```typescript
// app-routing.module.ts
const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  // ... other routes
];
```

---

## 📊 State Management

### Simple State with BehaviorSubject

```typescript
// features/inventory/services/inventory-state.service.ts
@Injectable({ providedIn: 'root' })
export class InventoryStateService {
  private inventorySubject = new BehaviorSubject<InventoryItem[]>([]);
  public inventory$ = this.inventorySubject.asObservable();

  updateInventory(items: InventoryItem[]): void {
    this.inventorySubject.next(items);
  }

  getInventory(): InventoryItem[] {
    return this.inventorySubject.getValue();
  }
}
```

---

## 🧪 Testing

### Unit Testing (Jasmine/Karma)

```typescript
// product-list.component.spec.ts
describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let productService: jasmine.SpyObj<ProductService>;

  beforeEach(() => {
    const productServiceSpy = jasmine.createSpyObj('ProductService', ['getProducts']);
    
    TestBed.configureTestingModule({
      declarations: [ProductListComponent],
      providers: [
        { provide: ProductService, useValue: productServiceSpy }
      ]
    });

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
  });

  it('should load products on init', () => {
    const mockProducts = [{ id: '1', name: 'Test Product' }];
    productService.getProducts.and.returnValue(of(mockProducts));
    
    component.ngOnInit();
    
    expect(component.products).toEqual(mockProducts);
  });
});
```

### E2E Testing (Cypress/Protractor)

```typescript
// e2e/products.e2e-spec.ts
describe('Product Management', () => {
  it('should display product list', () => {
    cy.visit('/products');
    cy.get('.product-card').should('have.length.greaterThan', 0);
  });

  it('should create new product', () => {
    cy.visit('/products/new');
    cy.get('#product-name').type('New Product');
    cy.get('#product-sku').type('SKU001');
    cy.get('button[type="submit"]').click();
    cy.contains('Product created successfully');
  });
});
```

---

## 🚀 Build & Deployment

### Production Build

```bash
# Build with production configuration
ng build --configuration production

# Output location
dist/stockflow-frontend/
```

### Deployment to InfinityFree

1. **Build the application**
   ```bash
   ng build --configuration production --base-href /
   ```

2. **Upload to hosting**
   - Upload contents of `dist/stockflow-frontend/` to `htdocs/` folder
   - Ensure `.htaccess` is configured for Angular routing

3. **`.htaccess` configuration**
   ```apache
   <IfModule mod_rewrite.c>
     RewriteEngine On
     RewriteBase /
     RewriteRule ^index\.html$ - [L]
     RewriteCond %{REQUEST_FILENAME} !-f
     RewriteCond %{REQUEST_FILENAME} !-d
     RewriteRule . /index.html [L]
   </IfModule>
   ```

### Environment Configuration

```typescript
// environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-backend-url.azurewebsites.net/api',
  pusher: {
    key: 'your-production-pusher-key',
    cluster: 'your-cluster'
  }
};
```

---

## 📱 Responsive Design

### Breakpoints (Tailwind)
* **sm**: 640px - Mobile
* **md**: 768px - Tablet
* **lg**: 1024px - Desktop
* **xl**: 1280px - Large screens

### Example Responsive Component

```html
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  <!-- Cards automatically adjust based on screen size -->
  <div *ngFor="let product of products" class="product-card">
    {{ product.name }}
  </div>
</div>
```

---

## 🛠️ Development Best Practices

### Code Style
* Use **Prettier** for code formatting
* Follow **Angular Style Guide**
* Use **ESLint** for linting
* Write meaningful component and variable names

### Component Design
* Keep components focused and small
* Use **@Input()** and **@Output()** for component communication
* Prefer **OnPush** change detection for performance
* Unsubscribe from observables in **ngOnDestroy**

### Performance Optimization
* Lazy load feature modules
* Use **trackBy** in *ngFor loops
* Optimize images (WebP format)
* Use **async** pipe for automatic subscription management
* Implement virtual scrolling for large lists

---

## 🐛 Debugging

### Angular DevTools
* Install Angular DevTools browser extension
* Inspect component tree
* Profile change detection
* View injector hierarchy

### Common Issues

**CORS errors**
- Verify backend CORS configuration
- Check API URL in environment files

**Pusher not connecting**
- Verify Pusher credentials
- Check browser console for errors
- Ensure channel names match backend

**Route not found after refresh**
- Add `.htaccess` for Apache
- Configure server for HTML5 routing

---

## 📚 Additional Resources

* [Angular Documentation](https://angular.io/docs)
* [Tailwind CSS Docs](https://tailwindcss.com/docs)
* [Pusher Angular Guide](https://pusher.com/docs/channels/getting_started/angular)
* [RxJS Documentation](https://rxjs.dev/)
* [Angular Material](https://material.angular.io/)

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

```yaml
# .github/workflows/frontend-ci.yml
name: Frontend CI/CD
on:
  push:
    branches: [main, develop]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup Node.js
        uses: actions/setup-node@v2
        with:
          node-version: '18'
      - name: Install dependencies
        run: npm ci
      - name: Run tests
        run: npm test -- --watch=false --browsers=ChromeHeadless
      - name: Build
        run: npm run build -- --configuration production
      - name: Deploy
        run: # deployment script
```

---

## 📄 License

This project is part of StockFlow Pro portfolio demonstration.

---

**Frontend Version**: 1.0.0  
**Last Updated**: April 2026

import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { PusherService, PusherNotification } from '../../core/services/pusher.service';
import { ProductService } from '../../core/services/product.service';
import { InventoryService } from '../../core/services/inventory.service';
import { OrderService } from '../../core/services/order.service';
import { WarehouseService } from '../../core/services/warehouse.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly pusherService = inject(PusherService);
  private readonly productService = inject(ProductService);
  private readonly inventoryService = inject(InventoryService);
  private readonly orderService = inject(OrderService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);

  currentUser = this.authService.currentUser;
  notifications: PusherNotification[] = [];
  
  stats = {
    totalProducts: 0,
    totalWarehouses: 0,
    lowStockItems: 0,
    pendingOrders: 0
  };

  ngOnInit(): void {
    this.loadStats();
    
    // Subscribe to Pusher notifications
    this.pusherService.notifications$.subscribe(notification => {
      this.notifications = [notification, ...this.notifications].slice(0, 10);
      this.toastr.info(notification.message, 'Notification');
    });
  }

  loadStats(): void {
    // Load products count
    this.productService.getPaged({ pageSize: 1, pageNumber: 1 }).subscribe({
      next: (response) => {
        this.stats.totalProducts = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load products stats', error)
    });

    // Load warehouses count
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.stats.totalWarehouses = warehouses.length;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load warehouses stats', error)
    });

    // Load low stock items
    this.inventoryService.getLowStock().subscribe({
      next: (items) => {
        this.stats.lowStockItems = items.length;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load low stock stats', error)
    });

    // Load pending orders count
    this.orderService.getPaged({ status: 0, pageSize: 1, pageNumber: 1 }).subscribe({
      next: (response) => {
        this.stats.pendingOrders = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load orders stats', error)
    });
  }

  getNotificationIcon(type: string): string {
    const iconMap: { [key: string]: string } = {
      'info': 'info',
      'success': 'check_circle',
      'warning': 'warning',
      'error': 'error',
      'low-stock': 'inventory',
      'order': 'shopping_cart'
    };
    return iconMap[type] || 'notifications';
  }
}
            <span class="card-trend neutral">
              <mat-icon>remove</mat-icon>
              No change
            </span>
          </div>
        </mat-card>

        <mat-card class="stat-card low-stock-card">
          <div class="card-icon-wrapper">
            <mat-icon class="card-icon">warning</mat-icon>
          </div>
          <div class="card-content">
            <p class="card-label">Low Stock Alerts</p>
            <h2 class="card-value">{{ stats.lowStockItems }}</h2>
            <span class="card-trend negative">
              <mat-icon>trending_down</mat-icon>
              Needs attention
            </span>
          </div>
        </mat-card>

        <mat-card class="stat-card orders-card">
          <div class="card-icon-wrapper">
            <mat-icon class="card-icon">shopping_cart</mat-icon>
          </div>
          <div class="card-content">
            <p class="card-label">Pending Orders</p>
            <h2 class="card-value">{{ stats.pendingOrders }}</h2>
            <span class="card-trend positive">
              <mat-icon>trending_up</mat-icon>
              +8% this week
            </span>
          </div>
        </mat-card>
      </div>

      <div class="dashboard-grid">
        <!-- Quick Actions -->
        <mat-card class="quick-actions-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>bolt</mat-icon>
              Quick Actions
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="actions-list">
              <button mat-stroked-button class="action-btn" routerLink="/products">
                <mat-icon>add_circle_outline</mat-icon>
                <span>Add New Product</span>
              </button>
              <button mat-stroked-button class="action-btn" routerLink="/inventory">
                <mat-icon>edit</mat-icon>
                <span>Update Inventory</span>
              </button>
              <button mat-stroked-button class="action-btn" routerLink="/orders">
                <mat-icon>receipt</mat-icon>
                <span>Create Order</span>
              </button>
              <button mat-stroked-button class="action-btn" routerLink="/stock-movements">
                <mat-icon>swap_horiz</mat-icon>
                <span>Transfer Stock</span>
              </button>
            </div>
          </mat-card-content>
        </mat-card>

        <!-- Recent Notifications -->
        <mat-card class="notifications-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>notifications</mat-icon>
              Recent Activity
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div *ngIf="notifications.length === 0" class="empty-state">
              <mat-icon>inbox</mat-icon>
              <p>No recent notifications</p>
            </div>
            <div class="notifications-list">
              <div *ngFor="let notification of notifications" class="notification-item">
                <div class="notification-icon" [ngClass]="notification.type">
                  <mat-icon>{{ getNotificationIcon(notification.type) }}</mat-icon>
                </div>
                <div class="notification-details">
                  <p class="notification-message">{{ notification.message }}</p>
                  <span class="notification-time">{{ notification.timestamp | date:'short' }}</span>
                </div>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .dashboard {
      animation: fadeIn 0.4s ease-out;
      max-width: 1600px;
      margin: 0 auto;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }

    .dashboard-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 32px;
      padding-bottom: 24px;
      border-bottom: 2px solid #e5e7eb;
    }

    .header-content h1 {
      font-size: 2.2rem;
      font-weight: 700;
      color: #1a1d29;
      margin: 0 0 8px 0;
      letter-spacing: -0.5px;
    }

    .subtitle {
      font-size: 1rem;
      color: #6b7280;
      margin: 0;
    }

    .header-actions button {
      height: 48px;
      padding: 0 28px;
      font-weight: 600;
      border-radius: 10px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
      margin-bottom: 32px;
    }

    .stat-card {
      position: relative;
      overflow: hidden;
      border-radius: 16px;
      border: 1px solid #e5e7eb;
      transition: all 0.3s ease;
      cursor: default;
      display: flex;
      align-items: center;
      gap: 20px;
      padding: 28px;
      background: white;
    }

    .stat-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 28px rgba(0,0,0,0.12);
    }

    .products-card {
      border-left: 4px solid #3b82f6;
    }

    .warehouses-card {
      border-left: 4px solid #10b981;
    }

    .low-stock-card {
      border-left: 4px solid #f59e0b;
    }

    .orders-card {
      border-left: 4px solid #8b5cf6;
    }

    .card-icon-wrapper {
      width: 64px;
      height: 64px;
      border-radius: 14px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .products-card .card-icon-wrapper {
      background: #000000;
    }

    .warehouses-card .card-icon-wrapper {
      background: white;
      border: 2px solid #000000;
    }

    .low-stock-card .card-icon-wrapper {
      background: #000000;
    }

    .orders-card .card-icon-wrapper {
      background: white;
      border: 2px solid #000000;
    }

    .card-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
    }

    .products-card .card-icon { color: white !important; }
    .warehouses-card .card-icon { color: #000000; }
    .low-stock-card .card-icon { color: white !important; }
    .orders-card .card-icon { color: #000000; }

    .card-content {
      flex: 1;
    }

    .card-label {
      font-size: 0.85rem;
      font-weight: 600;
      color: #6b7280;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      margin: 0 0 8px 0;
    }

    .card-value {
      font-size: 2.2rem;
      font-weight: 700;
      color: #1a1d29;
      margin: 0 0 8px 0;
      line-height: 1;
    }

    .card-trend {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8rem;
      font-weight: 600;
      padding: 4px 10px;
      border-radius: 12px;
    }

    .card-trend mat-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
    }

    .card-trend.positive {
      color: #000000;
      background: white;
      border: 2px solid #000000;
    }

    .card-trend.neutral {
      color: white !important;
      background: #000000;
    }

    .card-trend.negative {
      color: #000000;
      background: white;
      border: 2px solid #000000;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: 400px 1fr;
      gap: 24px;
      margin-top: 32px;
    }

    .quick-actions-card,
    .notifications-card {
      border-radius: 16px;
      border: 1px solid #e5e7eb;
    }

    mat-card-header {
      margin-bottom: 20px;
    }

    mat-card-title {
      display: flex;
      align-items: center;
      gap: 10px;
      font-size: 1.1rem;
      font-weight: 700;
      color: #1a1d29;
      
      mat-icon {
        color: #667eea;
      }
    }

    .actions-list {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .action-btn {
      width: 100%;
      height: 54px;
      justify-content: flex-start;
      padding: 0 20px;
      font-size: 0.95rem;
      font-weight: 600;
      border-radius: 10px;
      border: 2px solid #e5e7eb;
      transition: all 0.3s ease;
      
      mat-icon {
        margin-right: 12px;
        color: #6b7280;
      }

      span {
        color: #1a1d29;
      }
    }

    .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: #9ca3af;
      
      mat-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        margin-bottom: 16px;
        opacity: 0.3;
      }
      
      p {
        font-size: 1rem;
        margin: 0;
      }
    }

    .notifications-list {
      max-height: 500px;
      overflow-y: auto;
    }

    .notification-item {
      display: flex;
      gap: 16px;
      padding: 16px;
      border-bottom: 1px solid #f3f4f6;
      transition: all 0.2s ease;
    }

    .notification-item:last-child {
      border-bottom: none;
    }

    .notification-item:hover {
      background: #fafafa;
    }

    .notification-icon {
      width: 40px;
      height: 40px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      
      mat-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
      }

      &.info {
        background: #000000;
        color: white !important;
      }

      &.success {
        background: white;
        color: #000000;
        border: 2px solid #000000;
      }

      &.warning {
        background: #000000;
        color: white !important;
      }

      &.error {
        background: #fee2e2;
        color: #dc2626;
      }

      &.low-stock {
        background: #fed7aa;
        color: #d97706;
      }

      &.order {
        background: #e9d5ff;
        color: #7c3aed;
      }
    }

    .notification-details {
      flex: 1;
      min-width: 0;
    }

    .notification-message {
      font-size: 0.9rem;
      font-weight: 500;
      color: #1a1d29;
      margin: 0 0 4px 0;
    }

    .notification-time {
      font-size: 0.8rem;
      color: #9ca3af;
    }

    @media (max-width: 1200px) {
      .dashboard-grid {
        grid-template-columns: 1fr;
      }
    }

    @media (max-width: 768px) {
      .dashboard-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 16px;
      }

      .header-content h1 {
        font-size: 1.8rem;
      }

      .stats-grid {
        grid-template-columns: 1fr;
      }

      .stat-card {
        padding: 20px;
      }

      .card-icon-wrapper {
        width: 52px;
        height: 52px;
      }

      .card-icon {
        font-size: 28px;
        width: 28px;
        height: 28px;
      }

      .card-value {
        font-size: 1.8rem;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly pusherService = inject(PusherService);
  private readonly productService = inject(ProductService);
  private readonly inventoryService = inject(InventoryService);
  private readonly orderService = inject(OrderService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);

  currentUser = this.authService.currentUser;
  notifications: PusherNotification[] = [];
  
  stats = {
    totalProducts: 0,
    totalWarehouses: 0,
    lowStockItems: 0,
    pendingOrders: 0
  };

  ngOnInit(): void {
    this.loadStats();
    
    // Subscribe to Pusher notifications
    this.pusherService.notifications$.subscribe(notification => {
      this.notifications = [notification, ...this.notifications].slice(0, 10);
      this.toastr.info(notification.message, 'Notification');
    });
  }

  loadStats(): void {
    // Load products count
    this.productService.getPaged({ pageSize: 1, pageNumber: 1 }).subscribe({
      next: (response) => {
        this.stats.totalProducts = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load products stats', error)
    });

    // Load warehouses count
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.stats.totalWarehouses = warehouses.length;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load warehouses stats', error)
    });

    // Load low stock items
    this.inventoryService.getLowStock().subscribe({
      next: (items) => {
        this.stats.lowStockItems = items.length;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load low stock stats', error)
    });

    // Load pending orders count
    this.orderService.getPaged({ status: 0, pageSize: 1, pageNumber: 1 }).subscribe({
      next: (response) => {
        this.stats.pendingOrders = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load orders stats', error)
    });
  }

  getNotificationIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'info': 'info',
      'success': 'check_circle',
      'warning': 'warning',
      'error': 'error',
      'low-stock': 'inventory',
      'order': 'shopping_cart'
    };
    return icons[type] || 'notifications';
  }
}

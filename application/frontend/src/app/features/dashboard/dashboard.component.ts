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

import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../core/services/auth.service';
import { PusherService, PusherNotification } from '../../core/services/pusher.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    MatMenuModule
  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly pusherService = inject(PusherService);
  private readonly router = inject(Router);
  
  notifications: (PusherNotification & { viewed: boolean })[] = [];
  unreadCount = 0;
  private notificationSubscription?: Subscription;

  get currentUser() {
    return this.authService.currentUser;
  }

  ngOnInit(): void {
    // Subscribe to Pusher notifications
    this.notificationSubscription = this.pusherService.notifications$.subscribe(notification => {
      const notificationWithViewed = { ...notification, viewed: false };
      this.notifications = [notificationWithViewed, ...this.notifications].slice(0, 10);
      this.updateUnreadCount();
    });
  }

  ngOnDestroy(): void {
    this.notificationSubscription?.unsubscribe();
  }

  markAllAsViewed(): void {
    this.notifications.forEach(n => n.viewed = true);
    this.updateUnreadCount();
  }

  updateUnreadCount(): void {
    this.unreadCount = this.notifications.filter(n => !n.viewed).length;
  }

  getNotificationIcon(type: string): string {
    const iconMap: { [key: string]: string } = {
      'info': 'info',
      'success': 'check_circle',
      'warning': 'warning',
      'error': 'error'
    };
    return iconMap[type] || 'notifications';
  }

  getTimeAgo(timestamp: Date): string {
    const now = new Date();
    const notificationTime = new Date(timestamp);
    const diffMs = now.getTime() - notificationTime.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    
    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    
    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays}d ago`;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  getPageTitle(): string {
    const path = this.router.url.split('/')[1] || 'dashboard';
    const titles: { [key: string]: string } = {
      'dashboard': 'Dashboard',
      'products': 'Products Management',
      'inventory': 'Inventory Management',
      'warehouses': 'Warehouses',
      'suppliers': 'Suppliers',
      'orders': 'Orders',
      'purchase-orders': 'Purchase Orders',
      'stock-movements': 'Stock Movements',
      'audit-logs': 'Audit Logs'
    };
    return titles[path] || 'StockFlow Pro';
  }
}

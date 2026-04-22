import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../core/services/auth.service';

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
    MatButtonModule
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport
          [attr.role]="'navigation'"
          mode="side"
          opened>
        <div class="sidenav-header">
          <div class="logo-container">
            <mat-icon class="logo-icon">inventory</mat-icon>
            <div class="logo-text">
              <span class="brand-name">StockFlow</span>
              <span class="brand-suffix">Pro</span>
            </div>
          </div>
        </div>
        <div class="nav-divider"></div>
        <mat-nav-list class="nav-list">
          <div class="nav-section-title">MAIN</div>
          <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>dashboard</mat-icon>
            <span matListItemTitle>Dashboard</span>
          </a>
          
          <div class="nav-section-title">INVENTORY</div>
          <a mat-list-item routerLink="/products" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>inventory_2</mat-icon>
            <span matListItemTitle>Products</span>
          </a>
          <a mat-list-item routerLink="/inventory" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>warehouse</mat-icon>
            <span matListItemTitle>Inventory</span>
          </a>
          <a mat-list-item routerLink="/warehouses" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>store</mat-icon>
            <span matListItemTitle>Warehouses</span>
          </a>
          
          <div class="nav-section-title">OPERATIONS</div>
          <a mat-list-item routerLink="/orders" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>shopping_cart</mat-icon>
            <span matListItemTitle>Orders</span>
          </a>
          <a mat-list-item routerLink="/purchase-orders" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>receipt_long</mat-icon>
            <span matListItemTitle>Purchase Orders</span>
          </a>
          <a mat-list-item routerLink="/suppliers" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>business</mat-icon>
            <span matListItemTitle>Suppliers</span>
          </a>
          
          <div class="nav-section-title">TRACKING</div>
          <a mat-list-item routerLink="/stock-movements" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>swap_horiz</mat-icon>
            <span matListItemTitle>Stock Movements</span>
          </a>
          <a mat-list-item routerLink="/audit-logs" routerLinkActive="active-link" class="nav-item">
            <mat-icon matListItemIcon>history</mat-icon>
            <span matListItemTitle>Audit Logs</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>
      <mat-sidenav-content>
        <mat-toolbar class="top-toolbar">
          <button mat-icon-button class="menu-toggle" (click)="drawer.toggle()">
            <mat-icon>menu</mat-icon>
          </button>
          <span class="page-title">{{ getPageTitle() }}</span>
          <span class="spacer"></span>
          <button mat-icon-button class="toolbar-action">
            <mat-icon>notifications</mat-icon>
          </button>
          <button mat-icon-button class="toolbar-action">
            <mat-icon>settings</mat-icon>
          </button>
          <div class="user-menu">
            <div class="user-avatar">
              <mat-icon>account_circle</mat-icon>
            </div>
            <div class="user-details">
              <span class="user-name">{{ (currentUser?.email || 'admin@email.com').split('@')[0] || 'Admin' }}</span>
              <span class="user-role">Administrator</span>
            </div>
            <button mat-icon-button (click)="logout()" class="logout-btn">
              <mat-icon>logout</mat-icon>
            </button>
          </div>
        </mat-toolbar>
        <div class="content">
          <router-outlet></router-outlet>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
      background: white;
    }

    .sidenav {
      width: 280px;
      background: white;
      border-right: 1px solid #e0e0e0;
    }

    .sidenav-header {
      padding: 24px 20px;
      background: white;
      border-bottom: 1px solid #e0e0e0;
    }

    .logo-container {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .logo-icon {
      width: 40px;
      height: 40px;
      font-size: 40px;
      color: black;
      background: transparent;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 6px;
    }

    .logo-text {
      display: flex;
      flex-direction: column;
      line-height: 1.2;
    }

    .brand-name {
      font-size: 1.4rem;
      font-weight: 700;
      color: black;
      letter-spacing: -0.5px;
    }

    .brand-suffix {
      font-size: 0.75rem;
      font-weight: 600;
      color: #666;
      letter-spacing: 2px;
      text-transform: uppercase;
    }

    .nav-divider {
      height: 1px;
      background: #e0e0e0;
      margin: 0;
    }

    .nav-list {
      padding: 16px 0;
    }

    .nav-section-title {
      padding: 16px 20px 8px;
      font-size: 0.7rem;
      font-weight: 700;
      color: #999;
      letter-spacing: 1.5px;
      text-transform: uppercase;
    }

    .nav-item {
      margin: 2px 12px;
      border-radius: 8px;
      color: #333;
      transition: all 0.3s ease;
      height: 44px !important;
      
      mat-icon {
        color: #666;
      }

      &.active-link {
        background: #f0f0f0;
        border-left: 3px solid black;
        color: black;
        font-weight: 600;
        
        mat-icon {
          color: black;
        }
      }
    }

    .top-toolbar {
      position: sticky;
      top: 0;
      z-index: 100;
      background: white;
      border-bottom: 1px solid #e0e0e0;
      height: 70px;
      padding: 0 24px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.05);
    }

    .menu-toggle {
      margin-right: 16px;
      display: none;
      
      @media (max-width: 960px) {
        display: inline-flex;
      }
    }

    .page-title {
      font-size: 1.3rem;
      font-weight: 700;
      color: black;
      letter-spacing: -0.5px;
    }

    .toolbar-action {
      margin: 0 4px;
      
      mat-icon {
        color: #666;
      }
    }

    .user-menu {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-left: 16px;
      padding: 8px 12px;
      border-radius: 12px;
      background: #f5f5f5;
      border: 1px solid #e0e0e0;
    }

    .user-avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: black;
      display: flex;
      align-items: center;
      justify-content: center;
      
      mat-icon {
        color: white;
        font-size: 28px;
        width: 28px;
        height: 28px;
      }
    }

    .user-details {
      display: flex;
      flex-direction: column;
      line-height: 1.3;
    }

    .user-name {
      font-size: 0.9rem;
      font-weight: 600;
      color: black;
      text-transform: capitalize;
    }

    .user-role {
      font-size: 0.75rem;
      color: #666;
    }

    .logout-btn {
      mat-icon {
        color: #666;
      }
    }

    .content {
      padding: 32px;
      min-height: calc(100vh - 70px);
      background: white;
      overflow-y: auto;
    }

    .spacer {
      flex: 1 1 auto;
    }

    @media (max-width: 960px) {
      .sidenav {
        width: 240px;
      }
      
      .content {
        padding: 20px;
      }
      
      .user-details {
        display: none;
      }
    }
  `]
})
export class LayoutComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  get currentUser() {
    return this.authService.currentUser;
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

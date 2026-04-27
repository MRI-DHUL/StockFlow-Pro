import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../core/services/order.service';
import { Order, OrderStatus } from '../../../shared/models/domain.models';
import { PagedResponse } from '../../../core/models/auth.models';
import { PaginationComponent } from '../../../shared/components/pagination.component';
import { CurrencyFormatterPipe } from '../../../shared/pipes/currency-formatter.pipe';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCardModule,
    MatChipsModule,
    PaginationComponent,
    CurrencyFormatterPipe
  ],
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss']
})
export class OrderListComponent implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  orders: Order[] = [];
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  displayedColumns = ['orderNumber', 'orderDate', 'customerName', 'totalAmount', 'status', 'actions'];

  filterForm: FormGroup = this.fb.group({
    orderNumber: [''],
    customerName: [''],
    status: ['']
  });

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    const filters = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortBy: 'orderDate',
      sortDescending: true
    };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.orderService.getPaged(filters).subscribe({
      next: (response: PagedResponse<Order>) => {
        this.orders = [...response.items];
        this.totalCount = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load orders', 'Error');
        console.error(error);
      }
    });
  }

  createOrder(): void {
    this.toastr.info('Create Order functionality coming soon', 'Info');
    // TODO: Implement order creation dialog
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.pageNumber = 1;
    this.loadOrders();
  }

  onPageChange(event: { pageNumber: number; pageSize: number }): void {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadOrders();
  }

  viewOrder(order: Order): void {
    this.toastr.info(`Viewing order ${order.orderNumber}`, 'Info');
    // TODO: Implement order details dialog
  }

  getStatusLabel(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'Pending';
      case OrderStatus.Confirmed: return 'Confirmed';
      case OrderStatus.Shipped: return 'Shipped';
      case OrderStatus.Delivered: return 'Delivered';
      case OrderStatus.Cancelled: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'status-pending';
      case OrderStatus.Confirmed: return 'status-confirmed';
      case OrderStatus.Shipped: return 'status-shipped';
      case OrderStatus.Delivered: return 'status-delivered';
      case OrderStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }
}
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
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: white;
      border-bottom: 2px solid black;
      color: black;
      padding: 24px;
      margin: -16px -16px 24px -16px;
      border-radius: 16px 16px 0 0;

      mat-card-title {
        font-size: 1.8rem;
        font-weight: 600;
        margin: 0;
        color: black !important;
      }

      button {
        background: black !important;
        color: white !important;
        border-radius: 8px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);

        mat-icon {
          color: white;
        }
      }
    }

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

      button {
        border-radius: 8px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
    }

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
        background: #000000;
        color: white !important;
        font-weight: 700;
        font-size: 0.95rem;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        padding: 16px !important;
      }

      td {
        padding: 14px !important;
        font-size: 0.95rem;
        color: black !important;
      }

      tr {
        // No hover effect
      }

      mat-icon {
        // No hover effect
      }
    }

    mat-chip.status-pending {
      background: #000000 !important;
      color: white !important;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;

      ::ng-deep .mdc-evolution-chip__text-label {
        color: white !important;
      }
    }

    mat-chip.status-confirmed {
      background: white !important;
      color: #000000 !important;
      border: 2px solid #000000;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;

      ::ng-deep .mdc-evolution-chip__text-label {
        color: #000000 !important;
      }
    }

    mat-chip.status-shipped {
      background: #000000 !important;
      color: white !important;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;

      ::ng-deep .mdc-evolution-chip__text-label {
        color: white !important;
      }
    }

    mat-chip.status-delivered {
      background: white !important;
      color: #000000 !important;
      border: 2px solid #000000;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;

      ::ng-deep .mdc-evolution-chip__text-label {
        color: #000000 !important;
      }
    }

    mat-chip.status-cancelled {
      background: #000000 !important;
      color: white !important;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;

      ::ng-deep .mdc-evolution-chip__text-label {
        color: white !important;
      }
    }
  `]
})
export class OrderListComponent implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  orders: Order[] = [];
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  displayedColumns = ['orderNumber', 'orderDate', 'customerName', 'totalAmount', 'status', 'actions'];

  filterForm: FormGroup = this.fb.group({
    orderNumber: [''],
    customerName: [''],
    status: ['']
  });

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    const filters = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortBy: 'orderDate',
      sortDescending: true
    };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.orderService.getPaged(filters).subscribe({
      next: (response: PagedResponse<Order>) => {
        this.orders = [...response.items];
        this.totalCount = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load orders', 'Error');
        console.error(error);
      }
    });
  }

  createOrder(): void {
    this.toastr.info('Create Order functionality coming soon', 'Info');
    // TODO: Implement order creation dialog
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.pageNumber = 1;
    this.loadOrders();
  }

  onPageChange(event: { pageNumber: number; pageSize: number }): void {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadOrders();
  }

  viewOrder(order: Order): void {
    this.toastr.info(`Viewing order ${order.orderNumber}`, 'Info');
    // TODO: Implement order details dialog
  }

  getStatusLabel(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'Pending';
      case OrderStatus.Confirmed: return 'Confirmed';
      case OrderStatus.Shipped: return 'Shipped';
      case OrderStatus.Delivered: return 'Delivered';
      case OrderStatus.Cancelled: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'status-pending';
      case OrderStatus.Confirmed: return 'status-confirmed';
      case OrderStatus.Shipped: return 'status-shipped';
      case OrderStatus.Delivered: return 'status-delivered';
      case OrderStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }
}

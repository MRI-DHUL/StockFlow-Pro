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
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../core/services/order.service';
import { Order, OrderStatus } from '../../../shared/models/domain.models';
import { PagedResponse } from '../../../core/models/auth.models';
import { PaginationComponent } from '../../../shared/components/pagination.component';
import { CurrencyFormatterPipe } from '../../../shared/pipes/currency-formatter.pipe';
import { OrderFormComponent } from '../order-form/order-form.component';
import { OrderDetailsComponent } from '../order-details/order-details.component';

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
  private readonly dialog = inject(MatDialog);

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
    const dialogRef = this.dialog.open(OrderFormComponent, {
      width: '700px',
      maxWidth: '90vw',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadOrders();
      }
    });
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
    this.dialog.open(OrderDetailsComponent, {
      width: '800px',
      maxWidth: '90vw',
      data: order
    });
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

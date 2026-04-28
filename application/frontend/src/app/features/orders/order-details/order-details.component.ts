import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { Order, OrderStatus } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatChipsModule
  ],
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent {
  private readonly dialogRef = inject(MatDialogRef<OrderDetailsComponent>);
  displayedColumns: string[] = ['product', 'quantity', 'unitPrice', 'subtotal'];

  constructor(@Inject(MAT_DIALOG_DATA) public order: Order) {}

  getStatusColor(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'warn';
      case OrderStatus.Confirmed: return 'primary';
      case OrderStatus.Shipped: return 'accent';
      case OrderStatus.Delivered: return 'primary';
      case OrderStatus.Cancelled: return 'warn';
      default: return '';
    }
  }

  getStatusText(status: OrderStatus): string {
    return OrderStatus[status];
  }

  onClose(): void {
    this.dialogRef.close();
  }
}

import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { PurchaseOrder, PurchaseOrderStatus } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-purchase-order-details',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatChipsModule
  ],
  templateUrl: './purchase-order-details.component.html',
  styleUrls: ['./purchase-order-details.component.scss']
})
export class PurchaseOrderDetailsComponent {
  private readonly dialogRef = inject(MatDialogRef<PurchaseOrderDetailsComponent>);
  displayedColumns: string[] = ['product', 'quantity', 'unitPrice', 'subtotal'];

  constructor(@Inject(MAT_DIALOG_DATA) public po: PurchaseOrder) {}

  getStatusColor(status: PurchaseOrderStatus): string {
    switch (status) {
      case PurchaseOrderStatus.Draft: return 'warn';
      case PurchaseOrderStatus.Submitted: return 'accent';
      case PurchaseOrderStatus.Approved: return 'primary';
      case PurchaseOrderStatus.Received: return 'primary';
      case PurchaseOrderStatus.Cancelled: return 'warn';
      default: return '';
    }
  }

  getStatusText(status: PurchaseOrderStatus): string {
    return PurchaseOrderStatus[status];
  }

  onClose(): void {
    this.dialogRef.close();
  }
}

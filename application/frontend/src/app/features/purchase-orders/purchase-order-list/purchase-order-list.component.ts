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
import { PurchaseOrderService } from '../../../core/services/purchase-order.service';
import { PurchaseOrder, PurchaseOrderStatus } from '../../../shared/models/domain.models';
import { CurrencyFormatterPipe } from '../../../shared/pipes/currency-formatter.pipe';
import { PurchaseOrderFormComponent } from '../purchase-order-form/purchase-order-form.component';
import { PurchaseOrderDetailsComponent } from '../purchase-order-details/purchase-order-details.component';

@Component({
  selector: 'app-purchase-order-list',
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
    CurrencyFormatterPipe
  ],
  templateUrl: './purchase-order-list.component.html',
  styleUrls: ['./purchase-order-list.component.scss']
})
export class PurchaseOrderListComponent implements OnInit {
  private readonly purchaseOrderService = inject(PurchaseOrderService);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly fb = inject(FormBuilder);
  private readonly dialog = inject(MatDialog);

  purchaseOrders: PurchaseOrder[] = [];
  allPurchaseOrders: PurchaseOrder[] = [];
  displayedColumns = ['poNumber', 'orderDate', 'supplierName', 'expectedDeliveryDate', 'totalAmount', 'status', 'actions'];

  filterForm: FormGroup = this.fb.group({
    poNumber: [''],
    supplierName: [''],
    status: ['']
  });

  ngOnInit(): void {
    this.loadPurchaseOrders();
  }

  loadPurchaseOrders(): void {
    this.purchaseOrderService.getAll().subscribe({
      next: (pos) => {
        this.allPurchaseOrders = [...pos];
        this.applyFilters();
      },
      error: (error) => {
        this.toastr.error('Failed to load purchase orders', 'Error');
        console.error(error);
      }
    });
  }

  applyFilters(): void {
    const filters = this.filterForm.value;
    let filtered = [...this.allPurchaseOrders];

    if (filters.poNumber) {
      filtered = filtered.filter(po => 
        po.poNumber.toLowerCase().includes(filters.poNumber.toLowerCase())
      );
    }

    if (filters.supplierName) {
      filtered = filtered.filter(po => 
        po.supplierName.toLowerCase().includes(filters.supplierName.toLowerCase())
      );
    }

    if (filters.status !== '' && filters.status !== null) {
      filtered = filtered.filter(po => po.status === filters.status);
    }

    this.purchaseOrders = filtered;
    this.cdr.detectChanges();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.applyFilters();
  }

  createPO(): void {
    const dialogRef = this.dialog.open(PurchaseOrderFormComponent, {
      width: '800px',
      maxWidth: '90vw',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadPurchaseOrders();
      }
    });
  }

  viewPO(po: PurchaseOrder): void {
    this.dialog.open(PurchaseOrderDetailsComponent, {
      width: '800px',
      maxWidth: '90vw',
      data: po
    });
  }

  getStatusLabel(status: PurchaseOrderStatus): string {
    switch (status) {
      case PurchaseOrderStatus.Draft: return 'Draft';
      case PurchaseOrderStatus.Submitted: return 'Submitted';
      case PurchaseOrderStatus.Approved: return 'Approved';
      case PurchaseOrderStatus.Received: return 'Received';
      case PurchaseOrderStatus.Cancelled: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getStatusClass(status: PurchaseOrderStatus): string {
    switch (status) {
      case PurchaseOrderStatus.Draft: return 'status-draft';
      case PurchaseOrderStatus.Submitted: return 'status-submitted';
      case PurchaseOrderStatus.Approved: return 'status-approved';
      case PurchaseOrderStatus.Received: return 'status-received';
      case PurchaseOrderStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }
}

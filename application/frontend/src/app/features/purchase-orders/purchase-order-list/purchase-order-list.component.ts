import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { PurchaseOrderService } from '../../../core/services/purchase-order.service';
import { PurchaseOrder, PurchaseOrderStatus } from '../../../shared/models/domain.models';
import { CurrencyFormatterPipe } from '../../../shared/pipes/currency-formatter.pipe';

@Component({
  selector: 'app-purchase-order-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    CurrencyFormatterPipe
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Purchase Orders</mat-card-title>
        <button mat-raised-button color="accent" (click)="createPurchaseOrder()">
          <mat-icon>add</mat-icon>
          Create PO
        </button>
      </mat-card-header>
      <mat-card-content>
        <div class="table-container">
          <table mat-table [dataSource]="purchaseOrders" class="mat-elevation-z2">
            <ng-container matColumnDef="poNumber">
              <th mat-header-cell *matHeaderCellDef>PO #</th>
              <td mat-cell *matCellDef="let po">{{ po.poNumber }}</td>
            </ng-container>

            <ng-container matColumnDef="orderDate">
              <th mat-header-cell *matHeaderCellDef>Order Date</th>
              <td mat-cell *matCellDef="let po">{{ po.orderDate | date:'short' }}</td>
            </ng-container>

            <ng-container matColumnDef="supplierName">
              <th mat-header-cell *matHeaderCellDef>Supplier</th>
              <td mat-cell *matCellDef="let po">{{ po.supplierName }}</td>
            </ng-container>

            <ng-container matColumnDef="expectedDeliveryDate">
              <th mat-header-cell *matHeaderCellDef>Expected Delivery</th>
              <td mat-cell *matCellDef="let po">{{ po.expectedDeliveryDate | date:'short' }}</td>
            </ng-container>

            <ng-container matColumnDef="totalAmount">
              <th mat-header-cell *matHeaderCellDef>Total Amount</th>
              <td mat-cell *matCellDef="let po">{{ po.totalAmount | currency }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let po">
                <mat-chip [class]="getStatusClass(po.status)">
                  {{ getStatusLabel(po.status) }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let po">
                <button mat-icon-button color="primary" (click)="viewPO(po)">
                  <mat-icon>visibility</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          <div class="no-data-container" *ngIf="purchaseOrders.length === 0">
            <div class="no-data-message">No Data Found</div>
          </div>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
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
        background: white;
        border-bottom: 2px solid black;
        color: black !important;
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

    mat-chip.status-draft {
      background: linear-gradient(135deg, #95a5a6 0%, #7f8c8d 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }

    mat-chip.status-submitted {
      background: linear-gradient(135deg, #3498db 0%, #2980b9 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }

    mat-chip.status-approved {
      background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }

    mat-chip.status-received {
      background: linear-gradient(135deg, #1abc9c 0%, #16a085 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }

    mat-chip.status-cancelled {
      background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }
  `]
})
export class PurchaseOrderListComponent implements OnInit {
  private readonly purchaseOrderService = inject(PurchaseOrderService);
  private readonly toastr = inject(ToastrService);

  purchaseOrders: PurchaseOrder[] = [];
  displayedColumns = ['poNumber', 'orderDate', 'supplierName', 'expectedDeliveryDate', 'totalAmount', 'status', 'actions'];

  ngOnInit(): void {
    this.loadPurchaseOrders();
  }

  loadPurchaseOrders(): void {
    this.purchaseOrderService.getAll().subscribe({
      next: (pos) => {
        this.purchaseOrders = pos;
      },
      error: (error) => {
        this.toastr.error('Failed to load purchase orders', 'Error');
        console.error(error);
      }
    });
  }

  createPurchaseOrder(): void {
    this.toastr.info('Create PO functionality coming soon', 'Info');
    // TODO: Implement PO creation dialog
  }

  viewPO(po: PurchaseOrder): void {
    this.toastr.info(`Viewing PO ${po.poNumber}`, 'Info');
    // TODO: Implement PO details dialog
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

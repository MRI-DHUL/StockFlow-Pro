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
import { StockMovementService } from '../../../core/services/stock-movement.service';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { ProductService } from '../../../core/services/product.service';
import { StockMovement, MovementType } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-stock-movement-list',
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
    MatChipsModule
  ],
  templateUrl: './stock-movement-list.component.html',
  styleUrls: ['./stock-movement-list.component.scss']
})
export class StockMovementListComponent implements OnInit {
  private readonly stockMovementService = inject(StockMovementService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  movements: StockMovement[] = [];
  warehouses: any[] = [];
  displayedColumns = ['movementDate', 'productName', 'warehouseName', 'movementType', 'quantity', 'referenceNumber', 'notes'];

  filterForm: FormGroup = this.fb.group({
    warehouseId: [''],
    movementType: ['']
  });

  ngOnInit(): void {
    this.loadMovements();
    this.loadWarehouses();
  }

  loadMovements(): void {
    const filters = { ...this.filterForm.value };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.stockMovementService.getAll(filters).subscribe({
      next: (movements) => {
        this.movements = [...movements];
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load stock movements', 'Error');
        console.error(error);
      }
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.warehouses = [...warehouses];
        this.cdr.detectChanges();
      }
    });
  }

  recordMovement(): void {
    this.toastr.info('Record Movement functionality coming soon', 'Info');
    // TODO: Implement movement recording dialog
  }

  applyFilters(): void {
    this.loadMovements();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.loadMovements();
  }

  getMovementTypeLabel(type: MovementType): string {
    switch (type) {
      case MovementType.In: return 'In';
      case MovementType.Out: return 'Out';
      case MovementType.Transfer: return 'Transfer';
      case MovementType.Adjustment: return 'Adjustment';
      default: return 'Unknown';
    }
  }

  getMovementTypeClass(type: MovementType): string {
    switch (type) {
      case MovementType.In: return 'type-in';
      case MovementType.Out: return 'type-out';
      case MovementType.Transfer: return 'type-transfer';
      case MovementType.Adjustment: return 'type-adjustment';
      default: return '';
    }
  }
}

            <ng-container matColumnDef="quantity">
              <th mat-header-cell *matHeaderCellDef>Quantity</th>
              <td mat-cell *matCellDef="let movement">
                <span [class]="movement.movementType === 1 ? 'qty-out' : 'qty-in'">
                  {{ movement.movementType === 1 ? '-' : '+' }}{{ movement.quantity }}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="referenceNumber">
              <th mat-header-cell *matHeaderCellDef>Reference #</th>
              <td mat-cell *matCellDef="let movement">{{ movement.referenceNumber || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="notes">
              <th mat-header-cell *matHeaderCellDef>Notes</th>
              <td mat-cell *matCellDef="let movement">{{ movement.notes || '-' }}</td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
})export class StockMovementListComponent implements OnInit {
  private readonly stockMovementService = inject(StockMovementService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  movements: StockMovement[] = [];
  warehouses: any[] = [];
  displayedColumns = ['movementDate', 'productName', 'warehouseName', 'movementType', 'quantity', 'referenceNumber', 'notes'];

  filterForm: FormGroup = this.fb.group({
    warehouseId: [''],
    movementType: ['']
  });

  ngOnInit(): void {
    this.loadMovements();
    this.loadWarehouses();
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
    }

    .qty-in {
      color: #000000;
      font-weight: 700;
      background: white;
      border: 2px solid #000000;
      padding: 4px 12px;
      border-radius: 12px;
    }

    .qty-out {
      color: white !important;
      font-weight: 700;
      background: #000000;
      padding: 4px 12px;
      border-radius: 12px;
    }

    mat-chip.type-in {
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

    mat-chip.type-out {
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

    mat-chip.type-transfer {
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

    mat-chip.type-adjustment {
      background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%) !important;
      color: white;
      font-weight: 700;
      padding: 8px 16px !important;
      border-radius: 16px !important;
      text-transform: uppercase;
      font-size: 0.85rem !important;
    }
  `]
})
export class StockMovementListComponent implements OnInit {
  private readonly stockMovementService = inject(StockMovementService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  movements: StockMovement[] = [];
  warehouses: any[] = [];
  displayedColumns = ['movementDate', 'productName', 'warehouseName', 'movementType', 'quantity', 'referenceNumber', 'notes'];

  filterForm: FormGroup = this.fb.group({
    warehouseId: [''],
    movementType: ['']
  });

  ngOnInit(): void {
    this.loadMovements();
    this.loadWarehouses();
  }

  loadMovements(): void {
    const filters = { ...this.filterForm.value };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.stockMovementService.getAll(filters).subscribe({
      next: (movements) => {
        this.movements = [...movements];
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load stock movements', 'Error');
        console.error(error);
      }
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.warehouses = [...warehouses];
        this.cdr.detectChanges();
      }
    });
  }

  recordMovement(): void {
    this.toastr.info('Record Movement functionality coming soon', 'Info');
    // TODO: Implement movement recording dialog
  }

  applyFilters(): void {
    this.loadMovements();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.loadMovements();
  }

  getMovementTypeLabel(type: MovementType): string {
    switch (type) {
      case MovementType.In: return 'In';
      case MovementType.Out: return 'Out';
      case MovementType.Transfer: return 'Transfer';
      case MovementType.Adjustment: return 'Adjustment';
      default: return 'Unknown';
    }
  }

  getMovementTypeClass(type: MovementType): string {
    switch (type) {
      case MovementType.In: return 'type-in';
      case MovementType.Out: return 'type-out';
      case MovementType.Transfer: return 'type-transfer';
      case MovementType.Adjustment: return 'type-adjustment';
      default: return '';
    }
  }
}

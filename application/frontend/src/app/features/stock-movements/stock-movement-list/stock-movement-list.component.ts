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

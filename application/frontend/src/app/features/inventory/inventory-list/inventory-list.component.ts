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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { InventoryService } from '../../../core/services/inventory.service';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { ProductService } from '../../../core/services/product.service';
import { InventoryItem } from '../../../shared/models/domain.models';
import { PagedResponse } from '../../../core/models/auth.models';
import { PaginationComponent } from '../../../shared/components/pagination.component';
import { InventoryFormComponent } from '../inventory-form/inventory-form.component';

@Component({
  selector: 'app-inventory-list',
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
    MatDialogModule,
    PaginationComponent
  ],
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit {
  private readonly inventoryService = inject(InventoryService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  inventory: InventoryItem[] = [];
  warehouses: any[] = [];
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  displayedColumns = ['productName', 'warehouseName', 'quantity', 'threshold', 'actions'];

  filterForm: FormGroup = this.fb.group({
    warehouseId: ['']
  });

  ngOnInit(): void {
    this.loadInventory();
    this.loadWarehouses();
  }

  loadInventory(): void {
    const filters = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.inventoryService.getPaged(filters).subscribe({
      next: (response: PagedResponse<InventoryItem>) => {
        this.inventory = [...response.items];
        this.totalCount = response.totalCount;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load inventory', 'Error');
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

  openInventoryDialog(item?: InventoryItem): void {
    const dialogRef = this.dialog.open(InventoryFormComponent, {
      width: '500px',
      data: item
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadInventory();
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadInventory();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.pageNumber = 1;
    this.loadInventory();
  }

  onPageChange(event: { pageNumber: number; pageSize: number }): void {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadInventory();
  }
}

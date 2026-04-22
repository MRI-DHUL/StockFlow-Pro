import { Component, OnInit, inject } from '@angular/core';
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
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Inventory Management</mat-card-title>
        <button mat-raised-button color="accent" (click)="openInventoryDialog()">
          <mat-icon>add</mat-icon>
          Adjust Stock
        </button>
      </mat-card-header>
      <mat-card-content>
        <!-- Filters -->
        <div class="filters-container">
          <form [formGroup]="filterForm" class="filters-form">
            <mat-form-field appearance="outline">
              <mat-label>Warehouse</mat-label>
              <mat-select formControlName="warehouseId">
                <mat-option value="">All Warehouses</mat-option>
                <mat-option *ngFor="let warehouse of warehouses" [value]="warehouse.id">
                  {{ warehouse.name }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <button mat-raised-button color="primary" (click)="applyFilters()">
              <mat-icon>filter_list</mat-icon>
              Apply Filters
            </button>
            <button mat-button (click)="resetFilters()">Reset</button>
          </form>
        </div>

        <!-- Table -->
        <div class="table-container">
          <div class="no-data-container" *ngIf="inventory.length === 0">
            <div class="no-data-message">No Data Found</div>
          </div>
          <table mat-table [dataSource]="inventory" class="mat-elevation-z2" *ngIf="inventory.length > 0">
            <ng-container matColumnDef="productName">
              <th mat-header-cell *matHeaderCellDef>Product</th>
              <td mat-cell *matCellDef="let item">{{ item.productName }}</td>
            </ng-container>

            <ng-container matColumnDef="warehouseName">
              <th mat-header-cell *matHeaderCellDef>Warehouse</th>
              <td mat-cell *matCellDef="let item">{{ item.warehouseName }}</td>
            </ng-container>

            <ng-container matColumnDef="quantityOnHand">
              <th mat-header-cell *matHeaderCellDef>On Hand</th>
              <td mat-cell *matCellDef="let item">{{ item.quantityOnHand }}</td>
            </ng-container>

            <ng-container matColumnDef="quantityReserved">
              <th mat-header-cell *matHeaderCellDef>Reserved</th>
              <td mat-cell *matCellDef="let item">{{ item.quantityReserved }}</td>
            </ng-container>

            <ng-container matColumnDef="quantityAvailable">
              <th mat-header-cell *matHeaderCellDef>Available</th>
              <td mat-cell *matCellDef="let item">
                <span [class]="item.quantityAvailable < 10 ? 'low-stock' : ''">
                  {{ item.quantityAvailable }}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let item">
                <button mat-icon-button color="primary" (click)="openInventoryDialog(item)">
                  <mat-icon>edit</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        </div>

        <!-- Pagination -->
        <app-pagination
          [totalCount]="totalCount"
          [pageSize]="pageSize"
          [pageNumber]="pageNumber"
          (pageChange)="onPageChange($event)">
        </app-pagination>
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
        min-width: 250px;
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

    .low-stock {
      color: #e74c3c;
      font-weight: 700;
      background: rgba(231, 76, 60, 0.1);
      padding: 4px 12px;
      border-radius: 12px;
      display: inline-block;
      animation: pulse 2s ease-in-out infinite;
    }

    @keyframes pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.7; }
    }
  `]
})
export class InventoryListComponent implements OnInit {
  private readonly inventoryService = inject(InventoryService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  inventory: InventoryItem[] = [];
  warehouses: any[] = [];
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  displayedColumns = ['productName', 'warehouseName', 'quantityOnHand', 'quantityReserved', 'quantityAvailable', 'actions'];

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
        this.inventory = response.items;
        this.totalCount = response.totalCount;
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
        this.warehouses = warehouses;
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
}

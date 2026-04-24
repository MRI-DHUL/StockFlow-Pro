import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/domain.models';
import { PagedResponse } from '../../../core/models/auth.models';
import { PaginationComponent } from '../../../shared/components/pagination.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog.component';
import { CurrencyFormatterPipe } from '../../../shared/pipes/currency-formatter.pipe';
import { ProductFormComponent } from '../product-form/product-form.component';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCardModule,
    MatDialogModule,
    MatSnackBarModule,
    PaginationComponent,
    CurrencyFormatterPipe
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Products Management</mat-card-title>
        <button mat-raised-button color="accent" (click)="openProductDialog()">
          <mat-icon>add</mat-icon>
          Add Product
        </button>
      </mat-card-header>
      <mat-card-content>
        <!-- Filters -->
        <div class="filters-container">
          <form [formGroup]="filterForm" class="filters-form">
            <mat-form-field appearance="outline">
              <mat-label>Search</mat-label>
              <input matInput formControlName="searchTerm" placeholder="Search by name or SKU">
              <mat-icon matSuffix>search</mat-icon>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Category</mat-label>
              <mat-select formControlName="category">
                <mat-option value="">All Categories</mat-option>
                <mat-option *ngFor="let cat of categories" [value]="cat">{{ cat }}</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Min Price</mat-label>
              <input matInput type="number" formControlName="minPrice">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Max Price</mat-label>
              <input matInput type="number" formControlName="maxPrice">
            </mat-form-field>

            <button mat-raised-button class="filter-spacer" color="primary" (click)="applyFilters()">
              <mat-icon>filter_list</mat-icon>
              Apply Filters
            </button>
            <button mat-button class="filter-spacer" (click)="resetFilters()">Reset</button>
          </form>
        </div>

        <!-- Table -->
        <div class="table-container">
          <div class="no-data-container" *ngIf="products.length === 0">
            <div class="no-data-message">No Data Found</div>
          </div>
          <table mat-table [dataSource]="products" class="mat-elevation-z2" *ngIf="products.length > 0">
            <ng-container matColumnDef="sku">
              <th mat-header-cell *matHeaderCellDef>SKU</th>
              <td mat-cell *matCellDef="let product">{{ product.sku }}</td>
            </ng-container>

            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let product">{{ product.name }}</td>
            </ng-container>

            <ng-container matColumnDef="category">
              <th mat-header-cell *matHeaderCellDef>Category</th>
              <td mat-cell *matCellDef="let product">
                <span class="category-badge" [class]="getCategoryClass(product.category)">
                  {{ product.category }}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="unitPrice">
              <th mat-header-cell *matHeaderCellDef>Unit Price</th>
              <td mat-cell *matCellDef="let product">
                <span class="price-cell">{{ product.unitPrice | currency }}</span>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let product" class="actions-cell">
                <button mat-mini-fab color="primary" class="action-btn edit-btn" (click)="openProductDialog(product)" matTooltip="Edit">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-mini-fab color="warn" class="action-btn delete-btn" (click)="deleteProduct(product)" matTooltip="Delete">
                  <mat-icon>delete</mat-icon>
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
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 28px;
      gap: 20px;
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
    }

    .filters-form {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
      flex: 1;
    }

    .filters-form mat-form-field {
      min-width: 200px;
      flex: 1;
    }

    .filters-form button {
      border-radius: 8px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .table-container {
      overflow-x: auto;
      margin: 24px 0;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
      background: white;
    }

    table {
      width: 100%;
      background: white;
      
      th {
        background: #000000;
        color: white !important;
        font-weight: 700;
        font-size: 0.9rem;
        text-transform: uppercase;
        letter-spacing: 1px;
        padding: 18px 16px !important;
        border: none;
      }

      td {
        padding: 16px !important;
        font-size: 0.95rem;
        color: #000000;
        border-bottom: 1px solid #e5e5e5;
      }

      tr {
        &:nth-child(even) {
          background: #fafafa;
        }
      }
    }

    .category-badge {
      display: inline-block;
      padding: 6px 16px;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
      text-transform: capitalize;
      letter-spacing: 0.3px;
      border: 2px solid;
    }

    .category-electronics {
      background: #000000;
      color: white !important;
      border-color: #000000;
    }

    .category-office-supplies {
      background: white;
      color: #000000;
      border-color: #000000;
    }

    .category-furniture {
      background: #000000;
      color: white !important;
      border-color: #000000;
    }

    .category-hardware {
      background: white;
      color: #000000;
      border-color: #000000;
    }

    .category-packaging {
      background: #000000;
      color: white !important;
      border-color: #000000;
    }

    .category-default {
      background: white;
      color: #000000;
      border-color: #000000;
    }

    .price-cell {
      font-weight: 700;
      font-size: 1.1rem;
      color: #000000;
    }

    .actions-cell {
      display: flex;
      gap: 8px;
      justify-content: flex-start;
      align-items: center;
    }

    .action-btn {
      width: 40px !important;
      height: 40px !important;
      box-shadow: 0 4px 8px rgba(0,0,0,0.2);
      
      mat-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
      }
    }

    .edit-btn {
      background: #000000 !important;
      color: white !important;
      
      mat-icon {
        color: white !important;
      }
    }

    .delete-btn {
      background: white !important;
      color: #000000 !important;
      border: 2px solid #000000;
      
      mat-icon {
        color: #000000 !important;
      }
    }

    .status-active {
      color: #000000;
      font-weight: 600;
      background: white;
      border: 2px solid #000000;
      padding: 6px 14px;
      border-radius: 16px;
      display: inline-block;
    }

    .status-inactive {
      color: white !important;
      font-weight: 600;
      background: #000000;
      padding: 6px 14px;
      border-radius: 16px;
      display: inline-block;
    }

    button[mat-icon-button] {
      // No hover effect
    }

    @media (max-width: 768px) {
      .filters-container {
        flex-direction: column;
      }

      .filters-form {
        width: 100%;
      }
    }
  `]
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  products: Product[] = [];
  categories: string[] = [];
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  displayedColumns = ['sku', 'name', 'category', 'unitPrice', 'actions'];

  filterForm: FormGroup = this.fb.group({
    searchTerm: [''],
    category: [''],
    minPrice: [null],
    maxPrice: [null]
  });

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts(): void {
    const filters = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortBy: 'name'
    };

    this.productService.getPaged(filters).subscribe({
      next: (response: PagedResponse<Product>) => {
        this.products = [...response.items]; // Create new array reference
        this.totalCount = response.totalCount;
        this.cdr.detectChanges(); // Trigger change detection
      },
      error: (error) => {
        this.toastr.error('Failed to load products', 'Error');
        console.error(error);
      }
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = [...categories]; // Create new array reference
        this.cdr.detectChanges(); // Trigger change detection
      },
      error: (error) => {
        console.error('Failed to load categories', error);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadProducts();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.pageNumber = 1;
    this.loadProducts();
  }

  onPageChange(event: { pageNumber: number; pageSize: number }): void {
    this.pageNumber = event.pageNumber;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }

  openProductDialog(product?: Product): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      width: '600px',
      data: product
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProducts();
      }
    });
  }

  deleteProduct(product: Product): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: `Are you sure you want to delete "${product.name}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.productService.delete(product.id).subscribe({
          next: () => {
            this.toastr.success('Product deleted successfully', 'Success');
            this.loadProducts();
          },
          error: (error) => {
            this.toastr.error('Failed to delete product', 'Error');
            console.error(error);
          }
        });
      }
    });
  }

  getCategoryClass(category: string): string {
    if (!category) return 'category-default';
    const normalized = category.toLowerCase().replace(/\s+/g, '-');
    return `category-${normalized}`;
  }
}

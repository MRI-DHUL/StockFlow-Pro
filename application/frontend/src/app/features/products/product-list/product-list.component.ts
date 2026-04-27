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
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
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
        this.products = [...response.items];
        this.totalCount = response.totalCount;
        this.cdr.detectChanges();
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
        this.categories = [...categories];
        this.cdr.detectChanges();
      },
      error: (error) => console.error('Failed to load categories', error)
    });
  }

  openProductDialog(product?: Product): void {
    const dialogRef = this.dialog.open(ProductFormComponent, {
      width: '700px',
      data: product || null
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProducts();
      }
    });
  }

  deleteProduct(product: Product): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Product',
        message: `Are you sure you want to delete "${product.name}"?`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
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

  getCategoryClass(category: string): string {
    const normalized = category.toLowerCase().replace(/\s+/g, '-');
    return `category-${normalized}`;
  }
}

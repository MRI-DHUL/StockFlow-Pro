import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../core/services/product.service';
import { Product, CreateProductDto, UpdateProductDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule
  ],
  template: `
    <h2 mat-dialog-title>{{ isEditMode ? 'Edit Product' : 'Add Product' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="productForm" class="product-form">
        <mat-form-field appearance="outline">
          <mat-label>Product Name</mat-label>
          <input matInput formControlName="name" placeholder="Enter product name">
          <mat-error *ngIf="productForm.get('name')?.hasError('required')">
            Product name is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline" *ngIf="!isEditMode">
          <mat-label>SKU</mat-label>
          <input matInput formControlName="sku" placeholder="Enter SKU">
          <mat-error *ngIf="productForm.get('sku')?.hasError('required')">
            SKU is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description" rows="3" placeholder="Enter description"></textarea>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Category</mat-label>
          <input matInput formControlName="category" placeholder="Enter category">
          <mat-error *ngIf="productForm.get('category')?.hasError('required')">
            Category is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Unit Price</mat-label>
          <input matInput type="number" formControlName="unitPrice" placeholder="0.00">
          <span matPrefix>$ &nbsp;</span>
          <mat-error *ngIf="productForm.get('unitPrice')?.hasError('required')">
            Unit price is required
          </mat-error>
          <mat-error *ngIf="productForm.get('unitPrice')?.hasError('min')">
            Unit price must be greater than 0
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Reorder Level</mat-label>
          <input matInput type="number" formControlName="reorderLevel" placeholder="0">
          <mat-error *ngIf="productForm.get('reorderLevel')?.hasError('required')">
            Reorder level is required
          </mat-error>
          <mat-error *ngIf="productForm.get('reorderLevel')?.hasError('min')">
            Reorder level must be 0 or greater
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Quantity Per Unit</mat-label>
          <input matInput formControlName="quantityPerUnit" placeholder="e.g., 10 boxes x 20 bags">
          <mat-error *ngIf="productForm.get('quantityPerUnit')?.hasError('required')">
            Quantity per unit is required
          </mat-error>
        </mat-form-field>

        <mat-checkbox *ngIf="isEditMode" formControlName="isActive">
          Is Active
        </mat-checkbox>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!productForm.valid || isSubmitting">
        {{ isSubmitting ? 'Saving...' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    mat-dialog-content {
      min-width: 500px;
      max-height: 80vh;
      overflow-y: auto;
    }

    .product-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
    }

    .product-form mat-form-field {
      width: 100%;
    }
  `]
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<ProductFormComponent>);

  productForm!: FormGroup;
  isEditMode = false;
  isSubmitting = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Product | undefined) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;
    this.initForm();
  }

  initForm(): void {
    if (this.isEditMode && this.data) {
      this.productForm = this.fb.group({
        name: [this.data.name, Validators.required],
        description: [this.data.description],
        category: [this.data.category, Validators.required],
        unitPrice: [this.data.unitPrice, [Validators.required, Validators.min(0.01)]],
        reorderLevel: [this.data.reorderLevel, [Validators.required, Validators.min(0)]],
        quantityPerUnit: [this.data.quantityPerUnit, Validators.required],
        isActive: [this.data.isActive]
      });
    } else {
      this.productForm = this.fb.group({
        name: ['', Validators.required],
        sku: ['', Validators.required],
        description: [''],
        category: ['', Validators.required],
        unitPrice: [0, [Validators.required, Validators.min(0.01)]],
        reorderLevel: [0, [Validators.required, Validators.min(0)]],
        quantityPerUnit: ['', Validators.required]
      });
    }
  }

  onSubmit(): void {
    if (this.productForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      if (this.isEditMode && this.data) {
        const updateDto: UpdateProductDto = this.productForm.value;
        this.productService.update(this.data.id, updateDto).subscribe({
          next: () => {
            this.toastr.success('Product updated successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to update product', 'Error');
            console.error(error);
            this.isSubmitting = false;
          }
        });
      } else {
        const createDto: CreateProductDto = this.productForm.value;
        this.productService.create(createDto).subscribe({
          next: () => {
            this.toastr.success('Product created successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to create product', 'Error');
            console.error(error);
            this.isSubmitting = false;
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

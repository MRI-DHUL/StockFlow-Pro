import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../core/services/product.service';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Product, CreateProductDto, UpdateProductDto, Warehouse } from '../../../shared/models/domain.models';
import { switchMap } from 'rxjs';

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
    MatStepperModule
  ],
  template: `
    <h2 mat-dialog-title>{{ isEditMode ? 'Edit Product' : 'Add Product' }}</h2>
    <mat-dialog-content>
      <mat-stepper #stepper linear>
        <!-- Step 1: Basic Information -->
        <mat-step [stepControl]="basicInfoGroup">
          <form [formGroup]="basicInfoGroup" class="product-form">
            <ng-template matStepLabel>Basic Information</ng-template>
            
            <mat-form-field appearance="outline">
              <mat-label>Product Name</mat-label>
              <input matInput formControlName="name" placeholder="Enter product name">
              <mat-error *ngIf="basicInfoGroup.get('name')?.hasError('required')">
                Product name is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Category</mat-label>
              <mat-select formControlName="category">
                <mat-option *ngFor="let cat of categories" [value]="cat">
                  {{ cat }}
                </mat-option>
              </mat-select>
              <mat-error *ngIf="basicInfoGroup.get('category')?.hasError('required')">
                Category is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" *ngIf="!isEditMode">
              <mat-label>SKU</mat-label>
              <input matInput formControlName="sku" placeholder="Enter SKU">
              <mat-hint>Auto-generated based on category, but you can edit it</mat-hint>
              <mat-error *ngIf="basicInfoGroup.get('sku')?.hasError('required')">
                SKU is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="3" placeholder="Enter description"></textarea>
            </mat-form-field>

            <div class="step-actions">
              <button mat-button (click)="onCancel()">Cancel</button>
              <button mat-raised-button color="primary" matStepperNext>Next</button>
            </div>
          </form>
        </mat-step>

        <!-- Step 2: Pricing -->
        <mat-step [stepControl]="pricingGroup">
          <form [formGroup]="pricingGroup" class="product-form">
            <ng-template matStepLabel>Pricing</ng-template>
            
            <mat-form-field appearance="outline">
              <mat-label>Unit Price</mat-label>
              <input matInput type="number" formControlName="unitPrice" placeholder="0.00">
              <span matPrefix>₹ &nbsp;</span>
              <mat-error *ngIf="pricingGroup.get('unitPrice')?.hasError('required')">
                Unit price is required
              </mat-error>
              <mat-error *ngIf="pricingGroup.get('unitPrice')?.hasError('min')">
                Unit price must be greater than 0
              </mat-error>
            </mat-form-field>

            <div class="step-actions">
              <button mat-button matStepperPrevious>Back</button>
              <button mat-button (click)="onCancel()">Cancel</button>
              <button mat-raised-button color="primary" matStepperNext>Next</button>
            </div>
          </form>
        </mat-step>

        <!-- Step 3: Inventory Setup -->
        <mat-step [stepControl]="inventoryGroup">
          <form [formGroup]="inventoryGroup" class="product-form">
            <ng-template matStepLabel>Inventory {{ isEditMode ? 'Update' : 'Setup' }}</ng-template>
            
            <mat-form-field appearance="outline">
              <mat-label>Warehouse</mat-label>
              <mat-select formControlName="warehouseId">
                <mat-option *ngFor="let warehouse of warehouses" [value]="warehouse.id">
                  {{ warehouse.name }}
                </mat-option>
              </mat-select>
              <mat-error *ngIf="inventoryGroup.get('warehouseId')?.hasError('required')">
                Warehouse is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>{{ isEditMode ? 'Current Quantity' : 'Initial Quantity' }}</mat-label>
              <input matInput type="number" formControlName="initialQuantity" placeholder="0">
              <mat-error *ngIf="inventoryGroup.get('initialQuantity')?.hasError('required')">
                Quantity is required
              </mat-error>
              <mat-error *ngIf="inventoryGroup.get('initialQuantity')?.hasError('min')">
                Quantity must be 0 or greater
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Low Stock Threshold</mat-label>
              <input matInput type="number" formControlName="threshold" placeholder="0">
              <mat-hint>Alert when quantity falls below this level</mat-hint>
              <mat-error *ngIf="inventoryGroup.get('threshold')?.hasError('required')">
                Threshold is required
              </mat-error>
              <mat-error *ngIf="inventoryGroup.get('threshold')?.hasError('min')">
                Threshold must be 0 or greater
              </mat-error>
            </mat-form-field>

            <div class="step-actions">
              <button mat-button matStepperPrevious>Back</button>
              <button mat-button (click)="onCancel()">Cancel</button>
              <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!isFormValid() || isSubmitting">
                {{ isSubmitting ? 'Saving...' : 'Save' }}
              </button>
            </div>
          </form>
        </mat-step>
      </mat-stepper>
    </mat-dialog-content>
  `,
  styles: [`
    mat-dialog-content {
      min-width: 500px;
      max-height: 80vh;
      overflow-y: auto;
      padding: 0;
    }

    .product-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px;
    }

    .product-form mat-form-field {
      width: 100%;
    }

    .step-actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
      margin-top: 24px;
      padding-top: 16px;
      border-top: 1px solid #e0e0e0;
    }

    ::ng-deep .mat-stepper-horizontal {
      background: transparent;
    }

    ::ng-deep .mat-horizontal-stepper-header-container {
      display: none !important;
    }

    ::ng-deep .mat-stepper-horizontal-line {
      display: none !important;
    }

    ::ng-deep .mat-step-header {
      display: none !important;
    }
  `]
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly inventoryService = inject(InventoryService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<ProductFormComponent>);

  basicInfoGroup!: FormGroup;
  pricingGroup!: FormGroup;
  inventoryGroup!: FormGroup;
  isEditMode = false;
  isSubmitting = false;
  categories: string[] = [];
  warehouses: Warehouse[] = [];
  productInventory: any = null; // Store the inventory data for editing

  constructor(@Inject(MAT_DIALOG_DATA) public data: Product | undefined) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;
    this.initForm();
    this.loadCategories();
    this.loadWarehouses();
    
    if (!this.isEditMode) {
      // Watch for category changes to auto-generate SKU
      this.basicInfoGroup.get('category')?.valueChanges.subscribe(category => {
        if (category) {
          this.generateSKU(category);
        }
      });
    } else if (this.data) {
      // Load inventory data for this product
      this.loadProductInventory(this.data.id);
    }
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        this.toastr.error('Failed to load categories', 'Error');
        console.error(error);
      }
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
      },
      error: (error) => {
        this.toastr.error('Failed to load warehouses', 'Error');
        console.error(error);
      }
    });
  }

  loadProductInventory(productId: string): void {
    this.inventoryService.getPaged({ productId, pageNumber: 1, pageSize: 1 }).subscribe({
      next: (response) => {
        if (response.items && response.items.length > 0) {
          this.productInventory = response.items[0];
          // Populate the inventory form with existing data
          this.inventoryGroup.patchValue({
            warehouseId: this.productInventory.warehouseId,
            initialQuantity: this.productInventory.quantity,
            threshold: this.productInventory.threshold
          });
        }
      },
      error: (error) => {
        console.error('Failed to load inventory', error);
      }
    });
  }

  generateSKU(category: string): void {
    // Create a category prefix (first 4 letters uppercase)
    const prefix = category.toUpperCase().replace(/\s+/g, '-').substring(0, 4);
    
    // Fetch all products to find the last SKU for this category
    this.productService.getPaged({
      pageNumber: 1,
      pageSize: 1000, // Get enough to find the max
      searchTerm: '',
      category: category,
      sortBy: 'Name',
      sortDescending: false
    }).subscribe({
      next: (response) => {
        let maxNumber = 0;
        
        // Find the highest number for this category's SKU
        response.items.forEach(product => {
          if (product.sku && product.sku.startsWith(prefix)) {
            const parts = product.sku.split('-');
            if (parts.length === 2) {
              const number = parseInt(parts[1], 10);
              if (!isNaN(number) && number > maxNumber) {
                maxNumber = number;
              }
            }
          }
        });
        
        // Generate next SKU
        const nextNumber = (maxNumber + 1).toString().padStart(3, '0');
        const newSKU = `${prefix}-${nextNumber}`;
        
        // Set the SKU value (but keep it editable)
        this.basicInfoGroup.patchValue({ sku: newSKU });
      },
      error: (error) => {
        console.error('Failed to generate SKU', error);
        // Fallback: just use 001
        const newSKU = `${prefix}-001`;
        this.basicInfoGroup.patchValue({ sku: newSKU });
      }
    });
  }

  initForm(): void {
    if (this.isEditMode && this.data) {
      // Edit mode - all three groups including inventory
      this.basicInfoGroup = this.fb.group({
        name: [this.data.name, Validators.required],
        description: [this.data.description],
        category: [this.data.category, Validators.required]
      });
      
      this.pricingGroup = this.fb.group({
        unitPrice: [this.data.unitPrice, [Validators.required, Validators.min(0.01)]]
      });
      
      // Create inventory group (will be populated when data loads)
      this.inventoryGroup = this.fb.group({
        warehouseId: ['', Validators.required],
        initialQuantity: [0, [Validators.required, Validators.min(0)]],
        threshold: [10, [Validators.required, Validators.min(0)]]
      });
    } else {
      // Create mode - all three groups
      this.basicInfoGroup = this.fb.group({
        name: ['', Validators.required],
        sku: ['', Validators.required],
        description: [''],
        category: ['', Validators.required]
      });
      
      this.pricingGroup = this.fb.group({
        unitPrice: [0, [Validators.required, Validators.min(0.01)]]
      });
      
      this.inventoryGroup = this.fb.group({
        warehouseId: ['', Validators.required],
        initialQuantity: [0, [Validators.required, Validators.min(0)]],
        threshold: [10, [Validators.required, Validators.min(0)]]
      });
    }
  }

  isFormValid(): boolean {
    return this.basicInfoGroup.valid && this.pricingGroup.valid && this.inventoryGroup.valid;
  }

  onSubmit(): void {
    if (this.isFormValid() && !this.isSubmitting) {
      this.isSubmitting = true;

      if (this.isEditMode && this.data) {
        // Update product
        const updateDto: UpdateProductDto = {
          ...this.basicInfoGroup.value,
          ...this.pricingGroup.value
        };

        // Update product first, then update inventory
        this.productService.update(this.data.id, updateDto).pipe(
          switchMap(() => {
            // Update inventory if we have the inventory record
            if (this.productInventory) {
              const inventoryUpdateDto = {
                productId: this.data!.id,
                warehouseId: this.inventoryGroup.value.warehouseId,
                quantity: this.inventoryGroup.value.initialQuantity,
                threshold: this.inventoryGroup.value.threshold
              };
              return this.inventoryService.update(this.productInventory.id, inventoryUpdateDto);
            } else {
              // Create new inventory if none exists
              return this.inventoryService.create({
                productId: this.data!.id,
                warehouseId: this.inventoryGroup.value.warehouseId,
                quantity: this.inventoryGroup.value.initialQuantity,
                threshold: this.inventoryGroup.value.threshold
              });
            }
          })
        ).subscribe({
          next: () => {
            this.toastr.success('Product and inventory updated successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to update product', 'Error');
            console.error(error);
            this.isSubmitting = false;
          }
        });
      } else {
        // Create mode: Create product first, then inventory
        const createDto: CreateProductDto = {
          name: this.basicInfoGroup.value.name,
          sku: this.basicInfoGroup.value.sku,
          description: this.basicInfoGroup.value.description,
          category: this.basicInfoGroup.value.category,
          unitPrice: this.pricingGroup.value.unitPrice
        };

        const inventoryData = {
          warehouseId: this.inventoryGroup.value.warehouseId,
          quantity: this.inventoryGroup.value.initialQuantity,
          threshold: this.inventoryGroup.value.threshold
        };

        // Create product first, then create inventory record
        this.productService.create(createDto).pipe(
          switchMap(createdProduct => {
            // Now create the inventory record with the product ID
            return this.inventoryService.create({
              productId: createdProduct.id,
              warehouseId: inventoryData.warehouseId,
              quantity: inventoryData.quantity,
              threshold: inventoryData.threshold
            });
          })
        ).subscribe({
          next: () => {
            this.toastr.success('Product and inventory created successfully', 'Success');
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

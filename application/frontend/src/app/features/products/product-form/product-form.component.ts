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
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
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

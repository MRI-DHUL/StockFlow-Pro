import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { ToastrService } from 'ngx-toastr';
import { StockMovementService } from '../../../core/services/stock-movement.service';
import { ProductService } from '../../../core/services/product.service';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { StockMovement, Product, Warehouse, CreateStockMovementDto, MovementType } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-stock-movement-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './stock-movement-form.component.html',
  styleUrls: ['./stock-movement-form.component.scss']
})
export class StockMovementFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly movementService = inject(StockMovementService);
  private readonly productService = inject(ProductService);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<StockMovementFormComponent>);

  movementForm!: FormGroup;
  isSubmitting = false;
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  movementTypes = [
    { value: MovementType.In, label: 'In' },
    { value: MovementType.Out, label: 'Out' },
    { value: MovementType.Transfer, label: 'Transfer' },
    { value: MovementType.Adjustment, label: 'Adjustment' }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public data: StockMovement | undefined) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadWarehouses();
    this.initForm();
  }

  initForm(): void {
    this.movementForm = this.fb.group({
      productId: ['', Validators.required],
      type: ['', Validators.required],
      fromWarehouseId: [''],
      toWarehouseId: [''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      referenceNumber: [''],
      notes: ['']
    });

    // Add custom validation for warehouses based on type
    this.movementForm.get('type')?.valueChanges.subscribe((type) => {
      this.updateWarehouseValidation(type);
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products;
      },
      error: (error) => {
        this.toastr.error('Failed to load products', 'Error');
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

  updateWarehouseValidation(type: MovementType): void {
    const fromControl = this.movementForm.get('fromWarehouseId');
    const toControl = this.movementForm.get('toWarehouseId');

    // Clear existing validators
    fromControl?.clearValidators();
    toControl?.clearValidators();

    // Set validators based on movement type
    switch (type) {
      case MovementType.In:
        toControl?.setValidators([Validators.required]);
        break;
      case MovementType.Out:
        fromControl?.setValidators([Validators.required]);
        break;
      case MovementType.Transfer:
        fromControl?.setValidators([Validators.required]);
        toControl?.setValidators([Validators.required]);
        break;
      case MovementType.Adjustment:
        toControl?.setValidators([Validators.required]);
        break;
    }

    fromControl?.updateValueAndValidity();
    toControl?.updateValueAndValidity();
  }

  shouldShowFromWarehouse(): boolean {
    const type = this.movementForm.get('type')?.value;
    return type === MovementType.Out || type === MovementType.Transfer;
  }

  shouldShowToWarehouse(): boolean {
    const type = this.movementForm.get('type')?.value;
    return type === MovementType.In || type === MovementType.Transfer || type === MovementType.Adjustment;
  }

  onSubmit(): void {
    if (this.movementForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      const createDto: CreateStockMovementDto = {
        productId: this.movementForm.value.productId,
        type: this.movementForm.value.type,
        fromWarehouseId: this.movementForm.value.fromWarehouseId || undefined,
        toWarehouseId: this.movementForm.value.toWarehouseId || undefined,
        quantity: this.movementForm.value.quantity,
        referenceNumber: this.movementForm.value.referenceNumber || undefined,
        notes: this.movementForm.value.notes || undefined
      };

      this.movementService.create(createDto).subscribe({
        next: () => {
          this.toastr.success('Stock movement recorded successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.toastr.error('Failed to record stock movement', 'Error');
          console.error(error);
          this.isSubmitting = false;
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}

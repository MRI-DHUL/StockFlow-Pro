import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { PurchaseOrderService } from '../../../core/services/purchase-order.service';
import { ProductService } from '../../../core/services/product.service';
import { SupplierService } from '../../../core/services/supplier.service';
import { PurchaseOrder, Product, Supplier, CreatePurchaseOrderDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-purchase-order-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  templateUrl: './purchase-order-form.component.html',
  styleUrls: ['./purchase-order-form.component.scss']
})
export class PurchaseOrderFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly poService = inject(PurchaseOrderService);
  private readonly productService = inject(ProductService);
  private readonly supplierService = inject(SupplierService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<PurchaseOrderFormComponent>);

  poForm!: FormGroup;
  isSubmitting = false;
  products: Product[] = [];
  suppliers: Supplier[] = [];
  minDate = new Date();

  constructor(@Inject(MAT_DIALOG_DATA) public data: PurchaseOrder | undefined) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadSuppliers();
    this.initForm();
  }

  initForm(): void {
    this.poForm = this.fb.group({
      supplierId: ['', Validators.required],
      expectedDeliveryDate: [null, Validators.required],
      purchaseOrderItems: this.fb.array([], Validators.required)
    });

    // Add at least one item by default
    this.addPOItem();
  }

  get purchaseOrderItems(): FormArray {
    return this.poForm.get('purchaseOrderItems') as FormArray;
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

  loadSuppliers(): void {
    this.supplierService.getAll().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
      },
      error: (error) => {
        this.toastr.error('Failed to load suppliers', 'Error');
        console.error(error);
      }
    });
  }

  addPOItem(): void {
    const poItemGroup = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
    this.purchaseOrderItems.push(poItemGroup);
  }

  removePOItem(index: number): void {
    if (this.purchaseOrderItems.length > 1) {
      this.purchaseOrderItems.removeAt(index);
    } else {
      this.toastr.warning('Purchase order must have at least one item', 'Warning');
    }
  }

  onProductChange(index: number): void {
    const productId = this.purchaseOrderItems.at(index).get('productId')?.value;
    const product = this.products.find(p => p.id === productId);
    if (product) {
      this.purchaseOrderItems.at(index).patchValue({
        unitPrice: product.unitPrice
      });
    }
  }

  calculateTotal(): number {
    let total = 0;
    this.purchaseOrderItems.controls.forEach(control => {
      const quantity = control.get('quantity')?.value || 0;
      const unitPrice = control.get('unitPrice')?.value || 0;
      total += quantity * unitPrice;
    });
    return total;
  }

  onSubmit(): void {
    if (this.poForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      const createDto: CreatePurchaseOrderDto = {
        supplierId: this.poForm.value.supplierId,
        expectedDeliveryDate: this.poForm.value.expectedDeliveryDate,
        purchaseOrderItems: this.poForm.value.purchaseOrderItems
      };

      this.poService.create(createDto).subscribe({
        next: () => {
          this.toastr.success('Purchase order created successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.toastr.error('Failed to create purchase order', 'Error');
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

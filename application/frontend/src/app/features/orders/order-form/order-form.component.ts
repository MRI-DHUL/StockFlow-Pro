import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { OrderService } from '../../../core/services/order.service';
import { ProductService } from '../../../core/services/product.service';
import { Order, Product, CreateOrderDto, CreateOrderItemDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule
  ],
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss']
})
export class OrderFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly orderService = inject(OrderService);
  private readonly productService = inject(ProductService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<OrderFormComponent>);

  orderForm!: FormGroup;
  isSubmitting = false;
  products: Product[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) public data: Order | undefined) {}

  ngOnInit(): void {
    this.loadProducts();
    this.initForm();
  }

  initForm(): void {
    this.orderForm = this.fb.group({
      customerName: ['', Validators.required],
      customerEmail: ['', [Validators.email]],
      orderItems: this.fb.array([], Validators.required)
    });

    // Add at least one order item by default
    this.addOrderItem();
  }

  get orderItems(): FormArray {
    return this.orderForm.get('orderItems') as FormArray;
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

  addOrderItem(): void {
    const orderItemGroup = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    });
    this.orderItems.push(orderItemGroup);
  }

  removeOrderItem(index: number): void {
    if (this.orderItems.length > 1) {
      this.orderItems.removeAt(index);
    } else {
      this.toastr.warning('Order must have at least one item', 'Warning');
    }
  }

  getProductName(productId: string): string {
    const product = this.products.find(p => p.id === productId);
    return product ? `${product.name} - $${product.unitPrice}` : '';
  }

  calculateTotal(): number {
    let total = 0;
    this.orderItems.controls.forEach(control => {
      const productId = control.get('productId')?.value;
      const quantity = control.get('quantity')?.value || 0;
      const product = this.products.find(p => p.id === productId);
      if (product) {
        total += product.unitPrice * quantity;
      }
    });
    return total;
  }

  onSubmit(): void {
    if (this.orderForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      const createDto: CreateOrderDto = {
        customerName: this.orderForm.value.customerName,
        customerEmail: this.orderForm.value.customerEmail || undefined,
        orderItems: this.orderForm.value.orderItems
      };

      this.orderService.create(createDto).subscribe({
        next: () => {
          this.toastr.success('Order created successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.toastr.error('Failed to create order', 'Error');
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

import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ToastrService } from 'ngx-toastr';
import { InventoryService } from '../../../core/services/inventory.service';
import { InventoryItem, UpdateInventoryDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-inventory-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule
  ],
  template: `
    <h2 mat-dialog-title>Update Inventory</h2>
    <mat-dialog-content>
      <div class="inventory-info">
        <p><strong>Product:</strong> {{ data.productName }}</p>
        <p><strong>Warehouse:</strong> {{ data.warehouseName }}</p>
      </div>

      <form [formGroup]="inventoryForm" class="inventory-form">
        <mat-form-field appearance="outline">
          <mat-label>Quantity</mat-label>
          <input matInput type="number" formControlName="quantity">
          <mat-error *ngIf="inventoryForm.get('quantity')?.hasError('required')">
            Quantity is required
          </mat-error>
          <mat-error *ngIf="inventoryForm.get('quantity')?.hasError('min')">
            Quantity must be 0 or greater
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Threshold (Reorder Level)</mat-label>
          <input matInput type="number" formControlName="threshold">
          <mat-error *ngIf="inventoryForm.get('threshold')?.hasError('required')">
            Threshold is required
          </mat-error>
          <mat-error *ngIf="inventoryForm.get('threshold')?.hasError('min')">
            Threshold must be 0 or greater
          </mat-error>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!inventoryForm.valid || isSubmitting">
        {{ isSubmitting ? 'Saving...' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .inventory-info {
      background-color: #f5f5f5;
      padding: 16px;
      border-radius: 4px;
      margin-bottom: 20px;
    }

    .inventory-info p {
      margin: 8px 0;
    }

    .inventory-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      min-width: 400px;
    }
  `]
})
export class InventoryFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly inventoryService = inject(InventoryService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<InventoryFormComponent>);

  inventoryForm!: FormGroup;
  isSubmitting = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: InventoryItem) {}

  ngOnInit(): void {
    this.inventoryForm = this.fb.group({
      quantity: [this.data.quantity, [Validators.required, Validators.min(0)]],
      threshold: [this.data.threshold, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.inventoryForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const updateDto: UpdateInventoryDto = {
        productId: this.data.productId,
        warehouseId: this.data.warehouseId,
        quantity: this.inventoryForm.value.quantity,
        threshold: this.inventoryForm.value.threshold
      };

      this.inventoryService.update(this.data.id, updateDto).subscribe({
        next: () => {
          this.toastr.success('Inventory updated successfully', 'Success');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.toastr.error('Failed to update inventory', 'Error');
          console.error(error);
          this.isSubmitting = false;
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

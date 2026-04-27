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
  templateUrl: './inventory-form.component.html',
  styleUrls: ['./inventory-form.component.scss']
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

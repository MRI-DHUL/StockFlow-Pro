import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ToastrService } from 'ngx-toastr';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { Warehouse, CreateWarehouseDto, UpdateWarehouseDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-warehouse-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatCheckboxModule
  ],
  templateUrl: './warehouse-form.component.html',
  styleUrls: ['./warehouse-form.component.scss']
})
export class WarehouseFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly warehouseService = inject(WarehouseService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<WarehouseFormComponent>);

  warehouseForm!: FormGroup;
  isEditMode = false;
  isSubmitting = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Warehouse | undefined) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;
    this.initForm();
  }

  initForm(): void {
    if (this.isEditMode && this.data) {
      this.warehouseForm = this.fb.group({
        name: [this.data.name, Validators.required],
        location: [this.data.location, Validators.required],
        capacity: [this.data.capacity, [Validators.required, Validators.min(1)]]
      });
    } else {
      this.warehouseForm = this.fb.group({
        name: ['', Validators.required],
        location: ['', Validators.required],
        capacity: [0, [Validators.required, Validators.min(1)]]
      });
    }
  }

  onSubmit(): void {
    if (this.warehouseForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      if (this.isEditMode && this.data) {
        const updateDto: UpdateWarehouseDto = this.warehouseForm.value;
        this.warehouseService.update(this.data.id, updateDto).subscribe({
          next: () => {
            this.toastr.success('Warehouse updated successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to update warehouse', 'Error');
            console.error(error);
            this.isSubmitting = false;
          }
        });
      } else {
        const createDto: CreateWarehouseDto = this.warehouseForm.value;
        this.warehouseService.create(createDto).subscribe({
          next: () => {
            this.toastr.success('Warehouse created successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to create warehouse', 'Error');
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

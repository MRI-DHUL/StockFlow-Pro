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
  template: `
    <h2 mat-dialog-title>{{ isEditMode ? 'Edit Warehouse' : 'Add Warehouse' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="warehouseForm" class="warehouse-form">
        <mat-form-field appearance="outline">
          <mat-label>Warehouse Name</mat-label>
          <input matInput formControlName="name" placeholder="Enter warehouse name">
          <mat-error *ngIf="warehouseForm.get('name')?.hasError('required')">
            Warehouse name is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Location</mat-label>
          <input matInput formControlName="location" placeholder="Enter location">
          <mat-error *ngIf="warehouseForm.get('location')?.hasError('required')">
            Location is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Capacity</mat-label>
          <input matInput type="number" formControlName="capacity" placeholder="Enter capacity">
          <mat-error *ngIf="warehouseForm.get('capacity')?.hasError('required')">
            Capacity is required
          </mat-error>
          <mat-error *ngIf="warehouseForm.get('capacity')?.hasError('min')">
            Capacity must be greater than 0
          </mat-error>
        </mat-form-field>

        <mat-checkbox *ngIf="isEditMode" formControlName="isActive">
          Is Active
        </mat-checkbox>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!warehouseForm.valid || isSubmitting">
        {{ isSubmitting ? 'Saving...' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .warehouse-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
      min-width: 400px;
    }
  `]
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
        capacity: [this.data.capacity, [Validators.required, Validators.min(1)]],
        isActive: [this.data.isActive]
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

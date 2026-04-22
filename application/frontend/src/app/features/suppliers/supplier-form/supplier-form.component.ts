import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ToastrService } from 'ngx-toastr';
import { SupplierService } from '../../../core/services/supplier.service';
import { Supplier, CreateSupplierDto, UpdateSupplierDto } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-supplier-form',
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
    <h2 mat-dialog-title>{{ isEditMode ? 'Edit Supplier' : 'Add Supplier' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="supplierForm" class="supplier-form">
        <mat-form-field appearance="outline">
          <mat-label>Supplier Name</mat-label>
          <input matInput formControlName="name" placeholder="Enter supplier name">
          <mat-error *ngIf="supplierForm.get('name')?.hasError('required')">
            Supplier name is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Contact Name</mat-label>
          <input matInput formControlName="contactName" placeholder="Enter contact name">
          <mat-error *ngIf="supplierForm.get('contactName')?.hasError('required')">
            Contact name is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" placeholder="Enter email">
          <mat-error *ngIf="supplierForm.get('email')?.hasError('required')">
            Email is required
          </mat-error>
          <mat-error *ngIf="supplierForm.get('email')?.hasError('email')">
            Invalid email format
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Phone</mat-label>
          <input matInput formControlName="phone" placeholder="Enter phone number">
          <mat-error *ngIf="supplierForm.get('phone')?.hasError('required')">
            Phone is required
          </mat-error>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Address</mat-label>
          <textarea matInput formControlName="address" rows="3" placeholder="Enter address"></textarea>
          <mat-error *ngIf="supplierForm.get('address')?.hasError('required')">
            Address is required
          </mat-error>
        </mat-form-field>

        <mat-checkbox *ngIf="isEditMode" formControlName="isActive">
          Is Active
        </mat-checkbox>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!supplierForm.valid || isSubmitting">
        {{ isSubmitting ? 'Saving...' : 'Save' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .supplier-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 20px 0;
      min-width: 400px;
    }
  `]
})
export class SupplierFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly supplierService = inject(SupplierService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<SupplierFormComponent>);

  supplierForm!: FormGroup;
  isEditMode = false;
  isSubmitting = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Supplier | undefined) {}

  ngOnInit(): void {
    this.isEditMode = !!this.data;
    this.initForm();
  }

  initForm(): void {
    if (this.isEditMode && this.data) {
      this.supplierForm = this.fb.group({
        name: [this.data.name, Validators.required],
        contactName: [this.data.contactName, Validators.required],
        email: [this.data.email, [Validators.required, Validators.email]],
        phone: [this.data.phone, Validators.required],
        address: [this.data.address, Validators.required],
        isActive: [this.data.isActive]
      });
    } else {
      this.supplierForm = this.fb.group({
        name: ['', Validators.required],
        contactName: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        phone: ['', Validators.required],
        address: ['', Validators.required]
      });
    }
  }

  onSubmit(): void {
    if (this.supplierForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;

      if (this.isEditMode && this.data) {
        const updateDto: UpdateSupplierDto = this.supplierForm.value;
        this.supplierService.update(this.data.id, updateDto).subscribe({
          next: () => {
            this.toastr.success('Supplier updated successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to update supplier', 'Error');
            console.error(error);
            this.isSubmitting = false;
          }
        });
      } else {
        const createDto: CreateSupplierDto = this.supplierForm.value;
        this.supplierService.create(createDto).subscribe({
          next: () => {
            this.toastr.success('Supplier created successfully', 'Success');
            this.dialogRef.close(true);
          },
          error: (error) => {
            this.toastr.error('Failed to create supplier', 'Error');
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

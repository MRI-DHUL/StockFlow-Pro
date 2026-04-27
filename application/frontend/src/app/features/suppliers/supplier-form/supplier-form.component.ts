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
  templateUrl: './supplier-form.component.html',
  styleUrls: ['./supplier-form.component.scss']
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
        contactPerson: [this.data.contactInfo || '', Validators.required],
        email: [this.data.email || '', [Validators.required, Validators.email]],
        phone: [this.data.phone || '', Validators.required],
        address: ['', Validators.required],
        isActive: [true]
      });
    } else {
      this.supplierForm = this.fb.group({
        name: ['', Validators.required],
        contactPerson: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        phone: ['', Validators.required],
        address: ['', Validators.required],
        isActive: [true]
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

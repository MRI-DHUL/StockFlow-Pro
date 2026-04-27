import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { SupplierService } from '../../../core/services/supplier.service';
import { Supplier } from '../../../shared/models/domain.models';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog.component';
import { SupplierFormComponent } from '../supplier-form/supplier-form.component';

@Component({
  selector: 'app-supplier-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatDialogModule
  ],
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class SupplierListComponent implements OnInit {
  private readonly supplierService = inject(SupplierService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly fb = inject(FormBuilder);

  suppliers: Supplier[] = [];
  allSuppliers: Supplier[] = [];
  displayedColumns = ['name', 'contactInfo', 'email', 'phone', 'actions'];

  filterForm: FormGroup = this.fb.group({
    searchTerm: ['']
  });

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.supplierService.getAll().subscribe({
      next: (suppliers) => {
        this.allSuppliers = [...suppliers];
        this.applyFilters();
      },
      error: (error) => {
        this.toastr.error('Failed to load suppliers', 'Error');
        console.error(error);
      }
    });
  }

  applyFilters(): void {
    const searchTerm = this.filterForm.value.searchTerm?.toLowerCase() || '';
    
    if (!searchTerm) {
      this.suppliers = [...this.allSuppliers];
    } else {
      this.suppliers = this.allSuppliers.filter(supplier => 
        supplier.name.toLowerCase().includes(searchTerm) ||
        (supplier.email && supplier.email.toLowerCase().includes(searchTerm)) ||
        (supplier.phone && supplier.phone.toLowerCase().includes(searchTerm)) ||
        (supplier.contactInfo && supplier.contactInfo.toLowerCase().includes(searchTerm))
      );
    }
    
    this.cdr.detectChanges();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.applyFilters();
  }

  openSupplierDialog(supplier?: Supplier): void {
    const dialogRef = this.dialog.open(SupplierFormComponent, {
      width: '600px',
      data: supplier
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadSuppliers();
      }
    });
  }

  deleteSupplier(supplier: Supplier): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Supplier',
        message: `Are you sure you want to delete "${supplier.name}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.supplierService.delete(supplier.id).subscribe({
          next: () => {
            this.toastr.success('Supplier deleted successfully', 'Success');
            this.loadSuppliers();
          },
          error: (error) => {
            this.toastr.error('Failed to delete supplier', 'Error');
            console.error(error);
          }
        });
      }
    });
  }
}

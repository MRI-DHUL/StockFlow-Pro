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
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Suppliers Management</mat-card-title>
        <button mat-raised-button color="accent" (click)="openSupplierDialog()">
          <mat-icon>add</mat-icon>
          Add Supplier
        </button>
      </mat-card-header>
      <mat-card-content>
        <!-- Filters -->
        <div class="filters-container">
          <form [formGroup]="filterForm" class="filters-form">
            <mat-form-field appearance="outline">
              <mat-label>Search</mat-label>
              <input matInput formControlName="searchTerm" placeholder="Search name, email, or phone...">
            </mat-form-field>

            <button mat-raised-button color="primary" (click)="applyFilters()">
              <mat-icon>search</mat-icon>
              Search
            </button>
            <button mat-button (click)="resetFilters()">Reset</button>
          </form>
        </div>

        <div class="table-container">
          <table mat-table [dataSource]="suppliers" class="mat-elevation-z2">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let supplier">{{ supplier.name }}</td>
            </ng-container>

            <ng-container matColumnDef="contactInfo">
              <th mat-header-cell *matHeaderCellDef>Contact</th>
              <td mat-cell *matCellDef="let supplier">{{ supplier.contactInfo || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="email">
              <th mat-header-cell *matHeaderCellDef>Email</th>
              <td mat-cell *matCellDef="let supplier">{{ supplier.email }}</td>
            </ng-container>

            <ng-container matColumnDef="phone">
              <th mat-header-cell *matHeaderCellDef>Phone</th>
              <td mat-cell *matCellDef="let supplier">{{ supplier.phone || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let supplier">
                <button mat-icon-button color="primary" (click)="openSupplierDialog(supplier)">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="deleteSupplier(supplier)">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          <div class="no-data-container" *ngIf="suppliers.length === 0">
            <div class="no-data-message">No Data Found</div>
          </div>
        </div>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    mat-card {
      border-radius: 16px;
      box-shadow: 0 8px 16px rgba(0,0,0,0.1);
      margin: 24px;
      background: white;
      animation: fadeIn 0.4s ease-out;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }

    mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: white;
      border-bottom: 2px solid black;
      color: black;
      padding: 24px;
      margin: -16px -16px 24px -16px;
      border-radius: 16px 16px 0 0;

      mat-card-title {
        font-size: 1.8rem;
        font-weight: 600;
        margin: 0;
        color: black !important;
      }

      button {
        background: black !important;
        color: white !important;
        border-radius: 8px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);

        mat-icon {
          color: white;
        }
      }
    }

    .table-container {
      overflow-x: auto;
      margin: 20px 0;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
    }

    .filters-container {
      margin-bottom: 24px;
      padding: 20px;
      background: #fafafa;
      border-radius: 12px;
      border: 1px solid #e0e0e0;
    }

    .filters-form {
      display: flex;
      gap: 16px;
      align-items: center;
      flex-wrap: wrap;

      mat-form-field {
        flex: 1;
        min-width: 250px;
      }

      button {
        height: 40px;
        margin-top: 8px;
      }
    }

    table {
      width: 100%;
      background: white;

      th {
        background: #000000;
        color: white !important;
        font-weight: 700;
        font-size: 0.95rem;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        padding: 16px !important;
      }

      td {
        padding: 14px !important;
        font-size: 0.95rem;
        color: black !important;
      }

      tr {
        // No hover effect
      }

      mat-icon {
        // No hover effect
      }
    }

    .status-active {
      color: #000000;
      font-weight: 700;
      background: white;
      border: 2px solid #000000;
      padding: 6px 14px;
      border-radius: 16px;
      text-transform: uppercase;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
    }

    .status-inactive {
      color: white !important;
      font-weight: 700;
      background: #000000;
      padding: 6px 14px;
      border-radius: 16px;
      text-transform: uppercase;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
    }
  `]
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

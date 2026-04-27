import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { Warehouse } from '../../../shared/models/domain.models';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog.component';
import { WarehouseFormComponent } from '../warehouse-form/warehouse-form.component';

@Component({
  selector: 'app-warehouse-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule
  ],
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.scss']
})
export class WarehouseListComponent implements OnInit {
  private readonly warehouseService = inject(WarehouseService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);

  warehouses: Warehouse[] = [];
  displayedColumns = ['name', 'location', 'capacity', 'actions'];

  ngOnInit(): void {
    this.loadWarehouses();
  }

  loadWarehouses(): void {
    this.warehouseService.getAll().subscribe({
      next: (warehouses) => {
        this.warehouses = [...warehouses];
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load warehouses', 'Error');
        console.error(error);
      }
    });
  }

  openWarehouseDialog(warehouse?: Warehouse): void {
    const dialogRef = this.dialog.open(WarehouseFormComponent, {
      width: '600px',
      data: warehouse
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadWarehouses();
      }
    });
  }

  deleteWarehouse(warehouse: Warehouse): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Warehouse',
        message: `Are you sure you want to delete "${warehouse.name}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.warehouseService.delete(warehouse.id).subscribe({
          next: () => {
            this.toastr.success('Warehouse deleted successfully', 'Success');
            this.loadWarehouses();
          },
          error: (error) => {
            this.toastr.error('Failed to delete warehouse', 'Error');
            console.error(error);
          }
        });
      }
    });
  }
}

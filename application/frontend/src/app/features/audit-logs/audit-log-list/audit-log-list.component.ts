import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { ToastrService } from 'ngx-toastr';
import { AuditLogService } from '../../../core/services/audit-log.service';
import { AuditLog } from '../../../shared/models/domain.models';

@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Audit Logs</mat-card-title>
        <button mat-raised-button color="primary" (click)="exportLogs()">
          <mat-icon>download</mat-icon>
          Export Logs
        </button>
      </mat-card-header>
      <mat-card-content>
        <!-- Filters -->
        <div class="filters-container">
          <form [formGroup]="filterForm" class="filters-form">
            <mat-form-field appearance="outline">
              <mat-label>Entity Name</mat-label>
              <input matInput formControlName="entityName" placeholder="e.g., Product, Order">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Action</mat-label>
              <input matInput formControlName="action" placeholder="e.g., Created, Updated">
            </mat-form-field>

            <button mat-raised-button color="primary" (click)="applyFilters()">
              <mat-icon>filter_list</mat-icon>
              Apply
            </button>
            <button mat-button (click)="resetFilters()">Reset</button>
          </form>
        </div>

        <!-- Table -->
        <div class="table-container">
          <table mat-table [dataSource]="auditLogs" class="mat-elevation-z2">
            <ng-container matColumnDef="timestamp">
              <th mat-header-cell *matHeaderCellDef>Timestamp</th>
              <td mat-cell *matCellDef="let log">{{ log.timestamp | date:'short' }}</td>
            </ng-container>

            <ng-container matColumnDef="entityName">
              <th mat-header-cell *matHeaderCellDef>Entity</th>
              <td mat-cell *matCellDef="let log">{{ log.entityName }}</td>
            </ng-container>

            <ng-container matColumnDef="action">
              <th mat-header-cell *matHeaderCellDef>Action</th>
              <td mat-cell *matCellDef="let log">
                <span [class]="getActionClass(log.action)">{{ log.action }}</span>
              </td>
            </ng-container>

            <ng-container matColumnDef="performedBy">
              <th mat-header-cell *matHeaderCellDef>Performed By</th>
              <td mat-cell *matCellDef="let log">{{ log.performedBy || 'System' }}</td>
            </ng-container>

            <ng-container matColumnDef="ipAddress">
              <th mat-header-cell *matHeaderCellDef>IP Address</th>
              <td mat-cell *matCellDef="let log">{{ log.ipAddress || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="changes">
              <th mat-header-cell *matHeaderCellDef>Changes</th>
              <td mat-cell *matCellDef="let log">
                <button mat-icon-button *ngIf="log.changes" (click)="viewChanges(log)">
                  <mat-icon>visibility</mat-icon>
                </button>
                <span *ngIf="!log.changes">-</span>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
          <div class="no-data-container" *ngIf="auditLogs.length === 0">
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

    .filters-container {
      background: white;
      padding: 20px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      margin-bottom: 24px;
    }

    .filters-form {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
      align-items: center;

      mat-form-field {
        min-width: 220px;
        flex: 1;
      }

      button {
        border-radius: 8px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }
    }

    .table-container {
      overflow-x: auto;
      margin: 20px 0;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
    }

    table {
      width: 100%;
      background: white;

      th {
        background: white;
        border-bottom: 2px solid black;
        color: black !important;
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
    }

    .action-created {
      color: #27ae60;
      font-weight: 700;
      background: rgba(46, 204, 113, 0.1);
      padding: 6px 14px;
      border-radius: 16px;
      text-transform: uppercase;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
      display: inline-block;
    }

    .action-updated {
      color: #3498db;
      font-weight: 700;
      background: rgba(52, 152, 219, 0.1);
      padding: 6px 14px;
      border-radius: 16px;
      text-transform: uppercase;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
      display: inline-block;
    }

    .action-deleted {
      color: #e74c3c;
      font-weight: 700;
      background: rgba(231, 76, 60, 0.1);
      padding: 6px 14px;
      border-radius: 16px;
      text-transform: uppercase;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
      display: inline-block;
    }
  `]
})
export class AuditLogListComponent implements OnInit {
  private readonly auditLogService = inject(AuditLogService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  auditLogs: AuditLog[] = [];
  displayedColumns = ['timestamp', 'entityName', 'action', 'performedBy', 'ipAddress', 'changes'];

  filterForm: FormGroup = this.fb.group({
    entityName: [''],
    action: ['']
  });

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    const filters = { ...this.filterForm.value };

    // Remove empty values
    Object.keys(filters).forEach(key => {
      if (filters[key] === '' || filters[key] === null) {
        delete filters[key];
      }
    });

    this.auditLogService.getAll(filters).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
      },
      error: (error) => {
        this.toastr.error('Failed to load audit logs', 'Error');
        console.error(error);
      }
    });
  }

  exportLogs(): void {
    this.toastr.info('Export Logs functionality coming soon', 'Info');
    // TODO: Implement logs export functionality
  }

  applyFilters(): void {
    this.loadAuditLogs();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.loadAuditLogs();
  }

  viewChanges(log: AuditLog): void {
    if (log.changes) {
      this.toastr.info(log.changes, 'Changes', { timeOut: 10000 });
    }
  }

  getActionClass(action: string): string {
    if (action.includes('Created')) return 'action-created';
    if (action.includes('Updated')) return 'action-updated';
    if (action.includes('Deleted')) return 'action-deleted';
    return '';
  }
}

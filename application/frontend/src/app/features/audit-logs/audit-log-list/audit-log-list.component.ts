import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { AuditLogService } from '../../../core/services/audit-log.service';
import { AuditLog } from '../../../shared/models/domain.models';
import { AuditLogChangesDialogComponent } from '../audit-log-changes-dialog/audit-log-changes-dialog.component';

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
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss']
})
export class AuditLogListComponent implements OnInit {
  private readonly auditLogService = inject(AuditLogService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly dialog = inject(MatDialog);

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
        this.auditLogs = [...logs];
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load audit logs', 'Error');
        console.error(error);
      }
    });
  }

  exportLogs(): void {
    if (this.auditLogs.length === 0) {
      this.toastr.warning('No logs to export', 'Warning');
      return;
    }

    // Create CSV content
    const headers = ['Timestamp', 'Entity Name', 'Entity ID', 'Action', 'Performed By', 'IP Address'];
    const csvRows = [headers.join(',')];

    this.auditLogs.forEach(log => {
      const row = [
        new Date(log.timestamp).toISOString(),
        log.entityName,
        log.entityId,
        log.action,
        log.userName || 'System',
        log.ipAddress || 'N/A'
      ];
      csvRows.push(row.map(field => `"${String(field).replace(/"/g, '""')}"`).join(','));
    });

    const csvContent = csvRows.join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `audit-logs-${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    this.toastr.success('Audit logs exported successfully', 'Success');
  }

  applyFilters(): void {
    this.loadAuditLogs();
  }

  resetFilters(): void {
    this.filterForm.reset();
    this.loadAuditLogs();
  }

  viewChanges(log: AuditLog): void {
    this.dialog.open(AuditLogChangesDialogComponent, {
      width: '800px',
      maxWidth: '90vw',
      data: log
    });
  }

  getActionClass(action: string): string {
    switch (action.toLowerCase()) {
      case 'created': return 'action-created';
      case 'updated': return 'action-updated';
      case 'deleted': return 'action-deleted';
      default: return '';
    }
  }
}

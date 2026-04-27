import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
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
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss']
})
export class AuditLogListComponent implements OnInit {
  private readonly auditLogService = inject(AuditLogService);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

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
    this.toastr.info(`Viewing changes for ${log.entityName}`, 'Info');
    // TODO: Implement changes dialog
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

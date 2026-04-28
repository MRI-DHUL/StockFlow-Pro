import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { AuditLog } from '../../../shared/models/domain.models';

interface ChangeItem {
  field: string;
  oldValue: any;
  newValue: any;
}

@Component({
  selector: 'app-audit-log-changes-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatCardModule
  ],
  templateUrl: './audit-log-changes-dialog.component.html',
  styleUrls: ['./audit-log-changes-dialog.component.scss']
})
export class AuditLogChangesDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<AuditLogChangesDialogComponent>);
  changes: ChangeItem[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) public log: AuditLog) {
    this.parseChanges();
  }

  parseChanges(): void {
    try {
      const oldValues = this.log.oldValues ? JSON.parse(this.log.oldValues) : {};
      const newValues = this.log.newValues ? JSON.parse(this.log.newValues) : {};

      // Get all unique keys from both old and new values
      const allKeys = new Set([...Object.keys(oldValues), ...Object.keys(newValues)]);

      allKeys.forEach(key => {
        // Skip internal fields
        if (key.toLowerCase().includes('password') || 
            key.toLowerCase().includes('token') ||
            key === 'id' ||
            key === 'createdAt' ||
            key === 'updatedAt') {
          return;
        }

        this.changes.push({
          field: this.formatFieldName(key),
          oldValue: this.formatValue(oldValues[key]),
          newValue: this.formatValue(newValues[key])
        });
      });
    } catch (error) {
      console.error('Error parsing changes:', error);
    }
  }

  formatFieldName(field: string): string {
    // Convert camelCase to Title Case
    return field
      .replace(/([A-Z])/g, ' $1')
      .replace(/^./, str => str.toUpperCase())
      .trim();
  }

  formatValue(value: any): string {
    if (value === null || value === undefined) {
      return 'N/A';
    }
    if (typeof value === 'boolean') {
      return value ? 'Yes' : 'No';
    }
    if (typeof value === 'object') {
      return JSON.stringify(value, null, 2);
    }
    return String(value);
  }

  hasChanged(change: ChangeItem): boolean {
    return change.oldValue !== change.newValue;
  }

  onClose(): void {
    this.dialogRef.close();
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuditLog } from '../../shared/models/domain.models';

export interface AuditLogFilters {
  entityName?: string;
  action?: string;
  performedBy?: string;
  startDate?: Date;
  endDate?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auditlogs`;

  getAll(filters?: AuditLogFilters): Observable<AuditLog[]> {
    let params = new HttpParams();
    
    if (filters) {
      if (filters.entityName) params = params.set('entityName', filters.entityName);
      if (filters.action) params = params.set('action', filters.action);
      if (filters.performedBy) params = params.set('performedBy', filters.performedBy);
      if (filters.startDate) params = params.set('startDate', filters.startDate.toISOString());
      if (filters.endDate) params = params.set('endDate', filters.endDate.toISOString());
    }

    return this.http.get<AuditLog[]>(this.apiUrl, { params });
  }

  getByEntity(entityName: string, entityId: string): Observable<AuditLog[]> {
    return this.http.get<AuditLog[]>(`${this.apiUrl}/entity/${entityName}/${entityId}`);
  }
}

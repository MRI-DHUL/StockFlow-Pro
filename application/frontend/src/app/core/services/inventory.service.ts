import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { InventoryItem, UpdateInventoryDto } from '../../shared/models/domain.models';
import { PagedResponse } from '../models/auth.models';

export interface InventoryFilters {
  productId?: number;
  warehouseId?: number;
  minQuantity?: number;
  maxQuantity?: number;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/inventory`;

  getAll(): Observable<InventoryItem[]> {
    return this.http.get<InventoryItem[]>(this.apiUrl);
  }

  getPaged(filters?: InventoryFilters): Observable<PagedResponse<InventoryItem>> {
    let params = new HttpParams();
    
    if (filters) {
      if (filters.productId) params = params.set('productId', filters.productId.toString());
      if (filters.warehouseId) params = params.set('warehouseId', filters.warehouseId.toString());
      if (filters.minQuantity) params = params.set('minQuantity', filters.minQuantity.toString());
      if (filters.maxQuantity) params = params.set('maxQuantity', filters.maxQuantity.toString());
      if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
      if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());
      if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
      if (filters.sortDescending !== undefined) params = params.set('sortDescending', filters.sortDescending.toString());
    }

    return this.http.get<PagedResponse<InventoryItem>>(`${this.apiUrl}/paged`, { params });
  }

  getById(id: number): Observable<InventoryItem> {
    return this.http.get<InventoryItem>(`${this.apiUrl}/${id}`);
  }

  update(id: number, inventory: UpdateInventoryDto): Observable<InventoryItem> {
    return this.http.put<InventoryItem>(`${this.apiUrl}/${id}`, inventory);
  }

  getLowStock(threshold?: number): Observable<InventoryItem[]> {
    const params = threshold ? new HttpParams().set('threshold', threshold.toString()) : undefined;
    return this.http.get<InventoryItem[]>(`${this.apiUrl}/low-stock`, { params });
  }
}

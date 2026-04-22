import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StockMovement, CreateStockMovementDto, MovementType } from '../../shared/models/domain.models';

export interface StockMovementFilters {
  productId?: number;
  warehouseId?: number;
  movementType?: MovementType;
  startDate?: Date;
  endDate?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class StockMovementService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/stockmovements`;

  getAll(filters?: StockMovementFilters): Observable<StockMovement[]> {
    let params = new HttpParams();
    
    if (filters) {
      if (filters.productId) params = params.set('productId', filters.productId.toString());
      if (filters.warehouseId) params = params.set('warehouseId', filters.warehouseId.toString());
      if (filters.movementType !== undefined) params = params.set('movementType', filters.movementType.toString());
      if (filters.startDate) params = params.set('startDate', filters.startDate.toISOString());
      if (filters.endDate) params = params.set('endDate', filters.endDate.toISOString());
    }

    return this.http.get<StockMovement[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<StockMovement> {
    return this.http.get<StockMovement>(`${this.apiUrl}/${id}`);
  }

  create(movement: CreateStockMovementDto): Observable<StockMovement> {
    return this.http.post<StockMovement>(this.apiUrl, movement);
  }

  getByProduct(productId: number): Observable<StockMovement[]> {
    return this.http.get<StockMovement[]>(`${this.apiUrl}/product/${productId}`);
  }
}

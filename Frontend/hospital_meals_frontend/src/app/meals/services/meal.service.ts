import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { PagedResult } from '../../shared/models';
import type { MealViewModel, MealCreateRequest } from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class MealService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  listMeals(page: number, pageSize: number): Observable<PagedResult<MealViewModel>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<MealViewModel>>(`${this.base}/meals`, {
      params,
    });
  }

  getMealById(id: string): Observable<MealViewModel | null> {
    return this.http.get<MealViewModel | null>(`${this.base}/meals/${id}`);
  }

  createMeal(request: MealCreateRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/meals`, request);
  }
}

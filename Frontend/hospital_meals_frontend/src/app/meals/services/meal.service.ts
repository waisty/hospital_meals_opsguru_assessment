import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import { isSearchLongEnough } from '../../shared/constants/search.constants';
import type { PagedResult } from '../../shared/models';
import type { MealViewModel, MealCreateRequest, MealUpdateRequest } from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class MealService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  listMeals(
    page: number,
    pageSize: number,
    search?: string | null
  ): Observable<PagedResult<MealViewModel>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (isSearchLongEnough(search)) {
      params = params.set('search', search!.trim());
    }
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

  updateMeal(id: string, request: MealUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/meals/${id}`, request);
  }
}

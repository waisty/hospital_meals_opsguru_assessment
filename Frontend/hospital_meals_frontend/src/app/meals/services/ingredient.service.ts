import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import { isSearchLongEnough } from '../../shared/constants/search.constants';
import type { PagedResult } from '../../shared/models';
import type {
  IngredientViewModel,
  IngredientDetailViewModel,
  IngredientCreateRequest,
  IngredientUpdateRequest,
  IngredientExclusionNamesRequest,
  IngredientExclusionNamesResponse,
  SetIngredientAllergyExclusionsRequest,
  SetIngredientClinicalStateExclusionsRequest,
  SetIngredientDietTypeExclusionsRequest,
} from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class IngredientService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  listIngredients(
    page: number,
    pageSize: number,
    search?: string | null
  ): Observable<PagedResult<IngredientViewModel>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (isSearchLongEnough(search)) {
      params = params.set('search', search!.trim());
    }
    return this.http.get<PagedResult<IngredientViewModel>>(
      `${this.base}/ingredients`,
      { params }
    );
  }

  getExclusionNamesByIngredientIds(ingredientIds: string[]): Observable<IngredientExclusionNamesResponse> {
    return this.http.post<IngredientExclusionNamesResponse>(
      `${this.base}/ingredients/exclusion-names-by-ids`,
      { ingredientIds } as IngredientExclusionNamesRequest
    );
  }

  getIngredientById(id: string): Observable<IngredientViewModel | null> {
    return this.http.get<IngredientViewModel | null>(
      `${this.base}/ingredients/${id}`
    );
  }

  getIngredientDetailById(id: string): Observable<IngredientDetailViewModel | null> {
    return this.http.get<IngredientDetailViewModel | null>(
      `${this.base}/ingredients/${id}/detail`
    );
  }

  createIngredient(request: IngredientCreateRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/ingredients`, request);
  }

  updateIngredient(id: string, request: IngredientUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/ingredients/${id}`, request);
  }

  getIngredientAllergyExclusionIds(ingredientId: string): Observable<string[]> {
    return this.http.get<string[]>(
      `${this.base}/ingredients/${ingredientId}/allergies`
    );
  }

  setIngredientAllergyExclusions(
    ingredientId: string,
    request: SetIngredientAllergyExclusionsRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/ingredients/${ingredientId}/allergies`,
      request
    );
  }

  getIngredientClinicalStateExclusionIds(
    ingredientId: string
  ): Observable<string[]> {
    return this.http.get<string[]>(
      `${this.base}/ingredients/${ingredientId}/clinical-states`
    );
  }

  setIngredientClinicalStateExclusions(
    ingredientId: string,
    request: SetIngredientClinicalStateExclusionsRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/ingredients/${ingredientId}/clinical-states`,
      request
    );
  }

  getIngredientDietTypeExclusionIds(ingredientId: string): Observable<string[]> {
    return this.http.get<string[]>(
      `${this.base}/ingredients/${ingredientId}/diet-types`
    );
  }

  setIngredientDietTypeExclusions(
    ingredientId: string,
    request: SetIngredientDietTypeExclusionsRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/ingredients/${ingredientId}/diet-types`,
      request
    );
  }
}

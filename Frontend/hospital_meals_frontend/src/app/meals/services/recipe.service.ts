import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { PagedResult } from '../../shared/models';
import type {
  RecipeViewModel,
  RecipeDetailViewModel,
  RecipeCreateRequest,
  RecipeIngredientViewModel,
  SetRecipeIngredientsRequest,
} from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class RecipeService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  listRecipes(page: number, pageSize: number): Observable<PagedResult<RecipeViewModel>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<RecipeViewModel>>(`${this.base}/recipes`, {
      params,
    });
  }

  getRecipeById(id: string): Observable<RecipeViewModel | null> {
    return this.http.get<RecipeViewModel | null>(`${this.base}/recipes/${id}`);
  }

  getRecipeDetailById(id: string): Observable<RecipeDetailViewModel | null> {
    return this.http.get<RecipeDetailViewModel | null>(
      `${this.base}/recipes/${id}/detail`
    );
  }

  createRecipe(request: RecipeCreateRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/recipes`, request);
  }

  getRecipeIngredients(recipeId: string): Observable<RecipeIngredientViewModel[]> {
    return this.http.get<RecipeIngredientViewModel[]>(
      `${this.base}/recipes/${recipeId}/ingredients`
    );
  }

  setRecipeIngredients(
    recipeId: string,
    request: SetRecipeIngredientsRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/recipes/${recipeId}/ingredients`,
      request
    );
  }
}

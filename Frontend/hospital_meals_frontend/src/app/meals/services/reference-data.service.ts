import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type {
  AllergyViewModel,
  AllergyCreateRequest,
  AllergyCreateResponse,
  AllergyUpdateRequest,
  ClinicalStateViewModel,
  ClinicalStateCreateRequest,
  ClinicalStateCreateResponse,
  ClinicalStateUpdateRequest,
  DietTypeViewModel,
  DietTypeCreateRequest,
  DietTypeCreateResponse,
  DietTypeUpdateRequest,
} from '../../patient/models';

const API = '/api/v1';

/**
 * Reference data (allergies, clinical states, diet types) from the Meals API.
 * Meals maintains its own copy synced from the Patient service.
 */
@Injectable({ providedIn: 'root' })
export class ReferenceDataService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  // --- Allergies ---
  listAllergies(): Observable<AllergyViewModel[]> {
    return this.http.get<AllergyViewModel[]>(`${this.base}/allergies`);
  }

  getAllergyById(id: string): Observable<AllergyViewModel | null> {
    return this.http.get<AllergyViewModel | null>(`${this.base}/allergies/${id}`);
  }

  createAllergy(request: AllergyCreateRequest): Observable<AllergyCreateResponse> {
    return this.http.post<AllergyCreateResponse>(`${this.base}/allergies`, request);
  }

  updateAllergy(id: string, request: AllergyUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/allergies/${id}`, request);
  }

  // --- Clinical states ---
  listClinicalStates(): Observable<ClinicalStateViewModel[]> {
    return this.http.get<ClinicalStateViewModel[]>(`${this.base}/clinical-states`);
  }

  getClinicalStateById(id: string): Observable<ClinicalStateViewModel | null> {
    return this.http.get<ClinicalStateViewModel | null>(
      `${this.base}/clinical-states/${id}`
    );
  }

  createClinicalState(
    request: ClinicalStateCreateRequest
  ): Observable<ClinicalStateCreateResponse> {
    return this.http.post<ClinicalStateCreateResponse>(
      `${this.base}/clinical-states`,
      request
    );
  }

  updateClinicalState(
    id: string,
    request: ClinicalStateUpdateRequest
  ): Observable<void> {
    return this.http.put<void>(`${this.base}/clinical-states/${id}`, request);
  }

  // --- Diet types ---
  listDietTypes(): Observable<DietTypeViewModel[]> {
    return this.http.get<DietTypeViewModel[]>(`${this.base}/diet-types`);
  }

  getDietTypeById(id: string): Observable<DietTypeViewModel | null> {
    return this.http.get<DietTypeViewModel | null>(`${this.base}/diet-types/${id}`);
  }

  createDietType(request: DietTypeCreateRequest): Observable<DietTypeCreateResponse> {
    return this.http.post<DietTypeCreateResponse>(`${this.base}/diet-types`, request);
  }

  updateDietType(id: string, request: DietTypeUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/diet-types/${id}`, request);
  }
}

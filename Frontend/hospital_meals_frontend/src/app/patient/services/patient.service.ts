import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { PagedResult } from '../../shared/models';
import type {
  PatientViewModel,
  PatientWithDietTypeNameViewModel,
  PatientDetailViewModel,
  PatientCreateRequest,
  PatientCreateResponse,
  PatientUpdateRequest,
  PatientAllergiesUpdateRequest,
  BatchPatientAllergiesRequest,
  BatchPatientAllergiesResponse,
  PatientClinicalStatesUpdateRequest,
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
} from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.patient + API;
  }

  // --- Patients ---
  listPatients(page: number, pageSize: number): Observable<PagedResult<PatientWithDietTypeNameViewModel>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<PatientWithDietTypeNameViewModel>>(`${this.base}/patients`, { params });
  }

  getPatientById(id: string): Observable<PatientViewModel | null> {
    return this.http.get<PatientViewModel | null>(`${this.base}/patients/${id}`);
  }

  getPatientDetailById(id: string): Observable<PatientDetailViewModel | null> {
    return this.http.get<PatientDetailViewModel | null>(`${this.base}/patients/${id}/detail`);
  }

  createPatient(request: PatientCreateRequest): Observable<PatientCreateResponse> {
    return this.http.post<PatientCreateResponse>(`${this.base}/patients`, request);
  }

  updatePatient(id: string, request: PatientUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/patients/${id}`, request);
  }

  getPatientAllergyIds(patientId: string): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/patients/${patientId}/allergies`);
  }

  getAllergiesByPatientIds(patientIds: string[]): Observable<BatchPatientAllergiesResponse> {
    return this.http.post<BatchPatientAllergiesResponse>(`${this.base}/patients/allergies-by-ids`, {
      patientIds,
    } as BatchPatientAllergiesRequest);
  }

  updatePatientAllergies(
    patientId: string,
    request: PatientAllergiesUpdateRequest
  ): Observable<void> {
    return this.http.put<void>(`${this.base}/patients/${patientId}/allergies`, request);
  }

  getPatientClinicalStateIds(patientId: string): Observable<string[]> {
    return this.http.get<string[]>(`${this.base}/patients/${patientId}/clinical-states`);
  }

  updatePatientClinicalStates(
    patientId: string,
    request: PatientClinicalStatesUpdateRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/patients/${patientId}/clinical-states`,
      request
    );
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

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { PagedResult } from '../../shared/models';
import type {
  PatientRequestViewModel,
  PatientRequestCreateRequest,
  PatientRequestCreateResponse,
} from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class PatientRequestService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.meals + API;
  }

  listPatientRequests(
    page: number,
    pageSize: number
  ): Observable<PagedResult<PatientRequestViewModel>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<PatientRequestViewModel>>(
      `${this.base}/patient-requests`,
      { params }
    );
  }

  getPatientRequestById(id: string): Observable<PatientRequestViewModel | null> {
    return this.http.get<PatientRequestViewModel | null>(
      `${this.base}/patient-requests/${id}`
    );
  }

  /**
   * Create a patient meal request. Returns created response on success (201).
   * On rejection (e.g. unsafe for patient) the API returns 409 Conflict with
   * PatientRequestCreateResponse body (statusString, statusReason, unsafeIngredientId).
   */
  createPatientRequest(
    request: PatientRequestCreateRequest
  ): Observable<PatientRequestCreateResponse> {
    return this.http.post<PatientRequestCreateResponse>(
      `${this.base}/patient-requests`,
      request
    );
  }
}

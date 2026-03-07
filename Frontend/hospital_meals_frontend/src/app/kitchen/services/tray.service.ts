import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { PagedResult } from '../../shared/models';
import type {
  TrayViewModel,
  AdvanceTrayStateRequest,
  AdvanceTrayStateResponse,
} from '../models';
import { TrayState } from '../models';

const API = '/api/v1';

@Injectable({ providedIn: 'root' })
export class TrayService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);
  private get base() {
    return this.endpoints.kitchen + API;
  }

  /**
   * List trays with optional filters.
   * @param state - Filter by TrayState enum value
   * @param uncompletedOnly - If true, exclude Delivered and Retrieved
   */
  listTrays(
    page: number = 1,
    pageSize: number = 10,
    state?: TrayState,
    uncompletedOnly?: boolean
  ): Observable<PagedResult<TrayViewModel>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (state !== undefined) params = params.set('state', state);
    if (uncompletedOnly === true) params = params.set('uncompletedOnly', true);
    return this.http.get<PagedResult<TrayViewModel>>(`${this.base}/trays`, {
      params,
    });
  }

  advanceTrayState(
    request: AdvanceTrayStateRequest
  ): Observable<AdvanceTrayStateResponse> {
    return this.http.post<AdvanceTrayStateResponse>(
      `${this.base}/trays/advance-state`,
      request
    );
  }
}

import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, of } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';
import type { UserAuthRequest, UserAuthResponse } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly endpoints = inject(API_ENDPOINTS);

  /** JWT stored in memory (not localStorage) per security guidance. */
  private readonly tokenSignal = signal<string | null>(null);
  private readonly userSignal = signal<Omit<UserAuthResponse, 'authToken'> | null>(null);

  readonly token = this.tokenSignal.asReadonly();
  readonly user = this.userSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.tokenSignal() !== null);

  /** Whether the user can access the Patient dashboard (admin or patientAdmin). */
  readonly canAccessPatient = computed(() => {
    const u = this.userSignal();
    return u?.admin === true || u?.patientAdmin === true;
  });

  /** Whether the user can access the Meals dashboard (admin, mealsAdmin, or mealsUser). */
  readonly canAccessMeals = computed(() => {
    const u = this.userSignal();
    return u?.admin === true || u?.mealsAdmin === true || u?.mealsUser === true;
  });

  /** Whether the user can access the Kitchen dashboard (admin or kitchenUser). */
  readonly canAccessKitchen = computed(() => {
    const u = this.userSignal();
    return u?.admin === true || u?.kitchenUser === true;
  });

  /**
   * If the user has access to exactly one dashboard, returns that dashboard path; otherwise null.
   * Used to redirect non-admin users with single-area access from home.
   */
  getSingleDashboardRedirect(): '/patient' | '/meals' | '/kitchen' | null {
    const p = this.canAccessPatient();
    const m = this.canAccessMeals();
    const k = this.canAccessKitchen();
    const count = [p, m, k].filter(Boolean).length;
    if (count !== 1) return null;
    if (p) return '/patient';
    if (m) return '/meals';
    return '/kitchen';
  }

  login(request: UserAuthRequest): Observable<UserAuthResponse | null> {
    const url = `${this.endpoints.auth}/api/v1/login`;
    return this.http.post<UserAuthResponse>(url, request).pipe(
      tap((res) => {
        this.tokenSignal.set(res.authToken);
        this.userSignal.set({
          admin: res.admin,
          patientAdmin: res.patientAdmin,
          mealsAdmin: res.mealsAdmin,
          mealsUser: res.mealsUser,
          kitchenUser: res.kitchenUser,
        });
      }),
      catchError(() => of(null))
    );
  }

  logout(): void {
    this.tokenSignal.set(null);
    this.userSignal.set(null);
  }

  /** Returns current token for interceptors. */
  getToken(): string | null {
    return this.tokenSignal();
  }
}

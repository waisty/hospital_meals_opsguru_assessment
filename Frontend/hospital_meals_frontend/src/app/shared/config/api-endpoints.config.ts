import { InjectionToken } from '@angular/core';

/** Base URLs for backend APIs (no trailing slash). Used by domain services. */
export interface ApiEndpoints {
  auth: string;
  patient: string;
  meals: string;
  kitchen: string;
}

export const API_ENDPOINTS = new InjectionToken<ApiEndpoints>('API_ENDPOINTS', {
  providedIn: 'root',
  factory: () => ({
    auth: 'http://localhost:8080',
    patient: 'http://localhost:8081',
    meals: 'http://localhost:8082',
    kitchen: 'http://localhost:8083',
  }),
});

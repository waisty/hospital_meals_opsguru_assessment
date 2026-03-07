import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { API_ENDPOINTS } from '../../shared/config/api-endpoints.config';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const endpoints = inject(API_ENDPOINTS);
  const token = auth.getToken();

  if (!token) return next(req);

  // Do not attach token to auth login (same origin or auth base)
  const isAuthLogin =
    req.url.startsWith(endpoints.auth) && req.url.includes('/login');
  if (isAuthLogin) return next(req);

  const cloned = req.clone({
    setHeaders: { Authorization: `Bearer ${token}` },
  });
  return next(cloned);
};

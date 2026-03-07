import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Protects routes that only admins (user.admin) may access.
 * Redirects non-admins to the appropriate list: allergies, clinical-states, or diet-types.
 */
export const adminGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAdmin()) return true;
  const path = route.routeConfig?.path ?? '';
  if (path.includes('allergies')) return router.createUrlTree(['/patient', 'setup', 'allergies']);
  if (path.includes('clinical-states')) return router.createUrlTree(['/patient', 'setup', 'clinical-states']);
  if (path.includes('diet-types')) return router.createUrlTree(['/patient', 'setup', 'diet-types']);
  return router.createUrlTree(['/patient', 'patients']);
};

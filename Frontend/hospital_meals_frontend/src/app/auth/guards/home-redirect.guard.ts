import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Use on /home: redirects to the user's only dashboard if they have access to exactly one area.
 */
export const homeRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const redirect = auth.getSingleDashboardRedirect();
  if (redirect) return router.createUrlTree([redirect]);
  return true;
};

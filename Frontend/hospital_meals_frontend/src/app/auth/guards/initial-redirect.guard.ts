import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * For the index route (''): redirect to /login if not logged in, otherwise to /home.
 */
export const initialRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isLoggedIn()) return router.createUrlTree(['/home']);
  return router.createUrlTree(['/login']);
};

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Protects the Meals area: only admin, mealsAdmin, or mealsUser may access.
 * Use after authGuard. Redirects unauthorized (but logged-in) users to home.
 */
export const mealsGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.canAccessMeals()) return true;
  return router.createUrlTree(['/home']);
};

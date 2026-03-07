import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Protects Meals setup routes (ingredients, recipes, meals management).
 * Only admin or mealsAdmin may access. Use after authGuard and mealsGuard.
 * Redirects unauthorized users to /meals (meal-requests).
 */
export const mealsAdminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.canAccessMealsSetup()) return true;
  return router.createUrlTree(['/meals']);
};

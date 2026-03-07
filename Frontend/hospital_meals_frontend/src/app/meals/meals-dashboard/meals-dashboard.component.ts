import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppSidebarComponent, type SidebarNavItem } from '../../shared/components/app-sidebar/app-sidebar.component';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'app-meals-dashboard',
  standalone: true,
  imports: [AppSidebarComponent, RouterOutlet],
  templateUrl: './meals-dashboard.component.html',
  styleUrl: './meals-dashboard.component.scss',
})
export class MealsDashboardComponent {
  private readonly auth = inject(AuthService);

  protected readonly sidebarItems = computed<SidebarNavItem[]>(() => {
    const items: SidebarNavItem[] = [
      { path: '/meals/meal-requests', label: 'Meal Requests', exact: true },
    ];
    if (this.auth.canAccessMealsSetup()) {
      items.push({
        label: 'Setup',
        children: [
          { path: '/meals/setup/ingredients', label: 'Ingredients', exact: true },
          { path: '/meals/setup/recipes', label: 'Recipes' },
          { path: '/meals/setup/meals', label: 'Meals' },
        ],
      });
    }
    return items;
  });
}

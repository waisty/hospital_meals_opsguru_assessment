import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppSidebarComponent, type SidebarNavItem } from '../../shared/components/app-sidebar/app-sidebar.component';

@Component({
  selector: 'app-meals-dashboard',
  standalone: true,
  imports: [AppSidebarComponent, RouterOutlet],
  templateUrl: './meals-dashboard.component.html',
  styleUrl: './meals-dashboard.component.scss',
})
export class MealsDashboardComponent {
  protected readonly sidebarItems: SidebarNavItem[] = [
    { path: '/meals/meal-requests', label: 'Meal Requests', exact: true },
    {
      label: 'Setup',
      children: [
        {
          label: 'Ingredients',
          children: [
            { path: '/meals/setup/ingredients/allergy-exclusions', label: 'Allergy Exclusions' },
            { path: '/meals/setup/ingredients/clinical-state-exclusions', label: 'Clinical State Exclusions' },
            { path: '/meals/setup/ingredients/diet-type-exclusions', label: 'Diet Type Exclusions' },
          ],
        },
        { path: '/meals/setup/recipes', label: 'Recipes' },
        { path: '/meals/setup/meals', label: 'Meals' },
      ],
    },
  ];
}

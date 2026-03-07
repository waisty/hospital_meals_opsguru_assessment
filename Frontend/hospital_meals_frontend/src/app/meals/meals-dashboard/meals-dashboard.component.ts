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
        { path: '/meals/setup/ingredients', label: 'Ingredients', exact: true },
        { path: '/meals/setup/recipes', label: 'Recipes' },
        { path: '/meals/setup/meals', label: 'Meals' },
      ],
    },
  ];
}

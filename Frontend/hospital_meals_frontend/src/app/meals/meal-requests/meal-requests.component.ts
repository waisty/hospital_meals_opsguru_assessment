import { Component } from '@angular/core';
import { MealRequestListComponent } from './meal-request-list/meal-request-list.component';

@Component({
  selector: 'app-meal-requests',
  standalone: true,
  imports: [MealRequestListComponent],
  template: `
    <div class="meal-requests-view">
      <header class="page-header">
        <h2>Meal Requests</h2>
      </header>
      <app-meal-request-list />
    </div>
  `,
  styles: [
    `
      .meal-requests-view .page-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 1.5rem;
      }
      .meal-requests-view h2 {
        margin: 0;
        font-size: 1.5rem;
        font-weight: 600;
        color: #0f172a;
      }
    `,
  ],
})
export class MealRequestsComponent {}

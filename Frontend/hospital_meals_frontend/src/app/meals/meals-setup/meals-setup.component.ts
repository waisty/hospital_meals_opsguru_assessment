import { Component } from '@angular/core';
import { AddButtonComponent } from '../../shared/components/add-button/add-button.component';
import { MealListComponent } from './meal-list/meal-list.component';

@Component({
  selector: 'app-meals-setup',
  standalone: true,
  imports: [MealListComponent, AddButtonComponent],
  templateUrl: './meals-setup.component.html',
  styleUrl: './meals-setup.component.scss',
})
export class MealsSetupComponent {}

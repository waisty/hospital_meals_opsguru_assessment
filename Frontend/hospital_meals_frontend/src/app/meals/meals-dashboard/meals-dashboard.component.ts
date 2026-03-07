import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-meals-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './meals-dashboard.component.html',
  styleUrl: './meals-dashboard.component.scss',
})
export class MealsDashboardComponent {}

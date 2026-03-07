import { Component } from '@angular/core';
import { IngredientListComponent } from './ingredient-list/ingredient-list.component';

@Component({
  selector: 'app-ingredients',
  standalone: true,
  imports: [IngredientListComponent],
  templateUrl: './ingredients.component.html',
  styleUrl: './ingredients.component.scss',
})
export class IngredientsComponent {}

import { Component } from '@angular/core';
import { AddButtonComponent } from '../../shared/components/add-button/add-button.component';
import { IngredientListComponent } from './ingredient-list/ingredient-list.component';

@Component({
  selector: 'app-ingredients',
  standalone: true,
  imports: [IngredientListComponent, AddButtonComponent],
  templateUrl: './ingredients.component.html',
  styleUrl: './ingredients.component.scss',
})
export class IngredientsComponent {}

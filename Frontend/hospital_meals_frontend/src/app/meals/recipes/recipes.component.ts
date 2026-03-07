import { Component } from '@angular/core';
import { AddButtonComponent } from '../../shared/components/add-button/add-button.component';
import { RecipeListComponent } from './recipe-list/recipe-list.component';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [RecipeListComponent, AddButtonComponent],
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.scss',
})
export class RecipesComponent {}

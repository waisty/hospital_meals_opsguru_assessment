import { Component, input } from '@angular/core';
import type { RecipeExclusionSummary } from '../../../meals/utils/recipe-exclusions.util';

@Component({
  selector: 'app-recipe-exclusion-tags',
  standalone: true,
  templateUrl: './recipe-exclusion-tags.component.html',
  styleUrl: './recipe-exclusion-tags.component.scss',
})
export class RecipeExclusionTagsComponent {
  /** Aggregated exclusion summary for a recipe. When null or empty, shows muted dash. */
  readonly summary = input<RecipeExclusionSummary | null>(null);

  readonly showEmptyAsDash = input<boolean>(true);
}

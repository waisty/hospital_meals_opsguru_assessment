import { Component, input } from '@angular/core';
import type { RecipeExclusionSummary } from '../../../meals/utils/recipe-exclusions.util';
import { StatusBadgeComponent } from '../status-badge/status-badge.component';

@Component({
  selector: 'app-recipe-exclusion-tags',
  standalone: true,
  imports: [StatusBadgeComponent],
  templateUrl: './recipe-exclusion-tags.component.html',
  styleUrl: './recipe-exclusion-tags.component.scss',
})
export class RecipeExclusionTagsComponent {
  /** Aggregated exclusion summary for a recipe. When null or empty, shows muted dash. */
  readonly summary = input<RecipeExclusionSummary | null>(null);

  readonly showEmptyAsDash = input<boolean>(true);
}

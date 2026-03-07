import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import { MealService } from '../../services/meal.service';
import { AuthService } from '../../../auth/services/auth.service';
import type { RecipeDetailViewModel } from '../../models';
import type { MealViewModel } from '../../models';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { MealListComponent } from '../../meals-setup/meal-list/meal-list.component';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent, MealListComponent],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.scss',
})
export class RecipeDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly recipeService = inject(RecipeService);
  private readonly mealService = inject(MealService);
  private readonly auth = inject(AuthService);

  readonly detail = signal<RecipeDetailViewModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly showAddToMealModal = signal(false);
  readonly addingMealId = signal<string | null>(null);

  readonly canAddToMeal = this.auth.canAccessMealsSetup;

  constructor() {
    this.route.paramMap
      .pipe(
        filter((params) => !!params.get('id')),
        switchMap((params) => {
          const id = params.get('id')!;
          this.loading.set(true);
          this.error.set(null);
          return this.recipeService.getRecipeDetailById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load recipe.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();
  }

  formatQuantityUnit(quantity: number, unit: string | null): string {
    const q = Number(quantity);
    if (unit?.trim()) return `${q} ${unit.trim()}`;
    return String(q);
  }

  openAddToMealModal(): void {
    this.showAddToMealModal.set(true);
  }

  closeAddToMealModal(): void {
    this.showAddToMealModal.set(false);
    this.addingMealId.set(null);
  }

  onMealSelected(meal: MealViewModel): void {
    const d = this.detail();
    if (!d?.id) return;
    this.addingMealId.set(meal.id);
    this.mealService.addRecipeToMeal(meal.id, { recipeId: d.id }).subscribe({
      next: () => {
        this.addingMealId.set(null);
        this.closeAddToMealModal();
        this.error.set(null);
        this.detail.update((prev) => (prev ? { ...prev, mappedMealName: meal.name } : null));
      },
      error: (err) => {
        this.addingMealId.set(null);
        const body = err?.error as { existingMealName?: string } | undefined;
        const existingName = body?.existingMealName;
        this.error.set(
          existingName
            ? `This recipe is already assigned to meal "${existingName}".`
            : 'Failed to add recipe to meal.'
        );
      },
    });
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, map, of, switchMap, tap } from 'rxjs';
import { MealService } from '../../services/meal.service';
import { RecipeService } from '../../services/recipe.service';
import type { MealViewModel } from '../../models';
import type { RecipeViewModel } from '../../models';

@Component({
  selector: 'app-meal-edit',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './meal-edit.component.html',
  styleUrl: './meal-edit.component.scss',
})
export class MealEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly mealService = inject(MealService);
  private readonly recipeService = inject(RecipeService);

  readonly detail = signal<MealViewModel | null>(null);
  readonly recipes = signal<RecipeViewModel[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly formName = signal('');
  readonly formRecipeId = signal('');

  readonly isCreateMode = computed(() => this.route.snapshot.paramMap.get('id') === 'new');

  constructor() {
    this.recipeService
      .listRecipes(1, 500, null)
      .pipe(
        takeUntilDestroyed(),
        map((p) => p.items)
      )
      .subscribe((items) => this.recipes.set(items));

    this.route.paramMap
      .pipe(
        filter((params) => !!params.get('id')),
        switchMap((params) => {
          const id = params.get('id')!;
          this.loading.set(true);
          this.error.set(null);
          if (id === 'new') {
            this.detail.set({
              id: '',
              name: '',
              recipeId: '',
              disabled: false,
            });
            this.formName.set('');
            this.formRecipeId.set('');
            this.loading.set(false);
            return of(undefined);
          }
          return this.mealService.getMealById(id).pipe(
            tap({
              next: (meal) => {
                const d = meal ?? null;
                this.detail.set(d);
                if (d) {
                  this.formName.set(d.name);
                  this.formRecipeId.set(d.recipeId);
                }
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load meal.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();
  }

  private slugFromName(name: string): string {
    const slug = name
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/[^a-z0-9-]/g, '');
    return slug || 'meal-' + Math.random().toString(36).slice(2, 10);
  }

  save(): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    const name = this.formName().trim();
    if (!name) {
      this.error.set('Name is required.');
      return;
    }
    const recipeId = this.formRecipeId()?.trim() ?? '';
    if (!recipeId) {
      this.error.set('Recipe is required.');
      return;
    }
    this.error.set(null);
    this.saving.set(true);

    if (this.isCreateMode()) {
      const id = this.slugFromName(name);
      this.mealService
        .createMeal({ id, name, recipeId })
        .subscribe({
          next: () => {
            this.saving.set(false);
            this.router.navigate(['/meals/setup/meals', id]);
          },
          error: () => {
            this.saving.set(false);
            this.error.set('Failed to create meal. Please try again.');
          },
        });
    } else {
      this.mealService
        .updateMeal(d.id, { name, recipeId })
        .subscribe({
          next: () => {
            this.saving.set(false);
            this.router.navigate(['/meals/setup/meals', d.id]);
          },
          error: () => {
            this.saving.set(false);
            this.error.set('Failed to save meal. Please try again.');
          },
        });
    }
  }

  backToList(): void {
    this.router.navigate(['/meals/setup/meals']);
  }
}

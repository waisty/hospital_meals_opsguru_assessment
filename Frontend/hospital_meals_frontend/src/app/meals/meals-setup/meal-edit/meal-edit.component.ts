import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, forkJoin, of, switchMap, tap } from 'rxjs';
import { MealService } from '../../services/meal.service';
import { AuthService } from '../../../auth/services/auth.service';
import { RecipeListComponent } from '../../recipes/recipe-list/recipe-list.component';
import type { MealViewModel, MealRecipeViewModel } from '../../models';
import type { RecipeViewModel } from '../../models';

@Component({
  selector: 'app-meal-edit',
  standalone: true,
  imports: [FormsModule, RouterLink, RecipeListComponent],
  templateUrl: './meal-edit.component.html',
  styleUrl: './meal-edit.component.scss',
})
export class MealEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly mealService = inject(MealService);
  private readonly auth = inject(AuthService);

  readonly detail = signal<MealViewModel | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly formName = signal('');
  readonly formDescription = signal('');

  readonly showAddRecipeModal = signal(false);
  /** Recipes added in the UI this session; persisted when user clicks Save meal. */
  readonly pendingRecipeAdditions = signal<MealRecipeViewModel[]>([]);

  readonly isCreateMode = computed(() => this.route.snapshot.paramMap.get('id') === 'new');
  readonly canManageRecipes = this.auth.canAccessMealsSetup;

  readonly recipesForTable = computed(() => {
    const saved = (this.detail()?.recipes ?? []).map((r) => ({ ...r, pending: false as const }));
    const pending = this.pendingRecipeAdditions().map((r) => ({ ...r, pending: true as const }));
    return [...saved, ...pending];
  });

  readonly alreadySelectedRecipeIds = computed(() => {
    const saved = this.detail()?.recipes?.map((r) => r.recipeId) ?? [];
    const pending = this.pendingRecipeAdditions().map((r) => r.recipeId);
    return [...saved, ...pending];
  });

  constructor() {
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
              description: null,
              disabled: false,
              recipes: [],
              recipeCount: 0,
            });
            this.formName.set('');
            this.formDescription.set('');
            this.loading.set(false);
            return of(undefined);
          }
          return this.mealService.getMealById(id).pipe(
            tap({
              next: (meal) => {
                const d = meal ?? null;
                this.detail.set(d);
                this.pendingRecipeAdditions.set([]);
                if (d) {
                  this.formName.set(d.name);
                  this.formDescription.set(d.description ?? '');
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
    this.error.set(null);
    this.saving.set(true);
    const description = this.formDescription().trim() || null;

    if (this.isCreateMode()) {
      const id = this.slugFromName(name);
      this.mealService
        .createMeal({ id, name, description })
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
      const pending = this.pendingRecipeAdditions();
      this.mealService.updateMeal(d.id, { name, description }).subscribe({
        next: () => {
          if (pending.length === 0) {
            this.saving.set(false);
            this.refreshMealDetail();
            return;
          }
          const adds$ = pending.map((r) =>
            this.mealService.addRecipeToMeal(d.id, { recipeId: r.recipeId })
          );
          forkJoin(adds$).subscribe({
            next: () => {
              this.pendingRecipeAdditions.set([]);
              this.saving.set(false);
              this.refreshMealDetail();
            },
            error: () => {
              this.saving.set(false);
              this.error.set('Failed to add some recipes. Please try again.');
            },
          });
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to save meal. Please try again.');
        },
      });
    }
  }

  private refreshMealDetail(): void {
    const d = this.detail();
    if (!d?.id) return;
    this.mealService.getMealById(d.id).subscribe({
      next: (meal) => this.detail.set(meal ?? null),
    });
  }

  backToList(): void {
    this.router.navigate(['/meals/setup/meals']);
  }

  openAddRecipeModal(): void {
    this.showAddRecipeModal.set(true);
  }

  closeAddRecipeModal(): void {
    this.showAddRecipeModal.set(false);
  }

  private isRecipeAlreadySelected(recipeId: string): boolean {
    return this.alreadySelectedRecipeIds().includes(recipeId);
  }

  selectRecipe(recipe: RecipeViewModel): void {
    const d = this.detail();
    if (!d?.id || this.isRecipeAlreadySelected(recipe.id)) return;
    this.pendingRecipeAdditions.update((list) => [
      ...list,
      { recipeId: recipe.id, recipeName: recipe.name, disabled: false },
    ]);
    this.closeAddRecipeModal();
  }

  removePendingRecipe(recipeId: string): void {
    this.pendingRecipeAdditions.update((list) => list.filter((r) => r.recipeId !== recipeId));
  }

  setRecipeDisabled(recipe: MealRecipeViewModel, disabled: boolean): void {
    const d = this.detail();
    if (!d?.id) return;
    this.mealService
      .setMealRecipeDisabled(d.id, recipe.recipeId, { disabled })
      .subscribe({
        next: () => this.refreshMealDetail(),
        error: () => this.error.set('Failed to update recipe status.'),
      });
  }
}

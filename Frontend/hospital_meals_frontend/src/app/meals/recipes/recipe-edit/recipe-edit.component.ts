import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, map, of, switchMap, tap } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import { IngredientService } from '../../services/ingredient.service';
import { MealService } from '../../services/meal.service';
import { AuthService } from '../../../auth/services/auth.service';
import type {
  RecipeDetailViewModel,
  RecipeIngredientViewModel,
  IngredientViewModel,
} from '../../models';
import type { MealViewModel } from '../../models';
import { MealListComponent } from '../../meals-setup/meal-list/meal-list.component';

@Component({
  selector: 'app-recipe-edit',
  standalone: true,
  imports: [FormsModule, RouterLink, MealListComponent],
  templateUrl: './recipe-edit.component.html',
  styleUrl: './recipe-edit.component.scss',
})
export class RecipeEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly recipeService = inject(RecipeService);
  private readonly ingredientService = inject(IngredientService);
  private readonly mealService = inject(MealService);
  private readonly auth = inject(AuthService);

  readonly detail = signal<RecipeDetailViewModel | null>(null);
  readonly allIngredients = signal<IngredientViewModel[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly formName = signal('');
  readonly formDescription = signal<string | null>('');

  readonly isCreateMode = computed(() => this.route.snapshot.paramMap.get('id') === 'new');
  readonly showAddToMealModal = signal(false);
  readonly addingMealId = signal<string | null>(null);
  readonly canAddToMeal = this.auth.canAccessMealsSetup;

  readonly availableIngredients = computed(() => {
    const d = this.detail();
    const all = this.allIngredients();
    if (!d) return all;
    const assignedIds = new Set(d.ingredients.map((i) => i.ingredientId));
    return all.filter((ing) => !assignedIds.has(ing.id));
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
              mappedMealName: null,
              ingredients: [],
            });
            this.formName.set('');
            this.formDescription.set('');
            this.loading.set(false);
            return of(undefined);
          }
          return this.recipeService.getRecipeDetailById(id).pipe(
            tap({
              next: (detail) => {
                const d = detail ?? null;
                this.detail.set(d);
                if (d) {
                  this.formName.set(d.name);
                  this.formDescription.set(d.description ?? '');
                }
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

    this.ingredientService
      .listIngredients(1, 500, null)
      .pipe(
        takeUntilDestroyed(),
        map((paged) => paged.items)
      )
      .subscribe((items) => this.allIngredients.set(items));
  }

  removeIngredient(ing: RecipeIngredientViewModel): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            ingredients: prev.ingredients.filter((i) => i.ingredientId !== ing.ingredientId),
          }
        : null
    );
  }

  addIngredient(ingredientId: string): void {
    const d = this.detail();
    if (!d || this.saving() || !ingredientId) return;
    const all = this.allIngredients();
    const ing = all.find((i) => i.id === ingredientId);
    if (!ing) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            ingredients: [
              ...prev.ingredients,
              {
                ingredientId: ing.id,
                ingredientName: ing.name,
                quantity: 1,
                unit: null,
              },
            ],
          }
        : null
    );
  }

  setIngredientQuantity(ingredientId: string, quantity: number): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            ingredients: prev.ingredients.map((i) =>
              i.ingredientId === ingredientId ? { ...i, quantity } : i
            ),
          }
        : null
    );
  }

  setIngredientUnit(ingredientId: string, unit: string | null): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            ingredients: prev.ingredients.map((i) =>
              i.ingredientId === ingredientId ? { ...i, unit: unit?.trim() || null } : i
            ),
          }
        : null
    );
  }

  onIngredientSelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addIngredient(value);
      select.value = '';
    }
  }

  private slugFromName(name: string): string {
    const slug = name
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/[^a-z0-9-]/g, '');
    return slug || 'recipe-' + Math.random().toString(36).slice(2, 10);
  }

  save(): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    if (this.isCreateMode()) {
      const name = this.formName().trim();
      if (!name) {
        this.error.set('Name is required.');
        return;
      }
      const description = this.formDescription()?.trim() || null;
      const id = this.slugFromName(name);
      this.error.set(null);
      this.saving.set(true);
      this.recipeService
        .createRecipe({ id, name, description })
        .subscribe({
          next: () => {
            this.recipeService
              .setRecipeIngredients(id, { ingredients: d.ingredients })
              .subscribe({
                next: () => {
                  this.saving.set(false);
                  this.router.navigate(['/meals/setup/recipes', id]);
                },
                error: () => {
                  this.saving.set(false);
                  this.error.set('Failed to save ingredients. Please try again.');
                },
              });
          },
          error: () => {
            this.saving.set(false);
            this.error.set('Failed to create recipe. Please try again.');
          },
        });
    } else {
      const name = this.formName().trim();
      if (!name) {
        this.error.set('Name is required.');
        return;
      }
      const description = this.formDescription()?.trim() || null;
      this.error.set(null);
      this.saving.set(true);
      this.recipeService.updateRecipe(d.id, { name, description }).subscribe({
        next: () => {
          this.recipeService.setRecipeIngredients(d.id, { ingredients: d.ingredients }).subscribe({
            next: () => {
              this.saving.set(false);
              this.router.navigate(['/meals/setup/recipes', d.id]);
            },
            error: () => {
              this.saving.set(false);
              this.error.set('Failed to save ingredients. Please try again.');
            },
          });
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to save recipe. Please try again.');
        },
      });
    }
  }

  backToList(): void {
    this.router.navigate(['/meals/setup/recipes']);
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

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, forkJoin, map, of, switchMap } from 'rxjs';
import { PatientService } from '../../../patient/services/patient.service';
import { MealService } from '../../services/meal.service';
import { RecipeService } from '../../services/recipe.service';
import { PatientRequestService } from '../../services/patient-request.service';
import { RecipeListComponent } from '../../recipes/recipe-list/recipe-list.component';
import { RecipeExclusionTagsComponent } from '../../../shared/components/recipe-exclusion-tags/recipe-exclusion-tags.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { computeRecipeExclusionSummary } from '../../utils/recipe-exclusions.util';
import type { PatientDetailViewModel } from '../../../patient/models';
import type { MealViewModel, RecipeViewModel } from '../../models';
import type { RecipeDetailViewModel } from '../../models';
import type { IngredientExclusionNamesItem } from '../../models';

@Component({
  selector: 'app-meal-request-summary',
  standalone: true,
  imports: [RecipeListComponent, RecipeExclusionTagsComponent, StatusBadgeComponent],
  templateUrl: './meal-request-summary.component.html',
  styleUrl: './meal-request-summary.component.scss',
})
export class MealRequestSummaryComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);
  private readonly mealService = inject(MealService);
  private readonly recipeService = inject(RecipeService);
  private readonly patientRequestService = inject(PatientRequestService);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly patient = signal<PatientDetailViewModel | null>(null);
  readonly meal = signal<MealViewModel | null>(null);
  readonly recipe = signal<RecipeDetailViewModel | null>(null);
  readonly safetyCheck = signal<{ isSafe: boolean; statusReason: string | null; unsafeIngredientId: string | null } | null>(null);
  readonly exclusionsByIngredientId = signal<Record<string, IngredientExclusionNamesItem>>({});
  readonly dietTypeName = signal<string>('-');
  readonly showRecipeModal = signal(false);

  readonly patientDisplayName = computed(() => {
    const p = this.patient();
    if (!p) return '';
    return [p.firstName, p.middleName, p.lastName].filter(Boolean).join(' ').trim() || p.id;
  });

  readonly allergyNames = computed(() => {
    const p = this.patient();
    return (p?.allergies ?? []).map((a) => a.allergyName);
  });

  readonly clinicalStateNames = computed(() => {
    const p = this.patient();
    return (p?.clinicalStates ?? []).map((c) => c.clinicalStateName);
  });

  readonly isSafe = computed(() => this.safetyCheck()?.isSafe ?? true);

  /** True only when the user reached this page via Request meal -> patient -> meal -> recipe. */
  private readonly fromRequestFlow = (() => {
    const nav = this.router.getCurrentNavigation();
    return (nav?.extras?.state as { fromRequestFlow?: boolean } | undefined)?.fromRequestFlow === true;
  })();

  /** Meal name to display (from meal or recipe.mappedMealName). */
  readonly mealDisplayName = computed(() => {
    const m = this.meal();
    const r = this.recipe();
    if (m?.name) return m.name;
    return r?.mappedMealName ?? '-';
  });

  constructor() {
    const params = this.route.snapshot.queryParamMap;
    const hasRequiredParams = params.has('patientId') && params.has('recipeId');
    if (hasRequiredParams && !this.fromRequestFlow) {
      this.router.navigate(['/meals/meal-requests']);
      return;
    }

    this.route.queryParamMap
      .pipe(
        takeUntilDestroyed(),
        filter((params) => params.has('patientId') && params.has('recipeId')),
        switchMap((params) => {
          const patientId = params.get('patientId')!;
          const recipeId = params.get('recipeId')!;
          const mealId = params.get('mealId');
          this.loading.set(true);
          this.error.set(null);
          const meal$ = mealId
            ? this.mealService.getMealById(mealId).pipe(map((m) => m ?? null))
            : of(null);
          return forkJoin({
            patient: this.patientService.getPatientDetailById(patientId).pipe(map((p) => p ?? null)),
            meal: meal$,
            recipe: this.recipeService.getRecipeDetailById(recipeId).pipe(map((r) => r ?? null)),
            safety: this.patientRequestService.checkSafety(patientId, recipeId),
          }).pipe(
            map(({ patient, meal, recipe, safety }) => {
              this.patient.set(patient);
              this.meal.set(meal);
              this.recipe.set(recipe);
              this.safetyCheck.set(safety);
              this.loadDietTypeName(patient?.dietTypeId);
              this.exclusionsByIngredientId.set(recipe?.exclusionNamesByIngredientId ?? {});
            })
          );
        })
      )
      .subscribe({
        next: () => this.loading.set(false),
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to load summary.');
        },
      });

    this.route.queryParamMap.pipe(takeUntilDestroyed()).subscribe((params) => {
      if (!params.has('patientId') || !params.has('recipeId')) {
        this.loading.set(false);
      }
    });

  }

  private loadDietTypeName(dietTypeId: string | undefined): void {
    if (!dietTypeId) {
      this.dietTypeName.set('-');
      return;
    }
    this.patientService.listDietTypes().subscribe((types) => {
      const found = types.find((t) => t.id === dietTypeId);
      this.dietTypeName.set(found?.name ?? dietTypeId);
    });
  }

  getExclusions(ingredientId: string): IngredientExclusionNamesItem | null {
    return this.exclusionsByIngredientId()[ingredientId] ?? null;
  }

  /** Recipe-level exclusion summary (reusable display, same as recipe list). */
  recipeExclusionSummary() {
    const r = this.recipe();
    if (!r?.id || !r.ingredients?.length) return null;
    const recipeIngredientIds: Record<string, string[]> = {
      [r.id]: r.ingredients.map((i) => i.ingredientId),
    };
    const exclusionNamesByIngredientId = r.exclusionNamesByIngredientId ?? {};
    return computeRecipeExclusionSummary(r.id, recipeIngredientIds, exclusionNamesByIngredientId);
  }

  formatQuantityUnit(quantity: number, unit: string | null): string {
    const q = Number(quantity);
    if (unit?.trim()) return `${q} ${unit.trim()}`;
    return String(q);
  }

  cancel(): void {
    this.router.navigate(['/meals/meal-requests']);
  }

  openChangeRecipeModal(): void {
    this.showRecipeModal.set(true);
  }

  closeRecipeModal(): void {
    this.showRecipeModal.set(false);
  }

  onRecipeChange(recipe: RecipeViewModel): void {
    this.closeRecipeModal();
    const patientId = this.route.snapshot.queryParamMap.get('patientId');
    if (!patientId) return;
    this.router.navigate(['/meals/meal-requests/summary'], {
      queryParams: { patientId, recipeId: recipe.id },
      replaceUrl: true,
    });
  }

  submit(): void {
    const patientId = this.route.snapshot.queryParamMap.get('patientId');
    const recipeId = this.route.snapshot.queryParamMap.get('recipeId');
    if (!patientId || !recipeId) return;
    this.error.set(null);
    this.saving.set(true);
    this.patientRequestService.createPatientRequest({ patientId, recipeId }).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/meals/meal-requests']);
      },
      error: (err) => {
        this.saving.set(false);
        const status = err?.status;
        const body = err?.error as { statusReason?: string; statusString?: string } | undefined;
        const message = body?.statusReason ?? body?.statusString ?? 'Request failed. Please try again.';
        if (status === 409) {
          this.error.set(message);
        } else {
          this.error.set(message);
        }
      },
    });
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, forkJoin, switchMap, tap } from 'rxjs';
import { IngredientService } from '../../services/ingredient.service';
import { ReferenceDataService } from '../../services/reference-data.service';
import type { IngredientDetailViewModel } from '../../models';
import type { AllergyViewModel, ClinicalStateViewModel, DietTypeViewModel } from '../../../patient/models';

@Component({
  selector: 'app-ingredient-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './ingredient-detail.component.html',
  styleUrl: './ingredient-detail.component.scss',
})
export class IngredientDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly ingredientService = inject(IngredientService);
  private readonly referenceData = inject(ReferenceDataService);

  readonly detail = signal<IngredientDetailViewModel | null>(null);
  readonly allAllergies = signal<AllergyViewModel[]>([]);
  readonly allClinicalStates = signal<ClinicalStateViewModel[]>([]);
  readonly allDietTypes = signal<DietTypeViewModel[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly availableAllergies = computed(() => {
    const d = this.detail();
    const all = this.allAllergies();
    if (!d) return all;
    const assigned = new Set(d.allergyExclusionIds);
    return all.filter((a) => !assigned.has(a.id));
  });

  readonly availableClinicalStates = computed(() => {
    const d = this.detail();
    const all = this.allClinicalStates();
    if (!d) return all;
    const assigned = new Set(d.clinicalStateExclusionIds);
    return all.filter((c) => !assigned.has(c.id));
  });

  readonly availableDietTypes = computed(() => {
    const d = this.detail();
    const all = this.allDietTypes();
    if (!d) return all;
    const assigned = new Set(d.dietTypeExclusionIds);
    return all.filter((dt) => !assigned.has(dt.id));
  });

  constructor() {
    this.route.paramMap
      .pipe(
        filter((params) => !!params.get('id')),
        switchMap((params) => {
          const id = params.get('id')!;
          this.loading.set(true);
          this.error.set(null);
          return this.ingredientService.getIngredientDetailById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load ingredient.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();

    this.referenceData
      .listAllergies()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.allAllergies.set(list));
    this.referenceData
      .listClinicalStates()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.allClinicalStates.set(list));
    this.referenceData
      .listDietTypes()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.allDietTypes.set(list));
  }

  getAllergyName(id: string): string {
    return this.allAllergies().find((a) => a.id === id)?.name ?? id;
  }

  getClinicalStateName(id: string): string {
    return this.allClinicalStates().find((c) => c.id === id)?.name ?? id;
  }

  getDietTypeName(id: string): string {
    return this.allDietTypes().find((d) => d.id === id)?.name ?? id;
  }

  removeAllergyExclusion(allergyId: string): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? { ...prev, allergyExclusionIds: prev.allergyExclusionIds.filter((id) => id !== allergyId) }
        : null
    );
  }

  addAllergyExclusion(allergyId: string): void {
    const d = this.detail();
    if (!d || this.saving() || !allergyId) return;
    this.detail.update((prev) =>
      prev ? { ...prev, allergyExclusionIds: [...prev.allergyExclusionIds, allergyId] } : null
    );
  }

  removeClinicalStateExclusion(clinicalStateId: string): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            clinicalStateExclusionIds: prev.clinicalStateExclusionIds.filter((id) => id !== clinicalStateId),
          }
        : null
    );
  }

  addClinicalStateExclusion(clinicalStateId: string): void {
    const d = this.detail();
    if (!d || this.saving() || !clinicalStateId) return;
    this.detail.update((prev) =>
      prev
        ? { ...prev, clinicalStateExclusionIds: [...prev.clinicalStateExclusionIds, clinicalStateId] }
        : null
    );
  }

  removeDietTypeExclusion(dietTypeId: string): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            dietTypeExclusionIds: prev.dietTypeExclusionIds.filter((id) => id !== dietTypeId),
          }
        : null
    );
  }

  addDietTypeExclusion(dietTypeId: string): void {
    const d = this.detail();
    if (!d || this.saving() || !dietTypeId) return;
    this.detail.update((prev) =>
      prev ? { ...prev, dietTypeExclusionIds: [...prev.dietTypeExclusionIds, dietTypeId] } : null
    );
  }

  onAllergySelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addAllergyExclusion(value);
      select.value = '';
    }
  }

  onClinicalStateSelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addClinicalStateExclusion(value);
      select.value = '';
    }
  }

  onDietTypeSelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addDietTypeExclusion(value);
      select.value = '';
    }
  }

  saveExclusions(): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    this.error.set(null);
    this.saving.set(true);
    forkJoin({
      allergies: this.ingredientService.setIngredientAllergyExclusions(d.id, {
        allergyIds: d.allergyExclusionIds,
      }),
      clinicalStates: this.ingredientService.setIngredientClinicalStateExclusions(d.id, {
        clinicalStateIds: d.clinicalStateExclusionIds,
      }),
      dietTypes: this.ingredientService.setIngredientDietTypeExclusions(d.id, {
        dietTypeIds: d.dietTypeExclusionIds,
      }),
    }).subscribe({
      next: () => this.saving.set(false),
      error: () => {
        this.saving.set(false);
        this.error.set('Failed to save exclusions. Please try again.');
      },
    });
  }
}

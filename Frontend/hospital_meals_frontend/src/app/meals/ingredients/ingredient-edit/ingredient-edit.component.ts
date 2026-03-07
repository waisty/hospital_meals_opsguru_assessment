import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, forkJoin, of, switchMap, tap } from 'rxjs';
import { IngredientService } from '../../services/ingredient.service';
import { ReferenceDataService } from '../../services/reference-data.service';
import type { IngredientDetailViewModel } from '../../models';
import type { AllergyViewModel, ClinicalStateViewModel, DietTypeViewModel } from '../../../patient/models';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-ingredient-edit',
  standalone: true,
  imports: [FormsModule, RouterLink, StatusBadgeComponent],
  templateUrl: './ingredient-edit.component.html',
  styleUrl: './ingredient-edit.component.scss',
})
export class IngredientEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ingredientService = inject(IngredientService);
  private readonly referenceData = inject(ReferenceDataService);

  readonly detail = signal<IngredientDetailViewModel | null>(null);
  readonly allAllergies = signal<AllergyViewModel[]>([]);
  readonly allClinicalStates = signal<ClinicalStateViewModel[]>([]);
  readonly allDietTypes = signal<DietTypeViewModel[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly formName = signal('');
  readonly formDescription = signal<string | null>('');

  readonly isCreateMode = computed(() => this.route.snapshot.paramMap.get('id') === 'new');

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
          if (id === 'new') {
            this.detail.set({
              id: '',
              name: '',
              description: null,
              allergyExclusionIds: [],
              clinicalStateExclusionIds: [],
              dietTypeExclusionIds: [],
            });
            this.formName.set('');
            this.formDescription.set('');
            this.loading.set(false);
            return of(undefined);
          }
          return this.ingredientService.getIngredientDetailById(id).pipe(
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

  private slugFromName(name: string): string {
    const slug = name
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/[^a-z0-9-]/g, '');
    return slug || 'ing-' + Math.random().toString(36).slice(2, 10);
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
      this.ingredientService
        .createIngredient({ id, name, description })
        .subscribe({
          next: () => {
            forkJoin({
              allergies: this.ingredientService.setIngredientAllergyExclusions(id, {
                allergyIds: d.allergyExclusionIds,
              }),
              clinicalStates: this.ingredientService.setIngredientClinicalStateExclusions(id, {
                clinicalStateIds: d.clinicalStateExclusionIds,
              }),
              dietTypes: this.ingredientService.setIngredientDietTypeExclusions(id, {
                dietTypeIds: d.dietTypeExclusionIds,
              }),
            }).subscribe({
              next: () => {
                this.saving.set(false);
                this.router.navigate(['/meals/setup/ingredients', id]);
              },
              error: () => {
                this.saving.set(false);
                this.error.set('Failed to save exclusions. Please try again.');
              },
            });
          },
          error: () => {
            this.saving.set(false);
            this.error.set('Failed to create ingredient. Please try again.');
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
      this.ingredientService
        .updateIngredient(d.id, { name, description })
        .subscribe({
          next: () => {
            this.detail.update((prev) =>
              prev ? { ...prev, name, description } : null
            );
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
              next: () => {
                this.saving.set(false);
                this.router.navigate(['/meals/setup/ingredients', d.id]);
              },
              error: () => {
                this.saving.set(false);
                this.error.set('Failed to save exclusions. Please try again.');
              },
            });
          },
          error: () => {
            this.saving.set(false);
            this.error.set('Failed to save ingredient. Please try again.');
          },
        });
    }
  }

  backToList(): void {
    this.router.navigate(['/meals/setup/ingredients']);
  }
}

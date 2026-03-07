import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { IngredientService } from '../../services/ingredient.service';
import { ReferenceDataService } from '../../services/reference-data.service';
import type { IngredientDetailViewModel } from '../../models';
import type { AllergyViewModel, ClinicalStateViewModel, DietTypeViewModel } from '../../../patient/models';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';

@Component({
  selector: 'app-ingredient-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent],
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
  readonly error = signal<string | null>(null);

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
}

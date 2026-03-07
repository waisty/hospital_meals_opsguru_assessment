import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { MealService } from '../../services/meal.service';
import type { MealViewModel } from '../../models';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';

@Component({
  selector: 'app-meal-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent],
  templateUrl: './meal-detail.component.html',
  styleUrl: './meal-detail.component.scss',
})
export class MealDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly mealService = inject(MealService);

  readonly detail = signal<MealViewModel | null>(null);
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
          return this.mealService.getMealById(id).pipe(
            tap({
              next: (meal) => {
                this.detail.set(meal ?? null);
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
}

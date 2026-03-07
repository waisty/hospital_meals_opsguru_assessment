import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { RecipeService } from '../../services/recipe.service';
import type { RecipeDetailViewModel } from '../../models';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent],
  templateUrl: './recipe-detail.component.html',
  styleUrl: './recipe-detail.component.scss',
})
export class RecipeDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly recipeService = inject(RecipeService);

  readonly detail = signal<RecipeDetailViewModel | null>(null);
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
}

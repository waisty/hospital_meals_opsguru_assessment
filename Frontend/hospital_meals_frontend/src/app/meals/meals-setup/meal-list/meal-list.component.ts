import { Component, Injector, computed, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { catchError, debounceTime, map, of, startWith, switchMap, tap } from 'rxjs';
import type { MealViewModel, RecipeViewModel } from '../../models';
import { MealService } from '../../services/meal.service';
import { RecipeService } from '../../services/recipe.service';
import type { PagedResult } from '../../../shared/models';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-meal-list',
  standalone: true,
  imports: [PaginationComponent, EditButtonComponent],
  templateUrl: './meal-list.component.html',
  styleUrl: './meal-list.component.scss',
})
export class MealListComponent {
  private readonly mealService = inject(MealService);
  private readonly recipeService = inject(RecipeService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly showEditButton = input<boolean>(false);
  readonly clickToNavigateEnabled = input<boolean>(true);
  readonly suspendLoadUntilSearch = input<boolean>(false);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly searchTerm = signal('');
  readonly listError = signal<string | null>(null);

  readonly recipes = toSignal(
    this.recipeService.listRecipes(1, 500, null).pipe(map((p) => p.items)),
    { initialValue: [] as RecipeViewModel[] }
  );

  private readonly debouncedSearch = toSignal(
    toObservable(computed(() => this.searchTerm()), { injector: this.injector }).pipe(
      debounceTime(300),
      startWith('')
    ),
    { initialValue: '' }
  );

  readonly isLoadSuspended = computed(
    () => this.suspendLoadUntilSearch() && !isSearchLongEnough(this.debouncedSearch())
  );

  private readonly params$ = toObservable(
    computed(() => {
      const search = this.debouncedSearch();
      const suspended = this.suspendLoadUntilSearch() && !isSearchLongEnough(search);
      return {
        suspended,
        page: this.page(),
        pageSize: this.pageSize(),
        search: isSearchLongEnough(search) ? search ?? '' : '',
      };
    }),
    { injector: this.injector }
  ).pipe(
    tap(() => this.listError.set(null)),
    switchMap(({ suspended, page, pageSize, search }) =>
      suspended
        ? of(null)
        : this.mealService.listMeals(page, pageSize, search).pipe(
            catchError((err) => {
              console.error('Failed to load meals', err);
              this.listError.set('Failed to load meals.');
              return of(null);
            })
          )
    )
  );

  readonly result = toSignal<PagedResult<MealViewModel> | null>(this.params$, {
    initialValue: null,
  });

  get items(): MealViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  getRecipeName(recipeId: string): string {
    return this.recipes().find((r) => r.id === recipeId)?.name ?? recipeId;
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/meals/setup/meals', id]);
  }

  onRowClick(mealId: string): void {
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(mealId);
    }
  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  onPageChange(p: number): void {
    this.page.set(p);
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }
}

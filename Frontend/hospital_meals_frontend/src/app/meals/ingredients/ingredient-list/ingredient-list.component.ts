import { Component, Injector, computed, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { catchError, debounceTime, map, of, startWith, switchMap, tap } from 'rxjs';
import type { IngredientViewModel } from '../../models';
import { IngredientService } from '../../services/ingredient.service';
import type { PagedResult } from '../../../shared/models';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';
import type { IngredientExclusionNamesItem } from '../../models';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-ingredient-list',
  standalone: true,
  imports: [PaginationComponent, EditButtonComponent],
  templateUrl: './ingredient-list.component.html',
  styleUrl: './ingredient-list.component.scss',
})
export class IngredientListComponent {
  private readonly ingredientService = inject(IngredientService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly showEditButton = input<boolean>(false);
  readonly clickToNavigateEnabled = input<boolean>(true);
  readonly suspendLoadUntilSearch = input<boolean>(false);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly searchTerm = signal('');
  readonly listError = signal<string | null>(null);

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
        : this.ingredientService.listIngredients(page, pageSize, search).pipe(
            catchError((err) => {
              console.error('Failed to load ingredients', err);
              this.listError.set('Failed to load ingredients.');
              return of(null);
            })
          )
    )
  );

  readonly result = toSignal<PagedResult<IngredientViewModel> | null>(this.params$, {
    initialValue: null,
  });

  get items(): IngredientViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  private readonly items$ = toObservable(
    computed(() => this.result()?.items ?? []),
    { injector: this.injector }
  );

  readonly exclusionsByIngredientId = toSignal(
    this.items$.pipe(
      switchMap((items) => {
        const ids = items.map((i) => i.id);
        if (ids.length === 0)
          return of({} as Record<string, IngredientExclusionNamesItem>);
        return this.ingredientService.getExclusionNamesByIngredientIds(ids).pipe(
          map((res) => {
            const map: Record<string, IngredientExclusionNamesItem> = {};
            for (const it of res.items ?? []) {
              map[it.ingredientId] = it;
            }
            return map;
          })
        );
      })
    ),
    { initialValue: {} as Record<string, IngredientExclusionNamesItem> }
  );

  getAllergyNames(ingredientId: string): string[] {
    return this.exclusionsByIngredientId()[ingredientId]?.allergyNames ?? [];
  }

  getClinicalStateNames(ingredientId: string): string[] {
    return this.exclusionsByIngredientId()[ingredientId]?.clinicalStateNames ?? [];
  }

  getDietTypeNames(ingredientId: string): string[] {
    return this.exclusionsByIngredientId()[ingredientId]?.dietTypeNames ?? [];
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/meals/setup/ingredients', id]);
  }

  onRowClick(ingredientId: string): void {
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(ingredientId);
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

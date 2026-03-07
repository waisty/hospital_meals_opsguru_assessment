import { Component, Injector, computed, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { catchError, debounceTime, of, startWith, switchMap, tap } from 'rxjs';
import type { RecipeViewModel } from '../../models';
import { RecipeService } from '../../services/recipe.service';
import type { PagedResult } from '../../../shared/models';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [PaginationComponent, EditButtonComponent],
  templateUrl: './recipe-list.component.html',
  styleUrl: './recipe-list.component.scss',
})
export class RecipeListComponent {
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
        : this.recipeService.listRecipes(page, pageSize, search).pipe(
            catchError((err) => {
              console.error('Failed to load recipes', err);
              this.listError.set('Failed to load recipes.');
              return of(null);
            })
          )
    )
  );

  readonly result = toSignal<PagedResult<RecipeViewModel> | null>(this.params$, {
    initialValue: null,
  });

  get items(): RecipeViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/meals/setup/recipes', id]);
  }

  onRowClick(recipeId: string): void {
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(recipeId);
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

import { Component, Injector, computed, inject, input, output, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { catchError, debounceTime, map, of, startWith, switchMap, tap } from 'rxjs';
import type { RecipeViewModel, RecipeExclusionNamesItemViewModel } from '../../models';
import type { RecipeExclusionSummary } from '../../utils/recipe-exclusions.util';
import { RecipeService } from '../../services/recipe.service';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { RecipeExclusionTagsComponent } from '../../../shared/components/recipe-exclusion-tags/recipe-exclusion-tags.component';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [PaginationComponent, EditButtonComponent, RecipeExclusionTagsComponent],
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

  /** When true, show a Select column and emit recipeSelected instead of navigating. */
  readonly selectionMode = input<boolean>(false);
  /** When false (e.g. in request-flow popup), hide the Select column and select on row click. Default true. */
  readonly showSelectButton = input<boolean>(true);
  /** Recipe IDs that are already selected (e.g. already added to meal); show "Already added" and no Select button. */
  readonly alreadySelectedIds = input<string[]>([]);
  /** When set, the recipe with this ID shows "Adding…" and Select is disabled. */
  readonly addingRecipeId = input<string | null>(null);
  readonly recipeSelected = output<RecipeViewModel>();

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

  readonly result = toSignal(this.params$, {
    initialValue: null as { items: RecipeViewModel[]; totalCount: number; page: number; pageSize: number } | null,
  });

  private readonly result$ = toObservable(computed(() => this.result()), { injector: this.injector });

  /** Exclusion names per recipe ID (from subsequent getExclusionNamesByRecipeIds call). */
  readonly exclusionByRecipeId = toSignal(
    this.result$.pipe(
      switchMap((res) => {
        const items = res?.items ?? [];
        if (items.length === 0) return of({} as Record<string, RecipeExclusionNamesItemViewModel>);
        return this.recipeService.getExclusionNamesByRecipeIds(items.map((r) => r.id)).pipe(
          map((response) => {
            const map: Record<string, RecipeExclusionNamesItemViewModel> = {};
            for (const item of response.items ?? []) {
              map[item.recipeId] = item;
            }
            return map;
          }),
          catchError(() => of({} as Record<string, RecipeExclusionNamesItemViewModel>))
        );
      })
    ),
    { initialValue: {} as Record<string, RecipeExclusionNamesItemViewModel> }
  );

  get items(): RecipeViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  getExclusionSummary(recipeId: string): RecipeExclusionSummary | null {
    const item = this.exclusionByRecipeId()[recipeId];
    if (!item) return null;
    return {
      allergyNames: item.allergyNames ?? [],
      clinicalStateNames: item.clinicalStateNames ?? [],
      dietTypeNames: item.dietTypeNames ?? [],
    };
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/meals/setup/recipes', id]);
  }

  onRowClick(recipe: RecipeViewModel): void {
    if (this.selectionMode()) {
      if (!this.showSelectButton() && !this.isAlreadySelected(recipe.id) && this.addingRecipeId() === null) {
        this.recipeSelected.emit(recipe);
      }
      return;
    }
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(recipe.id);
    }
  }

  isAlreadySelected(recipeId: string): boolean {
    return this.alreadySelectedIds().includes(recipeId);
  }

  onSelectRecipe(recipe: RecipeViewModel): void {
    if (this.isAlreadySelected(recipe.id) || this.addingRecipeId() !== null) return;
    this.recipeSelected.emit(recipe);
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

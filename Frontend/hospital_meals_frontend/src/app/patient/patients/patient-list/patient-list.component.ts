import { Component, Injector, computed, inject, input, output, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { catchError, debounceTime, forkJoin, map, of, startWith, switchMap, tap } from 'rxjs';
import type { PatientWithDietTypeNameViewModel } from '../../models';
import { PatientService } from '../../services/patient.service';
import type { PagedResult } from '../../../shared/models';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [PaginationComponent, EditButtonComponent, StatusBadgeComponent],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss',
})
export class PatientListComponent {
  private readonly patientService = inject(PatientService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly showEditButton = input<boolean>(true);
  readonly clickToNavigateEnabled = input<boolean>(true);

  /** When true, row click emits patientSelected and does not navigate. */
  readonly selectionMode = input<boolean>(false);
  readonly patientSelected = output<PatientWithDietTypeNameViewModel>();

  /**
   * When true, the list is not loaded until the user has entered a search term (at least 2 characters).
   * While suspended, a hint is shown instead of loading or empty table.
   */
  readonly suspendLoadUntilSearch = input<boolean>(false);

  /**
   * When set, at most this many records are fetched (single page, page 1) and pagination controls are hidden.
   * When not set, normal pagination applies.
   */
  readonly maxRecords = input<number | undefined>();

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

  /** True when loading is suspended because suspendLoadUntilSearch is set and search has fewer than 2 characters. */
  readonly isLoadSuspended = computed(
    () => this.suspendLoadUntilSearch() && !isSearchLongEnough(this.debouncedSearch())
  );

  private readonly params$ = toObservable(
    computed(() => {
      const search = this.debouncedSearch();
      const suspended = this.suspendLoadUntilSearch() && !isSearchLongEnough(search);
      const max = this.maxRecords();
      return {
        suspended,
        page: max != null ? 1 : this.page(),
        pageSize: max != null ? max : this.pageSize(),
        search: isSearchLongEnough(search) ? search ?? '' : '',
      };
    }),
    { injector: this.injector }
  ).pipe(
    tap(() => this.listError.set(null)),
    switchMap(({ suspended, page, pageSize, search }) =>
      suspended
        ? of(null)
        : this.patientService.listPatients(page, pageSize, search).pipe(
            catchError((err) => {
              console.error('Failed to load patients', err);
              this.listError.set('Failed to load patients.');
              return of(null);
            })
          )
    )
  );
  readonly result = toSignal<PagedResult<PatientWithDietTypeNameViewModel> | null>(this.params$, {
    initialValue: null,
  });

  get items(): PatientWithDietTypeNameViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  /** When true, pagination is shown; when false (e.g. maxRecords is set), it is hidden. */
  readonly showPagination = computed(() => this.maxRecords() == null);

  private readonly items$ = toObservable(
    computed(() => this.result()?.items ?? []),
    { injector: this.injector }
  );
  readonly allergiesByPatientId = toSignal(
    this.items$.pipe(
      switchMap((items) => {
        const ids = items.map((p) => p.id);
        if (ids.length === 0)
          return of({ allergies: {} as Record<string, string[]>, clinicalStates: {} as Record<string, string[]> });
        return forkJoin({
          allergies: this.patientService.getAllergiesByPatientIds(ids).pipe(
            map((res) =>
              (res.items ?? []).reduce<Record<string, string[]>>(
                (acc, it) => {
                  acc[it.patientId] = it.allergyNames ?? [];
                  return acc;
                },
                {}
              )
            )
          ),
          clinicalStates: this.patientService.getClinicalStatesByPatientIds(ids).pipe(
            map((res) =>
              (res.items ?? []).reduce<Record<string, string[]>>(
                (acc, it) => {
                  acc[it.patientId] = it.clinicalStateNames ?? [];
                  return acc;
                },
                {}
              )
            )
          ),
        });
      })
    ),
    { initialValue: { allergies: {} as Record<string, string[]>, clinicalStates: {} as Record<string, string[]> } }
  );

  getAllergyNames(patientId: string): string[] {
    return this.allergiesByPatientId().allergies[patientId] ?? [];
  }

  getClinicalStateNames(patientId: string): string[] {
    return this.allergiesByPatientId().clinicalStates[patientId] ?? [];
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/patient/patients', id]);
  }

  onRowClick(patient: PatientWithDietTypeNameViewModel): void {
    if (this.selectionMode()) {
      this.patientSelected.emit(patient);
      return;
    }
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(patient.id);
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

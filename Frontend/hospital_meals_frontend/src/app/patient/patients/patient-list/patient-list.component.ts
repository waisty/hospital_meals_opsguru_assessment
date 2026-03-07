import { Component, Injector, computed, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { catchError, debounceTime, forkJoin, map, of, startWith, switchMap, tap } from 'rxjs';
import type { PatientWithDietTypeNameViewModel } from '../../models';
import { PatientService } from '../../services/patient.service';
import type { PagedResult } from '../../../shared/models';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [RouterLink, PaginationComponent],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss',
})
export class PatientListComponent {
  private readonly patientService = inject(PatientService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly showEditButton = input<boolean>(true);
  readonly clickToNavigateEnabled = input<boolean>(true);

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

  private readonly params$ = toObservable(
    computed(() => {
      const search = this.debouncedSearch();
      return {
        page: this.page(),
        pageSize: this.pageSize(),
        search: search.length >= 2 ? search : '',
      };
    }),
    { injector: this.injector }
  ).pipe(
    tap(() => this.listError.set(null)),
    switchMap(({ page, pageSize, search }) =>
      this.patientService.listPatients(page, pageSize, search).pipe(
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

  onRowClick(patientId: string): void {
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(patientId);
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

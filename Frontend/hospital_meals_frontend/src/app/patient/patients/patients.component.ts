import { Component, Injector, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, filter, map, of, switchMap, tap } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import type { PagedResult } from '../../shared/models';
import { PatientService } from '../services/patient.service';
import type { PatientWithDietTypeNameViewModel } from '../models';

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [RouterLink, PaginationComponent],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.scss',
})
export class PatientsComponent {
  private readonly patientService = inject(PatientService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly page = signal(1);
  readonly pageSize = signal(10);

  readonly listError = signal<string | null>(null);

  private readonly params$ = toObservable(
    computed(() => ({ page: this.page(), pageSize: this.pageSize() })),
    { injector: this.injector }
  ).pipe(
    tap(() => this.listError.set(null)),
    switchMap(({ page, pageSize }) =>
      this.patientService.listPatients(page, pageSize).pipe(
        catchError((err) => {
          this.listError.set(err?.message ?? 'Failed to load patients.');
          return of(null);
        })
      )
    )
  );
  readonly result = toSignal<PagedResult<PatientWithDietTypeNameViewModel> | null>(this.params$, {
    initialValue: null,
  });

  private readonly result$ = toObservable(computed(() => this.result()), {
    injector: this.injector,
  });
  readonly allergiesByPatientId = toSignal(
    this.result$.pipe(
      filter((r): r is PagedResult<PatientWithDietTypeNameViewModel> => r !== null),
      switchMap((r) => {
        const ids = r.items.map((p) => p.id);
        if (ids.length === 0) return of<Record<string, string[]>>({});
        return this.patientService.getAllergiesByPatientIds(ids).pipe(
          map((res) =>
            (res.items ?? []).reduce<Record<string, string[]>>(
              (acc, it) => {
                acc[it.patientId] = it.allergyNames ?? [];
                return acc;
              },
              {}
            )
          )
        );
      })
    ),
    { initialValue: {} as Record<string, string[]> }
  );

  get items(): PatientWithDietTypeNameViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  getAllergyNames(patientId: string): string[] {
    return this.allergiesByPatientId()[patientId] ?? [];
  }

  onPageChange(p: number): void {
    this.page.set(p);
  }
  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/patient/patients', id]);
  }
}

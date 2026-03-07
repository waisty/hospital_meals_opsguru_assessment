import { Component, Injector, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, of, switchMap, tap } from 'rxjs';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PatientListComponent } from './patient-list/patient-list.component';
import type { PagedResult } from '../../shared/models';
import { PatientService } from '../services/patient.service';
import type { PatientWithDietTypeNameViewModel } from '../models';

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [PatientListComponent, PaginationComponent],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.scss',
})
export class PatientsComponent {
  private readonly patientService = inject(PatientService);
  private readonly injector = inject(Injector);

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

  onPageChange(p: number): void {
    this.page.set(p);
  }
  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }
}

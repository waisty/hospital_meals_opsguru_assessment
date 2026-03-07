import { Component, Injector, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';
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

  private readonly params$ = toObservable(
    computed(() => ({ page: this.page(), pageSize: this.pageSize() })),
    { injector: this.injector }
  ).pipe(
    switchMap(({ page, pageSize }) =>
      this.patientService.listPatients(page, pageSize)
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

  navigateToDetail(id: string): void {
    this.router.navigate(['/patient/patients', id]);
  }
}

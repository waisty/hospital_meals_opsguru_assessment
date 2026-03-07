import { Component, Injector, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, of, switchMap, tap } from 'rxjs';
import type { PatientRequestViewModel } from '../../models';
import { PatientRequestService } from '../../services/patient-request.service';
import type { PagedResult } from '../../../shared/models';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { MealRequestApprovalStatus } from '../../models';

@Component({
  selector: 'app-meal-request-list',
  standalone: true,
  imports: [PaginationComponent],
  templateUrl: './meal-request-list.component.html',
  styleUrl: './meal-request-list.component.scss',
})
export class MealRequestListComponent {
  private readonly patientRequestService = inject(PatientRequestService);
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
      this.patientRequestService.listPatientRequests(page, pageSize).pipe(
        catchError((err) => {
          console.error('Failed to load meal requests', err);
          this.listError.set('Failed to load meal requests.');
          return of(null);
        })
      )
    )
  );

  readonly result = toSignal<PagedResult<PatientRequestViewModel> | null>(this.params$, {
    initialValue: null,
  });

  get items(): PatientRequestViewModel[] {
    return this.result()?.items ?? [];
  }
  get totalCount(): number {
    return this.result()?.totalCount ?? 0;
  }

  approvalStatusLabel(status: MealRequestApprovalStatus): string {
    switch (status) {
      case MealRequestApprovalStatus.Accepted:
        return 'Accepted';
      case MealRequestApprovalStatus.Rejected:
        return 'Rejected';
      case MealRequestApprovalStatus.Pending:
      default:
        return 'Pending';
    }
  }

  formatDate(d: Date | string | null): string {
    if (d == null) return '—';
    const date = typeof d === 'string' ? new Date(d) : d;
    return date.toLocaleString();
  }

  onPageChange(p: number): void {
    this.page.set(p);
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }
}

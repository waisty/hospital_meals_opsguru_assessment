import { Component, Injector, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, debounceTime, of, startWith, switchMap, tap } from 'rxjs';
import type { PatientRequestViewModel } from '../../models';
import { PatientRequestService } from '../../services/patient-request.service';
import type { PagedResult } from '../../../shared/models';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatusBadgeComponent, type StatusBadgeVariant } from '../../../shared/components/status-badge/status-badge.component';
import { MealRequestApprovalStatus } from '../../models';
import { isSearchLongEnough } from '../../../shared/constants/search.constants';

@Component({
  selector: 'app-meal-request-list',
  standalone: true,
  imports: [PaginationComponent, StatusBadgeComponent],
  templateUrl: './meal-request-list.component.html',
  styleUrl: './meal-request-list.component.scss',
})
export class MealRequestListComponent {
  private readonly patientRequestService = inject(PatientRequestService);
  private readonly injector = inject(Injector);

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
    computed(() => ({
      page: this.page(),
      pageSize: this.pageSize(),
      search: isSearchLongEnough(this.debouncedSearch()) ? this.debouncedSearch() ?? '' : '',
    })),
    { injector: this.injector }
  ).pipe(
    tap(() => this.listError.set(null)),
    switchMap(({ page, pageSize, search }) =>
      this.patientRequestService.listPatientRequests(page, pageSize, search).pipe(
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

  /** Status badge variant for meal request approval status. */
  statusBadgeVariant(status: MealRequestApprovalStatus): StatusBadgeVariant {
    switch (status) {
      case MealRequestApprovalStatus.Accepted:
        return 'accepted';
      case MealRequestApprovalStatus.Rejected:
        return 'rejected';
      default:
        return 'pending';
    }
  }

  formatDate(d: Date | string | null): string {
    if (d == null) return '-';
    const date = typeof d === 'string' ? new Date(d) : d;
    return date.toLocaleString();
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

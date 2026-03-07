import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { PatientService } from '../../services/patient.service';
import type { DietTypeViewModel } from '../../models';

@Component({
  selector: 'app-diet-types',
  standalone: true,
  imports: [RouterLink, PaginationComponent],
  templateUrl: './diet-types.component.html',
  styleUrl: './diet-types.component.scss',
})
export class DietTypesComponent {
  private readonly patientService = inject(PatientService);
  private readonly router = inject(Router);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly allItems = signal<DietTypeViewModel[]>([]);
  readonly loading = signal(true);
  readonly listError = signal<string | null>(null);

  readonly totalCount = computed(() => this.allItems().length);
  readonly pagedItems = computed(() => {
    const all = this.allItems();
    const p = this.page();
    const size = this.pageSize();
    const start = (p - 1) * size;
    return all.slice(start, start + size);
  });

  constructor() {
    this.patientService.listDietTypes().subscribe({
      next: (list) => {
        this.allItems.set(list);
        this.listError.set(null);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load diet types', err);
        this.listError.set('Failed to load diet types.');
        this.loading.set(false);
      },
    });
  }

  onPageChange(p: number): void {
    this.page.set(p);
  }
  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.page.set(1);
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/patient/setup/diet-types', id]);
  }
}

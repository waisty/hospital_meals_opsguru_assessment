import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AddButtonComponent } from '../../../shared/components/add-button/add-button.component';
import { EditButtonComponent } from '../../../shared/components/edit-button/edit-button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { AuthService } from '../../../auth/services/auth.service';
import { PatientService } from '../../services/patient.service';
import type { ClinicalStateViewModel } from '../../models';

@Component({
  selector: 'app-clinical-states',
  standalone: true,
  imports: [PaginationComponent, AddButtonComponent, EditButtonComponent],
  templateUrl: './clinical-states.component.html',
  styleUrl: './clinical-states.component.scss',
})
export class ClinicalStatesComponent {
  readonly auth = inject(AuthService);
  private readonly patientService = inject(PatientService);
  private readonly router = inject(Router);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly allItems = signal<ClinicalStateViewModel[]>([]);
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
    this.patientService.listClinicalStates().subscribe({
      next: (list) => {
        this.allItems.set(list);
        this.listError.set(null);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load clinical states', err);
        this.listError.set('Failed to load clinical states.');
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
    this.router.navigate(['/patient/setup/clinical-states', id]);
  }
}

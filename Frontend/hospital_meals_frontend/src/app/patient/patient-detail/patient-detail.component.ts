import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { EditButtonComponent } from '../../shared/components/edit-button/edit-button.component';
import { PatientService } from '../services/patient.service';
import type { PatientDetailViewModel, DietTypeViewModel } from '../models';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent],
  templateUrl: './patient-detail.component.html',
  styleUrl: './patient-detail.component.scss',
})
export class PatientDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<PatientDetailViewModel | null>(null);
  readonly dietTypes = signal<DietTypeViewModel[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly dietTypeName = computed(() => {
    const d = this.detail();
    const list = this.dietTypes();
    if (!d || !list.length) return d?.dietTypeId ?? '-';
    const dt = list.find((x) => x.id === d.dietTypeId);
    return dt?.name ?? d.dietTypeId;
  });

  constructor() {
    this.route.paramMap
      .pipe(
        filter((params) => !!params.get('id')),
        switchMap((params) => {
          const id = params.get('id')!;
          this.loading.set(true);
          this.error.set(null);
          return this.patientService.getPatientDetailById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load patient.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();

    this.patientService
      .listDietTypes()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.dietTypes.set(list));
  }
}

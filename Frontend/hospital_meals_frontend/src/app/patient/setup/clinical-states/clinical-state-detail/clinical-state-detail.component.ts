import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { PatientService } from '../../../services/patient.service';
import type { ClinicalStateViewModel } from '../../../models';

@Component({
  selector: 'app-clinical-state-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './clinical-state-detail.component.html',
  styleUrl: './clinical-state-detail.component.scss',
})
export class ClinicalStateDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<ClinicalStateViewModel | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  constructor() {
    this.route.paramMap
      .pipe(
        filter((params) => !!params.get('id')),
        switchMap((params) => {
          const id = params.get('id')!;
          this.loading.set(true);
          this.error.set(null);
          return this.patientService.getClinicalStateById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load clinical state.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();
  }
}

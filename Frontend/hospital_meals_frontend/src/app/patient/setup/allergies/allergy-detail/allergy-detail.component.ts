import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { PatientService } from '../../../services/patient.service';
import type { AllergyViewModel } from '../../../models';

@Component({
  selector: 'app-allergy-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './allergy-detail.component.html',
  styleUrl: './allergy-detail.component.scss',
})
export class AllergyDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<AllergyViewModel | null>(null);
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
          return this.patientService.getAllergyById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load allergy.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();
  }
}

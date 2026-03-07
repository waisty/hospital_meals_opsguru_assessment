import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { filter, switchMap, tap } from 'rxjs';
import { AuthService } from '../../../../auth/services/auth.service';
import { EditButtonComponent } from '../../../../shared/components/edit-button/edit-button.component';
import { PatientService } from '../../../services/patient.service';
import type { DietTypeViewModel } from '../../../models';

@Component({
  selector: 'app-diet-type-detail',
  standalone: true,
  imports: [RouterLink, EditButtonComponent],
  templateUrl: './diet-type-detail.component.html',
  styleUrl: './diet-type-detail.component.scss',
})
export class DietTypeDetailComponent {
  readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<DietTypeViewModel | null>(null);
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
          return this.patientService.getDietTypeById(id).pipe(
            tap({
              next: (detail) => {
                this.detail.set(detail ?? null);
                this.loading.set(false);
              },
              error: () => {
                this.detail.set(null);
                this.loading.set(false);
                this.error.set('Failed to load diet type.');
              },
            })
          );
        })
      )
      .pipe(takeUntilDestroyed())
      .subscribe();
  }
}

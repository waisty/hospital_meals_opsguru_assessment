import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, of, switchMap, tap } from 'rxjs';
import { PatientService } from '../../../services/patient.service';
import type { ClinicalStateViewModel } from '../../../models';

@Component({
  selector: 'app-clinical-state-edit',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './clinical-state-edit.component.html',
  styleUrl: './clinical-state-edit.component.scss',
})
export class ClinicalStateEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<ClinicalStateViewModel | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly formName = signal('');

  readonly isCreateMode = computed(
    () => this.route.snapshot.routeConfig?.path === 'setup/clinical-states/new'
  );

  constructor() {
    this.route.paramMap
      .pipe(
        filter(
          (params) =>
            !!params.get('id') ||
            this.route.snapshot.routeConfig?.path === 'setup/clinical-states/new'
        ),
        switchMap((params) => {
          const id =
            params.get('id') ??
            (this.route.snapshot.routeConfig?.path === 'setup/clinical-states/new' ? 'new' : '');
          this.loading.set(true);
          this.error.set(null);
          if (id === 'new') {
            this.detail.set(null);
            this.formName.set('');
            this.loading.set(false);
            return of(null);
          }
          return this.patientService.getClinicalStateById(id).pipe(
            tap({
              next: (detail) => {
                const d = detail ?? null;
                this.detail.set(d);
                if (d) this.formName.set(d.name);
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

  save(): void {
    if (this.saving()) return;
    const name = this.formName().trim();
    if (!name) {
      this.error.set('Name is required.');
      return;
    }
    this.error.set(null);
    this.saving.set(true);
    if (this.isCreateMode()) {
      this.patientService.createClinicalState({ name }).subscribe({
        next: (res) => {
          this.saving.set(false);
          this.router.navigate(['/patient/setup/clinical-states', res.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to create clinical state.');
        },
      });
    } else {
      const d = this.detail();
      if (!d) return;
      this.patientService.updateClinicalState(d.id, { name }).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/patient/setup/clinical-states', d.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to update clinical state.');
        },
      });
    }
  }

  backToList(): void {
    this.router.navigate(['/patient/setup/clinical-states']);
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, of, switchMap, tap } from 'rxjs';
import { PatientService } from '../../../services/patient.service';
import type { AllergyViewModel } from '../../../models';

@Component({
  selector: 'app-allergy-edit',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './allergy-edit.component.html',
  styleUrl: './allergy-edit.component.scss',
})
export class AllergyEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<AllergyViewModel | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly formName = signal('');

  readonly isCreateMode = computed(
    () => this.route.snapshot.routeConfig?.path === 'allergies/new'
  );

  constructor() {
    this.route.paramMap
      .pipe(
        filter(
          (params) =>
            !!params.get('id') || this.route.snapshot.routeConfig?.path === 'allergies/new'
        ),
        switchMap((params) => {
          const id =
            params.get('id') ??
            (this.route.snapshot.routeConfig?.path === 'allergies/new' ? 'new' : '');
          this.loading.set(true);
          this.error.set(null);
          if (id === 'new') {
            this.detail.set(null);
            this.formName.set('');
            this.loading.set(false);
            return of(null);
          }
          return this.patientService.getAllergyById(id).pipe(
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
                this.error.set('Failed to load allergy.');
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
      this.patientService.createAllergy({ name }).subscribe({
        next: (res) => {
          this.saving.set(false);
          this.router.navigate(['/setup/allergies', res.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to create allergy.');
        },
      });
    } else {
      const d = this.detail();
      if (!d) return;
      this.patientService.updateAllergy(d.id, { name }).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/setup/allergies', d.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to update allergy.');
        },
      });
    }
  }

  backToList(): void {
    this.router.navigate(['/setup/allergies']);
  }
}

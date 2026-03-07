import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, switchMap, tap } from 'rxjs';
import { PatientService } from '../../../services/patient.service';
import type { DietTypeViewModel } from '../../../models';

@Component({
  selector: 'app-diet-type-edit',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './diet-type-edit.component.html',
  styleUrl: './diet-type-edit.component.scss',
})
export class DietTypeEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<DietTypeViewModel | null>(null);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly formName = signal('');

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
                const d = detail ?? null;
                this.detail.set(d);
                if (d) this.formName.set(d.name);
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

  save(): void {
    const d = this.detail();
    if (!d || this.saving()) return;
    const name = this.formName().trim();
    if (!name) {
      this.error.set('Name is required.');
      return;
    }
    this.error.set(null);
    this.saving.set(true);
    this.patientService.updateDietType(d.id, { name }).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/patient/setup/diet-types', d.id]);
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Failed to update diet type.');
      },
    });
  }

  backToList(): void {
    this.router.navigate(['/patient/setup/diet-types']);
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, of, switchMap, tap } from 'rxjs';
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

  readonly isCreateMode = computed(
    () => this.route.snapshot.routeConfig?.path === 'diet-types/new'
  );

  constructor() {
    this.route.paramMap
      .pipe(
        filter(
          (params) =>
            !!params.get('id') ||
            this.route.snapshot.routeConfig?.path === 'diet-types/new'
        ),
        switchMap((params) => {
          const id =
            params.get('id') ??
            (this.route.snapshot.routeConfig?.path === 'diet-types/new' ? 'new' : '');
          this.loading.set(true);
          this.error.set(null);
          if (id === 'new') {
            this.detail.set(null);
            this.formName.set('');
            this.loading.set(false);
            return of(null);
          }
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
    if (this.saving()) return;
    const name = this.formName().trim();
    if (!name) {
      this.error.set('Name is required.');
      return;
    }
    this.error.set(null);
    this.saving.set(true);
    if (this.isCreateMode()) {
      this.patientService.createDietType({ name }).subscribe({
        next: (res) => {
          this.saving.set(false);
          this.router.navigate(['/setup/diet-types', res.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to create diet type.');
        },
      });
    } else {
      const d = this.detail();
      if (!d) return;
      this.patientService.updateDietType(d.id, { name }).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/setup/diet-types', d.id]);
        },
        error: () => {
          this.saving.set(false);
          this.error.set('Failed to update diet type.');
        },
      });
    }
  }

  backToList(): void {
    this.router.navigate(['/setup/diet-types']);
  }
}

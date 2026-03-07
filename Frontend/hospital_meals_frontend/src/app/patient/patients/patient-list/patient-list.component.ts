import { Component, Injector, computed, inject, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, map, of, switchMap } from 'rxjs';
import type { PatientWithDietTypeNameViewModel } from '../../models';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss',
})
export class PatientListComponent {
  private readonly patientService = inject(PatientService);
  private readonly injector = inject(Injector);
  private readonly router = inject(Router);

  readonly items = input.required<PatientWithDietTypeNameViewModel[]>();
  readonly showEditButton = input<boolean>(true);
  readonly clickToNavigateEnabled = input<boolean>(true);

  private readonly items$ = toObservable(computed(() => this.items()), {
    injector: this.injector,
  });
  readonly allergiesByPatientId = toSignal(
    this.items$.pipe(
      switchMap((items) => {
        const ids = items.map((p) => p.id);
        if (ids.length === 0)
          return of({ allergies: {} as Record<string, string[]>, clinicalStates: {} as Record<string, string[]> });
        return forkJoin({
          allergies: this.patientService.getAllergiesByPatientIds(ids).pipe(
            map((res) =>
              (res.items ?? []).reduce<Record<string, string[]>>(
                (acc, it) => {
                  acc[it.patientId] = it.allergyNames ?? [];
                  return acc;
                },
                {}
              )
            )
          ),
          clinicalStates: this.patientService.getClinicalStatesByPatientIds(ids).pipe(
            map((res) =>
              (res.items ?? []).reduce<Record<string, string[]>>(
                (acc, it) => {
                  acc[it.patientId] = it.clinicalStateNames ?? [];
                  return acc;
                },
                {}
              )
            )
          ),
        });
      })
    ),
    { initialValue: { allergies: {} as Record<string, string[]>, clinicalStates: {} as Record<string, string[]> } }
  );

  getAllergyNames(patientId: string): string[] {
    return this.allergiesByPatientId().allergies[patientId] ?? [];
  }

  getClinicalStateNames(patientId: string): string[] {
    return this.allergiesByPatientId().clinicalStates[patientId] ?? [];
  }

  navigateToDetail(id: string): void {
    this.router.navigate(['/patient/patients', id]);
  }

  onRowClick(patientId: string): void {
    if (this.clickToNavigateEnabled()) {
      this.navigateToDetail(patientId);
    }
  }
}

import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, switchMap, tap } from 'rxjs';
import { PatientService } from '../services/patient.service';
import type {
  PatientDetailViewModel,
  PatientAllergyViewModel,
  PatientClinicalStateViewModel,
  AllergyViewModel,
  ClinicalStateViewModel,
  DietTypeViewModel,
} from '../models';

@Component({
  selector: 'app-patient-edit',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './patient-edit.component.html',
  styleUrl: './patient-edit.component.scss',
})
export class PatientEditComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);

  readonly detail = signal<PatientDetailViewModel | null>(null);
  readonly allAllergies = signal<AllergyViewModel[]>([]);
  readonly allClinicalStates = signal<ClinicalStateViewModel[]>([]);
  readonly dietTypes = signal<DietTypeViewModel[]>([]);
  readonly loading = signal(true);
  readonly savingPatient = signal(false);
  readonly savingAllergies = signal(false);
  readonly savingClinicalStates = signal(false);
  readonly error = signal<string | null>(null);

  /** Editable patient fields (synced from detail when loaded). */
  readonly formName = signal('');
  readonly formMobile = signal('');
  readonly formDietTypeId = signal('');
  readonly formNotes = signal<string | null>('');

  readonly patientId = computed(() => this.detail()?.id ?? null);

  /** Allergies not yet assigned to this patient (for add dropdown). */
  readonly availableAllergies = computed(() => {
    const d = this.detail();
    const all = this.allAllergies();
    if (!d) return all;
    const assignedIds = new Set(d.allergies.map((a) => a.allergyId));
    return all.filter((a) => !assignedIds.has(a.id));
  });

  /** Clinical states not yet assigned (for add dropdown). */
  readonly availableClinicalStates = computed(() => {
    const d = this.detail();
    const all = this.allClinicalStates();
    if (!d) return all;
    const assignedIds = new Set(d.clinicalStates.map((c) => c.clinicalStateId));
    return all.filter((c) => !assignedIds.has(c.id));
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
                const d = detail ?? null;
                this.detail.set(d);
                if (d) {
                  this.formName.set(d.name);
                  this.formMobile.set(d.mobileNumber);
                  this.formDietTypeId.set(d.dietTypeId);
                  this.formNotes.set(d.notes ?? '');
                }
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
      .listAllergies()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.allAllergies.set(list));
    this.patientService
      .listClinicalStates()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.allClinicalStates.set(list));
    this.patientService
      .listDietTypes()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.dietTypes.set(list));
  }

  removeAllergy(allergy: PatientAllergyViewModel): void {
    const d = this.detail();
    if (!d || this.savingAllergies()) return;
    const newIds = d.allergies.filter((a) => a.allergyId !== allergy.allergyId).map((a) => a.allergyId);
    this.savingAllergies.set(true);
    this.patientService.updatePatientAllergies(d.id, { allergyIds: newIds }).subscribe({
      next: () => {
        this.detail.update((prev) =>
          prev
            ? {
                ...prev,
                allergies: prev.allergies.filter((a) => a.allergyId !== allergy.allergyId),
              }
            : null
        );
        this.savingAllergies.set(false);
      },
      error: () => {
        this.savingAllergies.set(false);
        this.error.set('Failed to update allergies.');
      },
    });
  }

  addAllergy(allergyId: string): void {
    const d = this.detail();
    if (!d || this.savingAllergies() || !allergyId) return;
    const all = this.allAllergies();
    const allergy = all.find((a) => a.id === allergyId);
    if (!allergy) return;
    const newIds = [...d.allergies.map((a) => a.allergyId), allergyId];
    this.savingAllergies.set(true);
    this.patientService.updatePatientAllergies(d.id, { allergyIds: newIds }).subscribe({
      next: () => {
        this.detail.update((prev) =>
          prev
            ? {
                ...prev,
                allergies: [...prev.allergies, { allergyId: allergy.id, allergyName: allergy.name }],
              }
            : null
        );
        this.savingAllergies.set(false);
      },
      error: () => {
        this.savingAllergies.set(false);
        this.error.set('Failed to update allergies.');
      },
    });
  }

  removeClinicalState(state: PatientClinicalStateViewModel): void {
    const d = this.detail();
    if (!d || this.savingClinicalStates()) return;
    const newIds = d.clinicalStates
      .filter((c) => c.clinicalStateId !== state.clinicalStateId)
      .map((c) => c.clinicalStateId);
    this.savingClinicalStates.set(true);
    this.patientService.updatePatientClinicalStates(d.id, { clinicalStateIds: newIds }).subscribe({
      next: () => {
        this.detail.update((prev) =>
          prev
            ? {
                ...prev,
                clinicalStates: prev.clinicalStates.filter((c) => c.clinicalStateId !== state.clinicalStateId),
              }
            : null
        );
        this.savingClinicalStates.set(false);
      },
      error: () => {
        this.savingClinicalStates.set(false);
        this.error.set('Failed to update clinical states.');
      },
    });
  }

  addClinicalState(clinicalStateId: string): void {
    const d = this.detail();
    if (!d || this.savingClinicalStates() || !clinicalStateId) return;
    const all = this.allClinicalStates();
    const state = all.find((c) => c.id === clinicalStateId);
    if (!state) return;
    const newIds = [...d.clinicalStates.map((c) => c.clinicalStateId), clinicalStateId];
    this.savingClinicalStates.set(true);
    this.patientService.updatePatientClinicalStates(d.id, { clinicalStateIds: newIds }).subscribe({
      next: () => {
        this.detail.update((prev) =>
          prev
            ? {
                ...prev,
                clinicalStates: [...prev.clinicalStates, { clinicalStateId: state.id, clinicalStateName: state.name }],
              }
            : null
        );
        this.savingClinicalStates.set(false);
      },
      error: () => {
        this.savingClinicalStates.set(false);
        this.error.set('Failed to update clinical states.');
      },
    });
  }

  onAllergySelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addAllergy(value);
      select.value = '';
    }
  }

  onClinicalStateSelect(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;
    if (value) {
      this.addClinicalState(value);
      select.value = '';
    }
  }

  savePatient(): void {
    const d = this.detail();
    if (!d || this.savingPatient()) return;
    const name = this.formName().trim();
    const mobile = this.formMobile().trim();
    if (!name || !mobile) {
      this.error.set('Name and mobile are required.');
      return;
    }
    this.error.set(null);
    this.savingPatient.set(true);
    this.patientService
      .updatePatient(d.id, {
        name,
        mobileNumber: mobile,
        dietTypeId: this.formDietTypeId(),
        notes: this.formNotes() || null,
      })
      .subscribe({
        next: () => {
          this.detail.update((prev) =>
            prev
              ? {
                  ...prev,
                  name,
                  mobileNumber: mobile,
                  dietTypeId: this.formDietTypeId(),
                  notes: this.formNotes() || null,
                }
              : null
          );
          this.savingPatient.set(false);
          this.router.navigate(['/patient/patients', d.id]);
        },
        error: () => {
          this.savingPatient.set(false);
          this.error.set('Failed to update patient.');
        },
      });
  }

  backToList(): void {
    this.router.navigate(['/patient/patients']);
  }
}

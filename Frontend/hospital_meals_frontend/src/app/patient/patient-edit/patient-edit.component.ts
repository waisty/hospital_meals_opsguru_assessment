import { Component, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, forkJoin, switchMap, tap } from 'rxjs';
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
  readonly error = signal<string | null>(null);

  /** Editable patient fields (synced from detail when loaded). */
  readonly formFirstName = signal('');
  readonly formMiddleName = signal('');
  readonly formLastName = signal('');
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
                  this.formFirstName.set(d.firstName);
                  this.formMiddleName.set(d.middleName ?? '');
                  this.formLastName.set(d.lastName);
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
    if (!d || this.savingPatient()) return;
    this.detail.update((prev) =>
      prev
        ? { ...prev, allergies: prev.allergies.filter((a) => a.allergyId !== allergy.allergyId) }
        : null
    );
  }

  addAllergy(allergyId: string): void {
    const d = this.detail();
    if (!d || this.savingPatient() || !allergyId) return;
    const all = this.allAllergies();
    const allergy = all.find((a) => a.id === allergyId);
    if (!allergy) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            allergies: [...prev.allergies, { allergyId: allergy.id, allergyName: allergy.name }],
          }
        : null
    );
  }

  removeClinicalState(state: PatientClinicalStateViewModel): void {
    const d = this.detail();
    if (!d || this.savingPatient()) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            clinicalStates: prev.clinicalStates.filter((c) => c.clinicalStateId !== state.clinicalStateId),
          }
        : null
    );
  }

  addClinicalState(clinicalStateId: string): void {
    const d = this.detail();
    if (!d || this.savingPatient() || !clinicalStateId) return;
    const all = this.allClinicalStates();
    const state = all.find((c) => c.id === clinicalStateId);
    if (!state) return;
    this.detail.update((prev) =>
      prev
        ? {
            ...prev,
            clinicalStates: [...prev.clinicalStates, { clinicalStateId: state.id, clinicalStateName: state.name }],
          }
        : null
    );
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
    const firstName = this.formFirstName().trim();
    const middleName = this.formMiddleName().trim();
    const lastName = this.formLastName().trim();
    const mobile = this.formMobile().trim();
    if (!firstName || !mobile) {
      this.error.set('First name and mobile are required.');
      return;
    }
    this.error.set(null);
    this.savingPatient.set(true);
    const allergyIds = d.allergies.map((a) => a.allergyId);
    const clinicalStateIds = d.clinicalStates.map((c) => c.clinicalStateId);
    forkJoin({
      patient: this.patientService.updatePatient(d.id, {
        firstName,
        middleName: middleName,
        lastName: lastName.trim(),
        mobileNumber: mobile,
        dietTypeId: this.formDietTypeId(),
        notes: this.formNotes() || null,
      }),
      allergies: this.patientService.updatePatientAllergies(d.id, { allergyIds }),
      clinicalStates: this.patientService.updatePatientClinicalStates(d.id, { clinicalStateIds }),
    }).subscribe({
      next: () => {
        this.savingPatient.set(false);
        this.router.navigate(['/patient/patients', d.id]);
      },
      error: () => {
        this.savingPatient.set(false);
        this.error.set('Failed to save patient. Please try again.');
      },
    });
  }

  backToList(): void {
    this.router.navigate(['/patient/patients']);
  }
}

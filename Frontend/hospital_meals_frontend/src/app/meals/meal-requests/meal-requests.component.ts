import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MealRequestListComponent } from './meal-request-list/meal-request-list.component';
import { PatientListComponent } from '../../patient/patients/patient-list/patient-list.component';
import { RecipeListComponent } from '../recipes/recipe-list/recipe-list.component';
import { AddButtonComponent } from '../../shared/components/add-button/add-button.component';
import type { PatientWithDietTypeNameViewModel } from '../../patient/models';
import type { RecipeViewModel } from '../models';

@Component({
  selector: 'app-meal-requests',
  standalone: true,
  imports: [MealRequestListComponent, PatientListComponent, RecipeListComponent, AddButtonComponent],
  templateUrl: './meal-requests.component.html',
  styleUrl: './meal-requests.component.scss',
})
export class MealRequestsComponent {
  private readonly router = inject(Router);

  readonly showPatientModal = signal(false);
  readonly selectedPatient = signal<PatientWithDietTypeNameViewModel | null>(null);
  readonly showRecipeModal = signal(false);

  openRequestMeal(): void {
    this.showPatientModal.set(true);
  }

  closePatientModal(): void {
    this.showPatientModal.set(false);
  }

  onPatientSelected(patient: PatientWithDietTypeNameViewModel): void {
    this.selectedPatient.set(patient);
    this.closePatientModal();
    this.showRecipeModal.set(true);
  }

  closeRecipeModal(): void {
    this.showRecipeModal.set(false);
  }

  onRecipeSelected(recipe: RecipeViewModel): void {
    const patient = this.selectedPatient();
    if (!patient) return;
    this.closeRecipeModal();
    this.router.navigate(['/meals/meal-requests/summary'], {
      queryParams: { patientId: patient.id, recipeId: recipe.id },
      state: { fromRequestFlow: true },
    });
  }
}

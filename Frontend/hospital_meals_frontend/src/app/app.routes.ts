import { Routes } from '@angular/router';
import { IndexRedirectComponent } from './index-redirect/index-redirect.component';
import { LoginComponent } from './auth/login/login.component';
import { HomeComponent } from './home/home.component';
import { PatientDashboardComponent } from './patient/patient-dashboard/patient-dashboard.component';
import { PatientsComponent } from './patient/patients/patients.component';
import { PatientDetailComponent } from './patient/patient-detail/patient-detail.component';
import { PatientEditComponent } from './patient/patient-edit/patient-edit.component';
import { AllergiesComponent } from './patient/setup/allergies/allergies.component';
import { AllergyDetailComponent } from './patient/setup/allergies/allergy-detail/allergy-detail.component';
import { AllergyEditComponent } from './patient/setup/allergies/allergy-edit/allergy-edit.component';
import { ClinicalStatesComponent } from './patient/setup/clinical-states/clinical-states.component';
import { ClinicalStateDetailComponent } from './patient/setup/clinical-states/clinical-state-detail/clinical-state-detail.component';
import { ClinicalStateEditComponent } from './patient/setup/clinical-states/clinical-state-edit/clinical-state-edit.component';
import { DietTypesComponent } from './patient/setup/diet-types/diet-types.component';
import { DietTypeDetailComponent } from './patient/setup/diet-types/diet-type-detail/diet-type-detail.component';
import { DietTypeEditComponent } from './patient/setup/diet-types/diet-type-edit/diet-type-edit.component';
import { SetupDashboardComponent } from './setup/setup-dashboard/setup-dashboard.component';
import { MealsDashboardComponent } from './meals/meals-dashboard/meals-dashboard.component';
import { MealRequestsComponent } from './meals/meal-requests/meal-requests.component';
import { SetupPlaceholderComponent } from './meals/setup/setup-placeholder.component';
import { KitchenDashboardComponent } from './kitchen/kitchen-dashboard/kitchen-dashboard.component';
import {
  initialRedirectGuard,
  redirectIfLoggedInGuard,
  authGuard,
  adminGuard,
  mealsGuard,
  homeRedirectGuard,
} from './auth/guards';

export const routes: Routes = [
  {
    path: '',
    component: IndexRedirectComponent,
    canActivate: [initialRedirectGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [redirectIfLoggedInGuard],
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [authGuard, homeRedirectGuard],
  },
  {
    path: 'patient',
    component: PatientDashboardComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'patients', pathMatch: 'full' },
      { path: 'patients', component: PatientsComponent },
      { path: 'patients/:id', component: PatientDetailComponent },
      { path: 'patients/:id/edit', component: PatientEditComponent },
    ],
  },
  {
    path: 'setup',
    component: SetupDashboardComponent,
    canActivate: [authGuard, adminGuard],
    children: [
      { path: '', redirectTo: 'allergies', pathMatch: 'full' },
      { path: 'allergies', component: AllergiesComponent },
      { path: 'allergies/new', component: AllergyEditComponent },
      { path: 'allergies/:id', component: AllergyDetailComponent },
      { path: 'allergies/:id/edit', component: AllergyEditComponent },
      { path: 'clinical-states', component: ClinicalStatesComponent },
      { path: 'clinical-states/new', component: ClinicalStateEditComponent },
      { path: 'clinical-states/:id', component: ClinicalStateDetailComponent },
      { path: 'clinical-states/:id/edit', component: ClinicalStateEditComponent },
      { path: 'diet-types', component: DietTypesComponent },
      { path: 'diet-types/new', component: DietTypeEditComponent },
      { path: 'diet-types/:id', component: DietTypeDetailComponent },
      { path: 'diet-types/:id/edit', component: DietTypeEditComponent },
    ],
  },
  {
    path: 'meals',
    component: MealsDashboardComponent,
    canActivate: [authGuard, mealsGuard],
    children: [
      { path: '', redirectTo: 'meal-requests', pathMatch: 'full' },
      { path: 'meal-requests', component: MealRequestsComponent },
      { path: 'setup/ingredients/allergy-exclusions', component: SetupPlaceholderComponent, data: { title: 'Allergy Exclusions' } },
      { path: 'setup/ingredients/clinical-state-exclusions', component: SetupPlaceholderComponent, data: { title: 'Clinical State Exclusions' } },
      { path: 'setup/ingredients/diet-type-exclusions', component: SetupPlaceholderComponent, data: { title: 'Diet Type Exclusions' } },
      { path: 'setup/recipes', component: SetupPlaceholderComponent, data: { title: 'Recipes' } },
      { path: 'setup/meals', component: SetupPlaceholderComponent, data: { title: 'Meals' } },
    ],
  },
  {
    path: 'kitchen',
    component: KitchenDashboardComponent,
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '' },
];

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
import { MealsDashboardComponent } from './meals/meals-dashboard/meals-dashboard.component';
import { KitchenDashboardComponent } from './kitchen/kitchen-dashboard/kitchen-dashboard.component';
import {
  initialRedirectGuard,
  redirectIfLoggedInGuard,
  authGuard,
  adminGuard,
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
      { path: 'setup/allergies', component: AllergiesComponent },
      { path: 'setup/allergies/new', component: AllergyEditComponent, canActivate: [adminGuard] },
      { path: 'setup/allergies/:id', component: AllergyDetailComponent },
      { path: 'setup/allergies/:id/edit', component: AllergyEditComponent, canActivate: [adminGuard] },
      { path: 'setup/clinical-states', component: ClinicalStatesComponent },
      { path: 'setup/clinical-states/new', component: ClinicalStateEditComponent, canActivate: [adminGuard] },
      { path: 'setup/clinical-states/:id', component: ClinicalStateDetailComponent },
      { path: 'setup/clinical-states/:id/edit', component: ClinicalStateEditComponent, canActivate: [adminGuard] },
      { path: 'setup/diet-types', component: DietTypesComponent },
      { path: 'setup/diet-types/new', component: DietTypeEditComponent, canActivate: [adminGuard] },
      { path: 'setup/diet-types/:id', component: DietTypeDetailComponent },
      { path: 'setup/diet-types/:id/edit', component: DietTypeEditComponent, canActivate: [adminGuard] },
    ],
  },
  {
    path: 'meals',
    component: MealsDashboardComponent,
    canActivate: [authGuard],
  },
  {
    path: 'kitchen',
    component: KitchenDashboardComponent,
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '' },
];

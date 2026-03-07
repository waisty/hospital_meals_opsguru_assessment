import { Routes } from '@angular/router';
import { IndexRedirectComponent } from './index-redirect/index-redirect.component';
import { LoginComponent } from './auth/login/login.component';
import { HomeComponent } from './home/home.component';
import { PatientDashboardComponent } from './patient/patient-dashboard/patient-dashboard.component';
import { PatientsComponent } from './patient/patients/patients.component';
import { AllergiesComponent } from './patient/setup/allergies/allergies.component';
import { ClinicalStatesComponent } from './patient/setup/clinical-states/clinical-states.component';
import { DietTypesComponent } from './patient/setup/diet-types/diet-types.component';
import { MealsDashboardComponent } from './meals/meals-dashboard/meals-dashboard.component';
import { KitchenDashboardComponent } from './kitchen/kitchen-dashboard/kitchen-dashboard.component';
import {
  initialRedirectGuard,
  redirectIfLoggedInGuard,
  authGuard,
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
      { path: 'setup/allergies', component: AllergiesComponent },
      { path: 'setup/clinical-states', component: ClinicalStatesComponent },
      { path: 'setup/diet-types', component: DietTypesComponent },
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

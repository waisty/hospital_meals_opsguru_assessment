import { Routes } from '@angular/router';
import { IndexRedirectComponent } from './index-redirect/index-redirect.component';
import { LoginComponent } from './auth/login/login.component';
import { HomeComponent } from './home/home.component';
import { PatientDashboardComponent } from './patient/patient-dashboard/patient-dashboard.component';
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

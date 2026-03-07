import { Component, inject, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth/services/auth.service';

export interface DashboardLink {
  path: string;
  label: string;
  area: 'patient' | 'meals' | 'kitchen';
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  protected readonly auth = inject(AuthService);

  protected readonly dashboardLinks = computed<DashboardLink[]>(() => {
    const links: DashboardLink[] = [];
    if (this.auth.canAccessPatient()) links.push({ path: '/patient', label: 'Patient', area: 'patient' });
    if (this.auth.canAccessMeals()) links.push({ path: '/meals', label: 'Meals', area: 'meals' });
    if (this.auth.canAccessKitchen()) links.push({ path: '/kitchen', label: 'Kitchen', area: 'kitchen' });
    return links;
  });
}

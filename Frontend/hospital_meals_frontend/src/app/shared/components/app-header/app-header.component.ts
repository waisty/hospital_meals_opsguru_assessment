import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';

interface NavItem {
  path: string;
  label: string;
  visible: boolean;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
})
export class AppHeaderComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly navItems = computed<NavItem[]>(() => [
    { path: '/home', label: 'Home', visible: true },
    { path: '/patient', label: 'Patient', visible: this.auth.canAccessPatient() },
    { path: '/meals', label: 'Meals', visible: this.auth.canAccessMeals() },
    { path: '/kitchen', label: 'Kitchen', visible: this.auth.canAccessKitchen() },
  ]);

  protected logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppSidebarComponent, type SidebarNavItem } from '../../shared/components/app-sidebar/app-sidebar.component';

@Component({
  selector: 'app-setup-dashboard',
  standalone: true,
  imports: [AppSidebarComponent, RouterOutlet],
  templateUrl: './setup-dashboard.component.html',
  styleUrl: './setup-dashboard.component.scss',
})
export class SetupDashboardComponent {
  protected readonly sidebarItems: SidebarNavItem[] = [
    { path: '/setup/allergies', label: 'Allergies' },
    { path: '/setup/clinical-states', label: 'Clinical States' },
    { path: '/setup/diet-types', label: 'Diet Types' },
  ];
}

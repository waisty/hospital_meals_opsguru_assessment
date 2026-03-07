import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppSidebarComponent, type SidebarNavItem } from '../../shared/components/app-sidebar/app-sidebar.component';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [AppSidebarComponent, RouterOutlet],
  templateUrl: './patient-dashboard.component.html',
  styleUrl: './patient-dashboard.component.scss',
})
export class PatientDashboardComponent {
  protected readonly sidebarItems: SidebarNavItem[] = [
    { path: '/patient/patients', label: 'Patients', exact: true },
  ];
}

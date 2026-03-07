import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-setup-dashboard',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './setup-dashboard.component.html',
  styleUrl: './setup-dashboard.component.scss',
})
export class SetupDashboardComponent {}

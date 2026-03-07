import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-kitchen-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './kitchen-dashboard.component.html',
  styleUrl: './kitchen-dashboard.component.scss',
})
export class KitchenDashboardComponent {}

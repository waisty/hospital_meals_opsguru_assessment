import { Component } from '@angular/core';
import { AddButtonComponent } from '../../shared/components/add-button/add-button.component';
import { PatientListComponent } from './patient-list/patient-list.component';

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [PatientListComponent, AddButtonComponent],
  templateUrl: './patients.component.html',
  styleUrl: './patients.component.scss',
})
export class PatientsComponent {}

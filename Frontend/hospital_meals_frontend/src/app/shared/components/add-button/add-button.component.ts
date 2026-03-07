import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-add-button',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './add-button.component.html',
  styleUrl: './add-button.component.scss',
})
export class AddButtonComponent {
  /** Route link (path string or array of segments). */
  readonly link = input.required<string | string[]>();

  /** Button label (e.g. "Add patient", "Add allergy"). */
  readonly label = input.required<string>();
}

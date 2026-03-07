import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-add-button',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './add-button.component.html',
  styleUrl: './add-button.component.scss',
})
export class AddButtonComponent {
  /** Route link (path string or array of segments). When not set, button emits addClick on click. */
  readonly link = input<string | string[]>();

  /** Button label (e.g. "Add patient", "Request meal"). */
  readonly label = input.required<string>();

  /** Emitted when the button is clicked and no link is set. */
  readonly addClick = output<void>();
}

import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-edit-button',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './edit-button.component.html',
  styleUrl: './edit-button.component.scss',
})
export class EditButtonComponent {
  /** Route link (path string or array of segments). */
  readonly link = input.required<string | string[]>();

  /** Optional tooltip / title. */
  readonly title = input<string>('');

  /** When true, click event does not propagate (e.g. for use inside clickable rows). */
  readonly stopPropagation = input<boolean>(false);

  onClick(event: Event): void {
    if (this.stopPropagation()) {
      event.stopPropagation();
    }
  }
}

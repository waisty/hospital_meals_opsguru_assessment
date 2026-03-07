import { Component, input } from '@angular/core';

export type StatusBadgeVariant =
  | 'accepted'
  | 'rejected'
  | 'pending'
  | 'allergy'
  | 'clinical-state'
  | 'diet';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss',
})
export class StatusBadgeComponent {
  /** Text to show in the badge. */
  readonly label = input.required<string>();

  /** Variant: accepted/rejected/pending (status) or allergy/clinical-state/diet (exclusions, clinical states, allergies). */
  readonly variant = input.required<StatusBadgeVariant>();
}

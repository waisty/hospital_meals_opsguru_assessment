import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-setup-placeholder',
  standalone: true,
  template: `<div class="setup-placeholder"><h2>{{ title }}</h2><p>Setup page - coming soon.</p></div>`,
  styles: [
    `
      .setup-placeholder {
        color: #64748b;
      }
      .setup-placeholder h2 {
        margin: 0 0 0.5rem;
        color: #0f172a;
      }
      .setup-placeholder p {
        margin: 0;
      }
    `,
  ],
})
export class SetupPlaceholderComponent {
  private readonly route = inject(ActivatedRoute);
  get title(): string {
    return (this.route.snapshot.data['title'] as string) ?? 'Setup';
  }
}

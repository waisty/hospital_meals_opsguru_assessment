import { Component } from '@angular/core';

/**
 * Placeholder for the empty route. initialRedirectGuard always redirects to /login or /home,
 * so this component is never rendered.
 */
@Component({
  selector: 'app-index-redirect',
  standalone: true,
  template: '',
})
export class IndexRedirectComponent {}

import { NgClass } from '@angular/common';
import { Component, inject } from '@angular/core';

import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-connection-bar',
  imports: [NgClass],
  templateUrl: './connection-bar.component.html',
})
export class ConnectionBarComponent {
  readonly store = inject(SocialAppStore);

  updateBaseUrl(event: Event): void {
    this.store.setBaseUrl((event.target as HTMLInputElement).value);
  }

  reconnect(): void {
    void this.store.bootstrap();
  }

  statusClass(): string {
    const status = this.store.status();

    if (status === 'online') {
      return 'border-emerald-200 bg-emerald-50 text-emerald-700';
    }

    if (status === 'checking') {
      return 'border-sky-200 bg-sky-50 text-sky-700';
    }

    return 'border-amber-200 bg-amber-50 text-amber-700';
  }
}

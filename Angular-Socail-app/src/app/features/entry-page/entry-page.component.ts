import { NgClass } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { SocialAppStore } from '../../core/store/social-app.store';
import { AuthPanelComponent } from '../auth-panel/auth-panel.component';

@Component({
  selector: 'app-entry-page',
  imports: [AuthPanelComponent, NgClass],
  templateUrl: './entry-page.component.html',
})
export class EntryPageComponent implements OnInit {
  readonly store = inject(SocialAppStore);
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (this.store.isAuthenticated()) {
      void this.openApp();
      return;
    }

    void this.store.bootstrap();
  }

  updateBaseUrl(event: Event): void {
    this.store.setBaseUrl((event.target as HTMLInputElement).value);
  }

  reconnect(): void {
    void this.store.bootstrap();
  }

  async openApp(): Promise<void> {
    await this.router.navigateByUrl('/app');
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

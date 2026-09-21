import { NgClass } from '@angular/common';
import { Component, inject } from '@angular/core';

import { SocialAppStore } from '../../core/store/social-app.store';

@Component({
  selector: 'app-system-activity-panel',
  imports: [NgClass],
  templateUrl: './system-activity-panel.component.html',
})
export class SystemActivityPanelComponent {
  readonly store = inject(SocialAppStore);

  clear(): void {
    this.store.clearActivity();
  }

  levelClass(level: string): string {
    if (level === 'success') {
      return 'border-emerald-200 bg-emerald-50 text-emerald-700';
    }

    if (level === 'warning') {
      return 'border-amber-200 bg-amber-50 text-amber-700';
    }

    if (level === 'error') {
      return 'border-rose-200 bg-rose-50 text-rose-700';
    }

    return 'border-sky-200 bg-sky-50 text-sky-700';
  }
}

import { NgClass } from '@angular/common';
import { Component, inject } from '@angular/core';

import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-notifications-panel',
  imports: [NgClass],
  templateUrl: './notifications-panel.component.html',
})
export class NotificationsPanelComponent {
  readonly store = inject(SocialAppStore);

  refresh(): void {
    void this.store.loadNotifications();
  }

  markRead(): void {
    this.store.markNotificationsRead();
  }
}

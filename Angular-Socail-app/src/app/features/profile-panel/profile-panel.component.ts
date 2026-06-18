import { Component, inject } from '@angular/core';

import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-profile-panel',
  templateUrl: './profile-panel.component.html',
})
export class ProfilePanelComponent {
  readonly store = inject(SocialAppStore);

  refreshProfile(): void {
    void this.store.loadCurrentUser();
  }

  logout(): void {
    void this.store.logout();
  }
}

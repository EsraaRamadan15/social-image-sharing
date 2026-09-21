import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { SocialAppStore } from '../../core/store/social-app.store';

@Component({
  selector: 'app-profile-panel',
  templateUrl: './profile-panel.component.html',
})
export class ProfilePanelComponent {
  readonly store = inject(SocialAppStore);
  private readonly router = inject(Router);

  refreshProfile(): void {
    void this.store.loadCurrentUser();
  }

  async logout(): Promise<void> {
    await this.store.logout();
    await this.router.navigateByUrl('/auth');
  }
}

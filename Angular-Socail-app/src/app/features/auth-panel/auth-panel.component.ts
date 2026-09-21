import { NgClass } from '@angular/common';
import { Component, inject, output } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SocialAppStore } from '../../core/store/social-app.store';

@Component({
  selector: 'app-auth-panel',
  imports: [FormsModule, NgClass],
  templateUrl: './auth-panel.component.html',
})
export class AuthPanelComponent {
  readonly store = inject(SocialAppStore);
  readonly authenticated = output<void>();

  mode: 'login' | 'register' = 'login';
  displayName = '';
  userName = '';
  email = '';
  password = '';

  setMode(mode: 'login' | 'register'): void {
    this.mode = mode;
  }

  async submit(): Promise<void> {
    let completed = false;

    if (this.mode === 'login') {
      completed = await this.store.login(this.email, this.password);
    } else {
      completed = await this.store.register(this.displayName, this.userName, this.email, this.password);
    }

    if (completed && this.store.isAuthenticated()) {
      this.authenticated.emit();
    }
  }
}

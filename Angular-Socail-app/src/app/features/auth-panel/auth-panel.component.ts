import { NgClass } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-auth-panel',
  imports: [FormsModule, NgClass],
  templateUrl: './auth-panel.component.html',
})
export class AuthPanelComponent {
  readonly store = inject(SocialAppStore);

  mode: 'login' | 'register' = 'login';
  displayName = 'Mona Kamel';
  userName = 'mona';
  email = 'mona@example.com';
  password = 'Passw0rd!';

  setMode(mode: 'login' | 'register'): void {
    this.mode = mode;
  }

  submit(): void {
    if (this.mode === 'login') {
      void this.store.login(this.email, this.password);
      return;
    }

    void this.store.register(this.displayName, this.userName, this.email, this.password);
  }
}

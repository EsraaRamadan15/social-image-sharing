import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SuggestedUser } from '../../core/social-api.models';
import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-people-panel',
  imports: [FormsModule],
  templateUrl: './people-panel.component.html',
})
export class PeoplePanelComponent {
  readonly store = inject(SocialAppStore);
  query = '';

  search(): void {
    void this.store.searchPeople(this.query);
  }

  follow(user: SuggestedUser): void {
    void this.store.followUser(user);
  }
}

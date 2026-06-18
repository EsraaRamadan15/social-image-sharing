import { Component, inject } from '@angular/core';

import { SocialAppStore } from '../../core/social-app.store';
import { PostCardComponent } from '../post-card/post-card.component';

@Component({
  selector: 'app-feed',
  imports: [PostCardComponent],
  templateUrl: './feed.component.html',
})
export class FeedComponent {
  readonly store = inject(SocialAppStore);

  refreshFeed(): void {
    void this.store.loadFeed();
  }
}

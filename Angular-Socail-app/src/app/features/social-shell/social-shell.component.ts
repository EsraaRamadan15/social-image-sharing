import { Component, inject, OnInit } from '@angular/core';

import { SocialAppStore } from '../../core/social-app.store';
import { AuthPanelComponent } from '../auth-panel/auth-panel.component';
import { ConnectionBarComponent } from '../connection-bar/connection-bar.component';
import { FeedComponent } from '../feed/feed.component';
import { NotificationsPanelComponent } from '../notifications-panel/notifications-panel.component';
import { PeoplePanelComponent } from '../people-panel/people-panel.component';
import { PostComposerComponent } from '../post-composer/post-composer.component';
import { ProfilePanelComponent } from '../profile-panel/profile-panel.component';

@Component({
  selector: 'app-social-shell',
  imports: [
    AuthPanelComponent,
    ConnectionBarComponent,
    FeedComponent,
    NotificationsPanelComponent,
    PeoplePanelComponent,
    PostComposerComponent,
    ProfilePanelComponent,
  ],
  templateUrl: './social-shell.component.html',
})
export class SocialShellComponent implements OnInit {
  private readonly store = inject(SocialAppStore);

  ngOnInit(): void {
    void this.store.bootstrap();
  }
}

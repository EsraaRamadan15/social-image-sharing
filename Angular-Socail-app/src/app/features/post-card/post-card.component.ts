import { NgClass } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { DemoPost } from '../../core/social-api.models';
import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-post-card',
  imports: [FormsModule, NgClass],
  templateUrl: './post-card.component.html',
})
export class PostCardComponent {
  readonly post = input.required<DemoPost>();
  readonly store = inject(SocialAppStore);

  comment = '';

  toggleLike(): void {
    void this.store.toggleLike(this.post());
  }

  submitComment(): void {
    void this.store.addComment(this.post(), this.comment);
    this.comment = '';
  }

  deletePost(): void {
    void this.store.deletePost(this.post());
  }
}

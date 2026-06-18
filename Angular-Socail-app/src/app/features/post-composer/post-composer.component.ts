import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SocialAppStore } from '../../core/social-app.store';

@Component({
  selector: 'app-post-composer',
  imports: [FormsModule],
  templateUrl: './post-composer.component.html',
})
export class PostComposerComponent {
  readonly store = inject(SocialAppStore);

  caption = 'Evening colors from the bridge.';
  selectedFile?: File;
  previewUrl = 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80';

  selectFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    this.selectedFile = file;
    this.previewUrl = URL.createObjectURL(file);
  }

  publish(): void {
    void this.store.createPost(this.caption, this.selectedFile);
    this.caption = '';
    this.selectedFile = undefined;
  }
}

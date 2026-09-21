import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SocialAppStore } from '../../core/store/social-app.store';

@Component({
  selector: 'app-post-composer',
  imports: [FormsModule],
  templateUrl: './post-composer.component.html',
})
export class PostComposerComponent {
  readonly store = inject(SocialAppStore);

  caption = '';
  selectedFile?: File;
  previewUrl = '';

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
    if (!this.selectedFile) {
      void this.store.createPost(this.caption);
      return;
    }

    void this.store.createPost(this.caption, this.selectedFile);
    this.caption = '';
    this.selectedFile = undefined;
    this.previewUrl = '';
  }
}

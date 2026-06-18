import { Component } from '@angular/core';

import { SocialShellComponent } from './features/social-shell/social-shell.component';

@Component({
  selector: 'app-root',
  imports: [SocialShellComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}

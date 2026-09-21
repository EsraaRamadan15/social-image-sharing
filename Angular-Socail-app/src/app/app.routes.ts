import { inject } from '@angular/core';
import { CanActivateFn, Router, Routes } from '@angular/router';

import { SocialAppStore } from './core/store/social-app.store';
import { EntryPageComponent } from './features/entry-page/entry-page.component';
import { SocialShellComponent } from './features/social-shell/social-shell.component';

const requireAuth: CanActivateFn = () => {
  const store = inject(SocialAppStore);
  const router = inject(Router);

  return store.isAuthenticated() ? true : router.createUrlTree(['/auth']);
};

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'auth',
  },
  {
    path: 'auth',
    component: EntryPageComponent,
  },
  {
    path: 'app',
    component: SocialShellComponent,
    canActivate: [requireAuth],
  },
  {
    path: '**',
    redirectTo: 'auth',
  },
];

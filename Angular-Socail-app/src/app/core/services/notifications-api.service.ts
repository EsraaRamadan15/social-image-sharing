import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiCall, DemoNotification } from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class NotificationsApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  getNotifications(): ApiCall<{ items: DemoNotification[]; unread: number }> {
    return this.session.withAutoRefresh(() =>
      this.http.get<{ items: DemoNotification[]; unread: number }>(
        this.session.endpointPath('/notifications'),
        this.session.authOptions(),
      ),
    );
  }
}

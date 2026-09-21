import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiCall, DemoUser, EntityId, ProfileUpdateRequest } from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class UsersApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  getCurrentProfile(): ApiCall<DemoUser> {
    return this.session.withAutoRefresh(() =>
      this.http.get<DemoUser>(this.session.endpointPath('/users/me'), this.session.authOptions()),
    );
  }

  updateProfile(payload: ProfileUpdateRequest): ApiCall<{ message: string; updatedAt: string }> {
    return this.session.withAutoRefresh(() =>
      this.http.put<{ message: string; updatedAt: string }>(
        this.session.endpointPath('/users/me'),
        payload,
        this.session.authOptions(),
      ),
    );
  }

  searchUsers(query: string): ApiCall<{ items: DemoUser[]; total: number }> {
    return this.session.withAutoRefresh(() =>
      this.http.get<{ items: DemoUser[]; total: number }>(
        this.session.endpointPath(`/users?query=${encodeURIComponent(query)}`),
        this.session.authOptions(),
      ),
    );
  }

  setFollowing(userId: EntityId, following: boolean): ApiCall<{ targetUserId: EntityId; following: boolean; followers: number }> {
    return this.session.withAutoRefresh(() =>
      this.http.post<{ targetUserId: EntityId; following: boolean; followers: number }>(
        this.session.endpointPath(`/users/${userId}/follow`),
        { following },
        this.session.authOptions(),
      ),
    );
  }
}

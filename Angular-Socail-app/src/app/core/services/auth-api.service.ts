import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { of } from 'rxjs';

import { ApiCall, AuthResponse, LoginRequest, RegisterRequest } from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  register(payload: RegisterRequest): ApiCall<AuthResponse> {
    return this.http.post<AuthResponse>(this.session.endpointPath('/auth/register'), payload);
  }

  login(payload: LoginRequest): ApiCall<AuthResponse> {
    return this.http.post<AuthResponse>(this.session.endpointPath('/auth/login'), payload);
  }

  revokeRefreshToken(): ApiCall<void> {
    const refreshToken = this.session.currentRefreshToken();

    if (!refreshToken) {
      return of(undefined);
    }

    return this.http.post<void>(
      this.session.endpointPath('/auth/logout'),
      {
        refreshToken,
      },
      this.session.authOptions(),
    );
  }
}

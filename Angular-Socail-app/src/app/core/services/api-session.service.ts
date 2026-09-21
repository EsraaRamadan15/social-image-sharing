import { HttpHeaders, HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, Observable, shareReplay, switchMap, throwError } from 'rxjs';

import { AuthResponse, RefreshSessionResponse } from '../models/social-api.models';

@Injectable({ providedIn: 'root' })
export class ApiSessionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = signal('https://localhost:7051/api');
  private readonly token = signal<string | null>(null);
  private readonly refreshToken = signal<string | null>(null);
  private refreshRequest?: Observable<string>;

  readonly apiBaseUrl = this.baseUrl.asReadonly();
  readonly accessToken = this.token.asReadonly();
  readonly storedRefreshToken = this.refreshToken.asReadonly();

  setBaseUrl(value: string): void {
    const next = value.trim();
    if (next) {
      this.baseUrl.set(next.replace(/\/$/, ''));
    }
  }

  endpointPath(path: string): string {
    return `${this.baseUrl()}${path}`;
  }

  publicAssetUrl(publicUrl?: string | null): string | undefined {
    if (!publicUrl) {
      return undefined;
    }

    if (/^https?:\/\//i.test(publicUrl)) {
      return publicUrl;
    }

    const apiRoot = this.baseUrl().replace(/\/api$/, '');
    const normalizedPath = publicUrl.startsWith('/') ? publicUrl : `/${publicUrl}`;

    return `${apiRoot}${normalizedPath}`;
  }

  authOptions(): { headers?: HttpHeaders } {
    const token = this.token();

    return token
      ? {
          headers: new HttpHeaders({
            Authorization: `Bearer ${token}`,
          }),
        }
      : {};
  }

  currentRefreshToken(): string | null {
    return this.refreshToken();
  }

  setToken(token: string | null): void {
    this.token.set(token);
  }

  setSession(auth: AuthResponse): void {
    this.token.set(auth.accessToken);

    if (auth.refreshToken) {
      this.refreshToken.set(auth.refreshToken);
    }
  }

  clearSession(): void {
    this.token.set(null);
    this.refreshToken.set(null);
    this.refreshRequest = undefined;
  }

  withAutoRefresh<T>(requestFactory: () => Observable<T>): Observable<T> {
    return requestFactory().pipe(
      catchError((error: { status?: number }) => {
        if (error.status !== 401 || !this.refreshToken()) {
          return throwError(() => error);
        }

        return this.refreshAccessToken().pipe(switchMap(() => requestFactory()));
      }),
    );
  }

  private refreshAccessToken(): Observable<string> {
    const refreshToken = this.refreshToken();

    if (!refreshToken) {
      return throwError(() => new Error('No refresh token is available.'));
    }

    if (!this.refreshRequest) {
      this.refreshRequest = this.http
        .post<RefreshSessionResponse>(this.endpointPath('/auth/refresh'), {
          refreshToken,
        })
        .pipe(
          map((response) => {
            this.token.set(response.accessToken);

            if (response.refreshToken) {
              this.refreshToken.set(response.refreshToken);
            }

            return response.accessToken;
          }),
          catchError((error) => {
            this.clearSession();
            return throwError(() => error);
          }),
          finalize(() => {
            this.refreshRequest = undefined;
          }),
          shareReplay(1),
        );
    }

    return this.refreshRequest;
  }
}

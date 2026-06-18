import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, Observable, of, shareReplay, switchMap, throwError } from 'rxjs';

import {
  ApiCall,
  AuthResponse,
  CommentCreateRequest,
  DemoNotification,
  DemoPost,
  DemoUser,
  LoginRequest,
  PostCreateRequest,
  ProfileUpdateRequest,
  RefreshSessionResponse,
  RegisterRequest,
} from './social-api.models';

@Injectable({ providedIn: 'root' })
export class SocialApiClient {
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

  private authOptions(): { headers?: HttpHeaders } {
    const token = this.token();

    return token
      ? {
          headers: new HttpHeaders({
            Authorization: `Bearer ${token}`,
          }),
        }
      : {};
  }

  register(payload: RegisterRequest): ApiCall<AuthResponse> {
    return this.http.post<AuthResponse>(this.endpointPath('/auth/register'), payload);
  }

  login(payload: LoginRequest): ApiCall<AuthResponse> {
    return this.http.post<AuthResponse>(this.endpointPath('/auth/login'), payload);
  }

  refreshSession(refreshToken: string): ApiCall<RefreshSessionResponse> {
    return this.http.post<RefreshSessionResponse>(this.endpointPath('/auth/refresh'), {
      refreshToken,
    });
  }

  revokeRefreshToken(): ApiCall<void> {
    const refreshToken = this.refreshToken();

    if (!refreshToken) {
      return of(undefined);
    }

    return this.http.post<void>(
      this.endpointPath('/auth/logout'),
      {
        refreshToken,
      },
      this.authOptions(),
    );
  }

  getCurrentProfile(): ApiCall<DemoUser> {
    return this.withAutoRefresh(() => this.http.get<DemoUser>(this.endpointPath('/users/me'), this.authOptions()));
  }

  updateProfile(payload: ProfileUpdateRequest): ApiCall<{ message: string; updatedAt: string }> {
    return this.withAutoRefresh(() =>
      this.http.put<{ message: string; updatedAt: string }>(this.endpointPath('/users/me'), payload, this.authOptions()),
    );
  }

  searchUsers(query: string): ApiCall<{ items: DemoUser[]; total: number }> {
    return this.withAutoRefresh(() =>
      this.http.get<{ items: DemoUser[]; total: number }>(
        this.endpointPath(`/users?query=${encodeURIComponent(query)}`),
        this.authOptions(),
      ),
    );
  }

  uploadImage(payload: FormData): ApiCall<{ imageId: number; url: string; width: number; height: number }> {
    return this.withAutoRefresh(() =>
      this.http.post<{ imageId: number; url: string; width: number; height: number }>(
        this.endpointPath('/images'),
        payload,
        this.authOptions(),
      ),
    );
  }

  createPost(payload: PostCreateRequest): ApiCall<DemoPost> {
    return this.withAutoRefresh(() => this.http.post<DemoPost>(this.endpointPath('/posts'), payload, this.authOptions()));
  }

  getFeed(page = 1): ApiCall<{ items: DemoPost[]; nextCursor?: string }> {
    return this.withAutoRefresh(() =>
      this.http.get<{ items: DemoPost[]; nextCursor?: string }>(
        this.endpointPath(`/posts/feed?page=${page}`),
        this.authOptions(),
      ),
    );
  }

  getPost(postId: number): ApiCall<DemoPost> {
    return this.withAutoRefresh(() => this.http.get<DemoPost>(this.endpointPath(`/posts/${postId}`), this.authOptions()));
  }

  setPostLike(postId: number, liked: boolean): ApiCall<{ postId: number; liked: boolean; likes: number }> {
    return this.withAutoRefresh(() =>
      this.http.post<{ postId: number; liked: boolean; likes: number }>(
        this.endpointPath(`/posts/${postId}/likes`),
        {
          liked,
        },
        this.authOptions(),
      ),
    );
  }

  addComment(postId: number, payload: CommentCreateRequest): ApiCall<{ id: number; body: string; author: string }> {
    return this.withAutoRefresh(() =>
      this.http.post<{ id: number; body: string; author: string }>(
        this.endpointPath(`/posts/${postId}/comments`),
        payload,
        this.authOptions(),
      ),
    );
  }

  setFollowing(userId: number, following: boolean): ApiCall<{ targetUserId: number; following: boolean; followers: number }> {
    return this.withAutoRefresh(() =>
      this.http.post<{ targetUserId: number; following: boolean; followers: number }>(
        this.endpointPath(`/users/${userId}/follow`),
        { following },
        this.authOptions(),
      ),
    );
  }

  getNotifications(): ApiCall<{ items: DemoNotification[]; unread: number }> {
    return this.withAutoRefresh(() =>
      this.http.get<{ items: DemoNotification[]; unread: number }>(this.endpointPath('/notifications'), this.authOptions()),
    );
  }

  deletePost(postId: number): ApiCall<void> {
    return this.withAutoRefresh(() => this.http.delete<void>(this.endpointPath(`/posts/${postId}`), this.authOptions()));
  }

  healthCheck(): ApiCall<{ api: string; database: string; storage: string }> {
    return this.http.get<{ api: string; database: string; storage: string }>(this.endpointPath('/health'));
  }

  private withAutoRefresh<T>(requestFactory: () => Observable<T>): Observable<T> {
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
      this.refreshRequest = this.refreshSession(refreshToken).pipe(
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

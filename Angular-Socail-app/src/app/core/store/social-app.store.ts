import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { EMPTY_NOTIFICATIONS, EMPTY_POSTS, EMPTY_SUGGESTED_USERS, EMPTY_USER } from '../data/social-api.data';
import {
  AuthResponse,
  BackendPostResponse,
  DemoNotification,
  DemoPost,
  DemoUser,
  EntityId,
  POST_VISIBILITY,
  SuggestedUser,
  SystemActivity,
  UploadContentResponse,
} from '../models/social-api.models';
import { ApiSessionService } from '../services/api-session.service';
import { AuthApiService } from '../services/auth-api.service';
import { MediaApiService } from '../services/media-api.service';
import { NotificationsApiService } from '../services/notifications-api.service';
import { PostsApiService } from '../services/posts-api.service';
import { SystemApiService } from '../services/system-api.service';
import { UsersApiService } from '../services/users-api.service';

type ConnectionState = 'checking' | 'online' | 'offline';
type PostViewSource = Partial<Omit<DemoPost, 'caption'>> &
  Partial<BackendPostResponse> & {
    caption?: string | null;
  };

@Injectable({ providedIn: 'root' })
export class SocialAppStore {
  private readonly session = inject(ApiSessionService);
  private readonly authApi = inject(AuthApiService);
  private readonly usersApi = inject(UsersApiService);
  private readonly postsApi = inject(PostsApiService);
  private readonly mediaApi = inject(MediaApiService);
  private readonly notificationsApi = inject(NotificationsApiService);
  private readonly systemApi = inject(SystemApiService);
  private readonly connectionState = signal<ConnectionState>('checking');
  private readonly statusMessage = signal('Connecting to the Core API...');
  private readonly busyState = signal<string | null>(null);
  private readonly currentUserState = signal<DemoUser>(EMPTY_USER);
  private readonly postsState = signal<DemoPost[]>(EMPTY_POSTS);
  private readonly notificationsState = signal<DemoNotification[]>(EMPTY_NOTIFICATIONS);
  private readonly peopleState = signal<SuggestedUser[]>(EMPTY_SUGGESTED_USERS);
  private readonly activityState = signal<SystemActivity[]>([]);

  readonly apiBaseUrl = this.session.apiBaseUrl;
  readonly status = this.connectionState.asReadonly();
  readonly message = this.statusMessage.asReadonly();
  readonly busyMessage = this.busyState.asReadonly();
  readonly currentUser = this.currentUserState.asReadonly();
  readonly posts = this.postsState.asReadonly();
  readonly notifications = this.notificationsState.asReadonly();
  readonly people = this.peopleState.asReadonly();
  readonly activity = this.activityState.asReadonly();
  readonly isAuthenticated = computed(() => Boolean(this.session.accessToken()));

  readonly stats = computed(() => {
    const posts = this.postsState();
    const notifications = this.notificationsState();

    return {
      posts: posts.length,
      likes: posts.reduce((total, post) => total + post.likes, 0),
      comments: posts.reduce((total, post) => total + post.comments, 0),
      unread: notifications.filter((item) => item.unread).length,
    };
  });

  setBaseUrl(value: string): void {
    this.session.setBaseUrl(value);
  }

  async bootstrap(): Promise<void> {
    this.busyState.set('Connecting to backend services...');
    this.connectionState.set('checking');
    this.logActivity('Connection check started', this.session.apiBaseUrl(), 'info');

    try {
      await firstValueFrom(this.systemApi.healthCheck());
      this.connectionState.set('online');
      this.statusMessage.set('Connected to the Core API.');
      this.logActivity('Connection check passed', 'Backend health endpoint responded.', 'success');
    } catch {
      this.connectionState.set('offline');
      this.statusMessage.set('Backend not reachable. No local data was loaded.');
      this.logActivity('Connection check failed', 'Backend health endpoint did not respond.', 'error');
    }

    await this.loadCurrentUser();
    await Promise.allSettled([this.loadFeed(), this.loadNotifications()]);
    this.busyState.set(null);
  }

  async login(email: string, password: string): Promise<boolean> {
    this.busyState.set('Signing in...');
    this.logActivity('Login started', email, 'info');

    try {
      const auth = await firstValueFrom(this.authApi.login({ email, password }));
      await this.applyAuth(auth);
      await Promise.allSettled([this.loadFeed(), this.loadNotifications()]);
      this.connectionState.set('online');
      this.statusMessage.set('Signed in through the backend login service.');
      this.logActivity('Login succeeded', email, 'success');
      return true;
    } catch (error) {
      if (!this.isNetworkFailure(error)) {
        this.session.clearSession();
        this.statusMessage.set('Sign in failed. Check the email and password, then try again.');
        this.logActivity('Login rejected', 'The backend rejected these credentials.', 'error');
        return false;
      }

      this.connectionState.set('offline');
      this.statusMessage.set('Sign in could not complete because the backend is not reachable.');
      this.logActivity('Login unavailable', 'Backend login endpoint did not respond.', 'error');
      return false;
    } finally {
      this.busyState.set(null);
    }
  }

  async register(displayName: string, userName: string, email: string, password: string): Promise<boolean> {
    this.busyState.set('Creating account...');
    this.logActivity('Registration started', email, 'info');

    try {
      const auth = await firstValueFrom(this.authApi.register({ displayName, userName, email, password }));
      await this.applyAuth(auth);
      await Promise.allSettled([this.loadFeed(), this.loadNotifications()]);
      this.connectionState.set('online');
      this.statusMessage.set('Account created through the backend register service.');
      this.logActivity('Registration succeeded', userName || email, 'success');
      return true;
    } catch (error) {
      if (!this.isNetworkFailure(error)) {
        this.session.clearSession();
        this.statusMessage.set('Account creation failed. Check the user name, email, and password requirements.');
        this.logActivity('Registration rejected', 'The backend rejected the account request.', 'error');
        return false;
      }

      this.connectionState.set('offline');
      this.statusMessage.set('Account creation could not complete because the backend is not reachable.');
      this.logActivity('Registration unavailable', 'Backend register endpoint did not respond.', 'error');
      return false;
    } finally {
      this.busyState.set(null);
    }
  }

  async loadCurrentUser(): Promise<void> {
    try {
      this.currentUserState.set(this.normalizeUser(await firstValueFrom(this.usersApi.getCurrentProfile())));
      this.logActivity('Profile loaded', 'Current backend user profile was refreshed.', 'success');
    } catch {
      this.currentUserState.set(EMPTY_USER);
      this.logActivity('Profile unavailable', 'No current user profile was loaded.', 'warning');
    }
  }

  async loadFeed(): Promise<void> {
    const userId = this.currentUserState().id;

    if (!this.session.accessToken() || !this.isBackendGuid(userId)) {
      this.postsState.set(EMPTY_POSTS);
      this.logActivity('Posts skipped', 'A signed-in backend user id is required before loading posts.', 'warning');
      return;
    }

    try {
      const feed = await firstValueFrom(this.postsApi.getUserPosts(userId));
      const posts = await this.normalizeBackendPosts(feed.items ?? []);
      this.postsState.set(posts);
      this.logActivity('Posts loaded', `${posts.length} post(s) loaded from the backend.`, 'success');
    } catch {
      this.postsState.set(EMPTY_POSTS);
      this.logActivity('Posts unavailable', 'The backend posts request failed.', 'error');
    }
  }

  async loadNotifications(): Promise<void> {
    try {
      const response = await firstValueFrom(this.notificationsApi.getNotifications());
      const notifications = response.items ?? EMPTY_NOTIFICATIONS;
      this.notificationsState.set(notifications);
      this.logActivity('Notifications loaded', `${notifications.length} notification(s) loaded from the backend.`, 'success');
    } catch {
      this.notificationsState.set(EMPTY_NOTIFICATIONS);
      this.logActivity('Notifications unavailable', 'The backend notifications request failed.', 'warning');
    }
  }

  async searchPeople(query: string): Promise<void> {
    const trimmed = query.trim();

    if (trimmed.length < 2) {
      this.peopleState.set(EMPTY_SUGGESTED_USERS);
      return;
    }

    try {
      const response = await firstValueFrom(this.usersApi.searchUsers(trimmed));
      const people = response.items.map((user) => this.toSuggestedUser(user));
      this.peopleState.set(people);
      this.logActivity('People search completed', `${people.length} result(s) for "${trimmed}".`, 'success');
    } catch {
      this.peopleState.set(EMPTY_SUGGESTED_USERS);
      this.logActivity('People search failed', `No local users were loaded for "${trimmed}".`, 'error');
    }
  }

  async createPost(caption: string, file?: File): Promise<void> {
    const finalCaption = caption.trim();

    if (!file) {
      this.statusMessage.set('Choose an image before publishing because the posts API requires media.');
      this.logActivity('Post blocked', 'Publishing requires a real selected image file.', 'warning');
      return;
    }

    this.busyState.set(file ? 'Uploading image and publishing post...' : 'Publishing post...');
    this.logActivity('Post creation started', file.name, 'info');

    try {
      const uploaded = await this.uploadImage(file);
      const created = await firstValueFrom(
        this.postsApi.createPost({
          caption: finalCaption || undefined,
          mediaId: uploaded.mediaId,
          visibility: POST_VISIBILITY.public,
        }),
      );
      this.prependPost(this.normalizePost(created, finalCaption, this.mediaApi.publicAssetUrl(uploaded.publicUrl)));
      this.statusMessage.set('Post created through the backend posts service.');
      this.logActivity('Post created', created.postId, 'success');
    } catch {
      this.connectionState.set('offline');
      this.statusMessage.set('Post was not created because the backend post service is not reachable.');
      this.logActivity('Post creation failed', 'No local post was added.', 'error');
    } finally {
      this.busyState.set(null);
    }
  }

  async toggleLike(post: DemoPost): Promise<void> {
    this.statusMessage.set('Like was not changed because this backend has no like route yet.');
    this.logActivity('Like ignored', `No backend like endpoint exists for post ${post.id}.`, 'warning');
  }

  async addComment(post: DemoPost, body: string): Promise<void> {
    const comment = body.trim();
    if (!comment) {
      return;
    }

    this.statusMessage.set('Comment was not saved because this backend has no comments route yet.');
    this.logActivity('Comment ignored', `No backend comment endpoint exists for post ${post.id}.`, 'warning');
  }

  async deletePost(post: DemoPost): Promise<void> {
    const previousPosts = this.postsState();

    if (!this.isBackendGuid(post.id)) {
      this.statusMessage.set('Post was not deleted because it does not have a backend id.');
      this.logActivity('Delete skipped', 'Only backend posts with GUID ids can be deleted.', 'warning');
      return;
    }

    this.postsState.update((posts) => posts.filter((item) => item.id !== post.id));

    try {
      await firstValueFrom(this.postsApi.deletePost(post.id));
      this.statusMessage.set('Post deleted through the backend posts service.');
      this.logActivity('Post deleted', post.id.toString(), 'success');
    } catch {
      this.postsState.set(previousPosts);
      this.connectionState.set('offline');
      this.statusMessage.set('Post delete failed. The post was restored in the UI.');
      this.logActivity('Post delete failed', post.id.toString(), 'error');
    }
  }

  async followUser(user: SuggestedUser): Promise<void> {
    const following = !user.isFollowing;
    const previousPeople = this.peopleState();
    const previousUser = this.currentUserState();

    this.peopleState.update((people) =>
      people.map((item) => (item.id === user.id ? { ...item, isFollowing: following } : item)),
    );
    this.currentUserState.update((currentUser) => ({
      ...currentUser,
      following: currentUser.following + (following ? 1 : -1),
    }));

    try {
      await firstValueFrom(this.usersApi.setFollowing(user.id, following));
      this.statusMessage.set('Follow state saved through the backend network service.');
      this.logActivity(following ? 'User followed' : 'User unfollowed', user.handle || user.displayName, 'success');
    } catch {
      this.peopleState.set(previousPeople);
      this.currentUserState.set(previousUser);
      this.connectionState.set('offline');
      this.statusMessage.set('Follow change failed. The UI was restored.');
      this.logActivity('Follow change failed', user.handle || user.displayName, 'error');
    }
  }

  markNotificationsRead(): void {
    this.notificationsState.update((items) => items.map((item) => ({ ...item, unread: false })));
    this.logActivity('Notifications marked read', 'Unread counters were cleared in the UI.', 'info');
  }

  clearActivity(): void {
    this.activityState.set([]);
  }

  async logout(): Promise<void> {
    this.busyState.set('Revoking refresh token...');
    this.logActivity('Logout started', 'Revoking the current refresh token.', 'info');

    try {
      await firstValueFrom(this.authApi.revokeRefreshToken());
      this.statusMessage.set('Logged out. The backend refresh token was revoked.');
      this.logActivity('Logout completed', 'Refresh token revoked by the backend.', 'success');
    } catch {
      this.statusMessage.set('Local session cleared. The backend revoke call did not complete.');
      this.logActivity('Logout revoke failed', 'Local memory was cleared, but backend revoke did not complete.', 'warning');
    } finally {
      this.session.clearSession();
      this.currentUserState.set(EMPTY_USER);
      this.postsState.set(EMPTY_POSTS);
      this.notificationsState.set(EMPTY_NOTIFICATIONS);
      this.peopleState.set(EMPTY_SUGGESTED_USERS);
      this.connectionState.set('offline');
      this.busyState.set(null);
    }
  }

  private async uploadImage(file: File): Promise<UploadContentResponse> {
    const session = await firstValueFrom(this.mediaApi.createUploadSession());
    this.logActivity('Upload session created', session.mediaId, 'success');
    const formData = new FormData();
    formData.append('file', file, file.name);

    const uploaded = await firstValueFrom(this.mediaApi.uploadMediaContent(session.mediaId, formData));
    this.logActivity('Media uploaded', uploaded.mediaId, 'success');
    return uploaded;
  }

  private async applyAuth(auth: AuthResponse): Promise<void> {
    this.session.setSession(auth);

    if (auth.user) {
      this.currentUserState.set(this.normalizeUser(auth.user));
    } else {
      await this.loadCurrentUser();
    }
  }

  private prependPost(post: DemoPost): void {
    this.postsState.update((posts) => [post, ...posts]);
    this.currentUserState.update((user) => ({ ...user, posts: user.posts + 1 }));
  }

  private async normalizeBackendPosts(posts: BackendPostResponse[]): Promise<DemoPost[]> {
    return Promise.all(
      posts.map(async (post) => {
        const imageUrl = await this.resolvePostImageUrl(post);
        return this.normalizePost(post, post.caption ?? '', imageUrl);
      }),
    );
  }

  private async resolvePostImageUrl(post: BackendPostResponse): Promise<string | undefined> {
    try {
      const media = await firstValueFrom(this.mediaApi.getMedia(post.mediaId));
      return this.mediaApi.publicAssetUrl(media.publicUrl);
    } catch {
      return undefined;
    }
  }

  private normalizePost(post: PostViewSource, caption: string, imageUrl?: string): DemoPost {
    const user = this.currentUserState();

    return {
      id: post.id ?? post.postId ?? `${Date.now()}`,
      author: post.author ?? user.displayName,
      handle: post.handle ?? user.handle,
      avatarUrl: post.avatarUrl ?? user.avatarUrl,
      imageUrl: post.imageUrl ?? imageUrl ?? '',
      caption: post.caption ?? (caption || 'Untitled image post'),
      likes: post.likes ?? post.likeCount ?? 0,
      comments: post.comments ?? post.commentCount ?? 0,
      liked: post.liked ?? false,
      createdAt: post.createdAt ?? this.formatPostDate(post.createdAtUtc),
    };
  }

  private toSuggestedUser(user: DemoUser): SuggestedUser {
    return {
      ...user,
      bio: 'Discovered from backend people search.',
      isFollowing: false,
    };
  }

  private normalizeUser(user: Partial<DemoUser> & { userId?: EntityId; userName?: string }): DemoUser {
    return {
      ...EMPTY_USER,
      id: user.id ?? user.userId ?? EMPTY_USER.id,
      displayName: user.displayName ?? user.userName ?? EMPTY_USER.displayName,
      handle: user.handle ?? (user.userName ? `@${user.userName}` : EMPTY_USER.handle),
      avatarUrl: user.avatarUrl ?? EMPTY_USER.avatarUrl,
      followers: user.followers ?? EMPTY_USER.followers,
      following: user.following ?? EMPTY_USER.following,
      posts: user.posts ?? EMPTY_USER.posts,
    };
  }

  private isBackendGuid(value: EntityId): value is string {
    return (
      typeof value === 'string' &&
      /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value)
    );
  }

  private formatPostDate(value?: string): string {
    if (!value) {
      return 'just now';
    }

    const createdAt = new Date(value);

    if (Number.isNaN(createdAt.getTime())) {
      return 'just now';
    }

    const elapsedMinutes = Math.max(0, Math.floor((Date.now() - createdAt.getTime()) / 60000));

    if (elapsedMinutes < 1) {
      return 'just now';
    }

    if (elapsedMinutes < 60) {
      return `${elapsedMinutes} min ago`;
    }

    const elapsedHours = Math.floor(elapsedMinutes / 60);

    if (elapsedHours < 24) {
      return `${elapsedHours} hr ago`;
    }

    return createdAt.toLocaleDateString();
  }

  private isNetworkFailure(error: unknown): boolean {
    return typeof error === 'object' && error !== null && 'status' in error && (error as { status?: number }).status === 0;
  }

  private logActivity(title: string, detail: string, level: SystemActivity['level']): void {
    const event: SystemActivity = {
      id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
      title,
      detail,
      level,
      createdAt: new Date().toLocaleTimeString(),
    };

    this.activityState.update((events) => [event, ...events].slice(0, 30));
  }
}

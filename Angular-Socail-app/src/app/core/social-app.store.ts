import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { DEMO_NOTIFICATIONS, DEMO_POSTS, DEMO_USER, SUGGESTED_USERS } from './social-api.data';
import { AuthResponse, DemoNotification, DemoPost, DemoUser, SuggestedUser } from './social-api.models';
import { SocialApiClient } from './social-api-client.service';

type ConnectionState = 'checking' | 'online' | 'demo';

@Injectable({ providedIn: 'root' })
export class SocialAppStore {
  private readonly client = inject(SocialApiClient);
  private readonly connectionState = signal<ConnectionState>('checking');
  private readonly statusMessage = signal('Connecting to the Core API...');
  private readonly busyState = signal<string | null>(null);
  private readonly currentUserState = signal<DemoUser>(DEMO_USER);
  private readonly postsState = signal<DemoPost[]>(DEMO_POSTS);
  private readonly notificationsState = signal<DemoNotification[]>(DEMO_NOTIFICATIONS);
  private readonly peopleState = signal<SuggestedUser[]>(SUGGESTED_USERS);

  readonly apiBaseUrl = this.client.apiBaseUrl;
  readonly status = this.connectionState.asReadonly();
  readonly message = this.statusMessage.asReadonly();
  readonly busyMessage = this.busyState.asReadonly();
  readonly currentUser = this.currentUserState.asReadonly();
  readonly posts = this.postsState.asReadonly();
  readonly notifications = this.notificationsState.asReadonly();
  readonly people = this.peopleState.asReadonly();
  readonly isAuthenticated = computed(() => Boolean(this.client.accessToken()));

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
    this.client.setBaseUrl(value);
  }

  async bootstrap(): Promise<void> {
    this.busyState.set('Connecting to backend services...');
    this.connectionState.set('checking');

    try {
      await firstValueFrom(this.client.healthCheck());
      this.connectionState.set('online');
      this.statusMessage.set('Connected to the Core API.');
    } catch {
      this.connectionState.set('demo');
      this.statusMessage.set('Backend not reachable yet. The app is using local fallback data until your Core API runs.');
    }

    await Promise.allSettled([this.loadCurrentUser(), this.loadFeed(), this.loadNotifications()]);
    this.busyState.set(null);
  }

  async login(email: string, password: string): Promise<void> {
    this.busyState.set('Signing in...');

    try {
      const auth = await firstValueFrom(this.client.login({ email, password }));
      this.applyAuth(auth);
      this.connectionState.set('online');
      this.statusMessage.set('Signed in through the backend login service.');
    } catch {
      this.applyDemoSession('Signed in locally because the backend login service is not reachable.');
    } finally {
      this.busyState.set(null);
    }
  }

  async register(displayName: string, userName: string, email: string, password: string): Promise<void> {
    this.busyState.set('Creating account...');

    try {
      const auth = await firstValueFrom(this.client.register({ displayName, userName, email, password }));
      this.applyAuth(auth);
      this.connectionState.set('online');
      this.statusMessage.set('Account created through the backend register service.');
    } catch {
      this.currentUserState.set({
        ...DEMO_USER,
        displayName: displayName || DEMO_USER.displayName,
        handle: userName ? `@${userName.replace(/^@/, '')}` : DEMO_USER.handle,
      });
      this.applyDemoSession('Account preview created locally because the backend register service is not reachable.');
    } finally {
      this.busyState.set(null);
    }
  }

  async loadCurrentUser(): Promise<void> {
    try {
      this.currentUserState.set(this.normalizeUser(await firstValueFrom(this.client.getCurrentProfile())));
    } catch {
      this.currentUserState.set(DEMO_USER);
    }
  }

  async loadFeed(): Promise<void> {
    try {
      const feed = await firstValueFrom(this.client.getFeed());
      this.postsState.set(feed.items?.length ? feed.items : DEMO_POSTS);
    } catch {
      this.postsState.set(DEMO_POSTS);
    }
  }

  async loadNotifications(): Promise<void> {
    try {
      const response = await firstValueFrom(this.client.getNotifications());
      this.notificationsState.set(response.items?.length ? response.items : DEMO_NOTIFICATIONS);
    } catch {
      this.notificationsState.set(DEMO_NOTIFICATIONS);
    }
  }

  async searchPeople(query: string): Promise<void> {
    const trimmed = query.trim();

    if (trimmed.length < 2) {
      this.peopleState.set(SUGGESTED_USERS);
      return;
    }

    try {
      const response = await firstValueFrom(this.client.searchUsers(trimmed));
      this.peopleState.set(response.items.map((user) => this.toSuggestedUser(user)));
    } catch {
      const lower = trimmed.toLowerCase();
      this.peopleState.set(
        SUGGESTED_USERS.filter(
          (user) => user.displayName.toLowerCase().includes(lower) || user.handle.toLowerCase().includes(lower),
        ),
      );
    }
  }

  async createPost(caption: string, file?: File): Promise<void> {
    const finalCaption = caption.trim() || 'A new image post from the Angular app.';
    this.busyState.set(file ? 'Uploading image and publishing post...' : 'Publishing post...');

    try {
      const imageId = file ? await this.uploadImage(file, finalCaption) : 772;
      const created = await firstValueFrom(
        this.client.createPost({
          caption: finalCaption,
          imageId,
          visibility: 'public',
        }),
      );
      this.prependPost(this.normalizePost(created, finalCaption));
      this.statusMessage.set('Post created through the backend posts service.');
    } catch {
      this.prependPost(this.fallbackPost(finalCaption, file));
      this.connectionState.set('demo');
      this.statusMessage.set('Post added locally because the backend post service is not reachable.');
    } finally {
      this.busyState.set(null);
    }
  }

  async toggleLike(post: DemoPost): Promise<void> {
    const nextLiked = !post.liked;
    this.postsState.update((posts) =>
      posts.map((item) =>
        item.id === post.id
          ? {
              ...item,
              liked: nextLiked,
              likes: item.likes + (nextLiked ? 1 : -1),
            }
          : item,
      ),
    );

    try {
      await firstValueFrom(this.client.setPostLike(post.id, nextLiked));
      this.statusMessage.set('Like saved through the backend engagement service.');
    } catch {
      this.connectionState.set('demo');
      this.statusMessage.set('Like changed locally because the backend engagement service is not reachable.');
    }
  }

  async addComment(post: DemoPost, body: string): Promise<void> {
    const comment = body.trim();
    if (!comment) {
      return;
    }

    this.postsState.update((posts) =>
      posts.map((item) => (item.id === post.id ? { ...item, comments: item.comments + 1 } : item)),
    );

    try {
      await firstValueFrom(this.client.addComment(post.id, { body: comment }));
      this.statusMessage.set('Comment saved through the backend comments service.');
    } catch {
      this.connectionState.set('demo');
      this.statusMessage.set('Comment counted locally because the backend comments service is not reachable.');
    }
  }

  async deletePost(post: DemoPost): Promise<void> {
    this.postsState.update((posts) => posts.filter((item) => item.id !== post.id));

    try {
      await firstValueFrom(this.client.deletePost(post.id));
      this.statusMessage.set('Post deleted through the backend posts service.');
    } catch {
      this.connectionState.set('demo');
      this.statusMessage.set('Post removed locally because the backend delete service is not reachable.');
    }
  }

  async followUser(user: SuggestedUser): Promise<void> {
    const following = !user.isFollowing;

    this.peopleState.update((people) =>
      people.map((item) => (item.id === user.id ? { ...item, isFollowing: following } : item)),
    );
    this.currentUserState.update((currentUser) => ({
      ...currentUser,
      following: currentUser.following + (following ? 1 : -1),
    }));

    try {
      await firstValueFrom(this.client.setFollowing(user.id, following));
      this.statusMessage.set('Follow state saved through the backend network service.');
    } catch {
      this.connectionState.set('demo');
      this.statusMessage.set('Follow state changed locally because the backend network service is not reachable.');
    }
  }

  markNotificationsRead(): void {
    this.notificationsState.update((items) => items.map((item) => ({ ...item, unread: false })));
  }

  async logout(): Promise<void> {
    this.busyState.set('Revoking refresh token...');

    try {
      await firstValueFrom(this.client.revokeRefreshToken());
      this.statusMessage.set('Logged out. The backend refresh token was revoked.');
    } catch {
      this.statusMessage.set('Local session cleared. The backend revoke call did not complete.');
    } finally {
      this.client.clearSession();
      this.currentUserState.set(DEMO_USER);
      this.connectionState.set('demo');
      this.busyState.set(null);
    }
  }

  private async uploadImage(file: File, altText: string): Promise<number> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('altText', altText);

    const uploaded = await firstValueFrom(this.client.uploadImage(formData));
    return uploaded.imageId;
  }

  private applyAuth(auth: AuthResponse): void {
    this.client.setSession(auth);

    if (auth.user) {
      this.currentUserState.set(this.normalizeUser(auth.user));
    } else {
      void this.loadCurrentUser();
    }
  }

  private applyDemoSession(message: string): void {
    this.client.setToken('demo-token');
    this.connectionState.set('demo');
    this.statusMessage.set(message);
  }

  private prependPost(post: DemoPost): void {
    this.postsState.update((posts) => [post, ...posts]);
    this.currentUserState.update((user) => ({ ...user, posts: user.posts + 1 }));
  }

  private normalizePost(post: DemoPost, caption: string): DemoPost {
    const user = this.currentUserState();

    return {
      id: post.id ?? Date.now(),
      author: post.author ?? user.displayName,
      handle: post.handle ?? user.handle,
      avatarUrl: post.avatarUrl ?? user.avatarUrl,
      imageUrl:
        post.imageUrl ?? 'https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=900&q=80',
      caption: post.caption ?? caption,
      likes: post.likes ?? 0,
      comments: post.comments ?? 0,
      liked: post.liked ?? false,
      createdAt: post.createdAt ?? 'just now',
    };
  }

  private fallbackPost(caption: string, file?: File): DemoPost {
    const user = this.currentUserState();

    return {
      id: Date.now(),
      author: user.displayName,
      handle: user.handle,
      avatarUrl: user.avatarUrl,
      imageUrl: file
        ? URL.createObjectURL(file)
        : 'https://images.unsplash.com/photo-1498050108023-c5249f4df085?auto=format&fit=crop&w=900&q=80',
      caption,
      likes: 0,
      comments: 0,
      liked: false,
      createdAt: 'just now',
    };
  }

  private toSuggestedUser(user: DemoUser): SuggestedUser {
    return {
      ...user,
      bio: 'Discovered from backend people search.',
      isFollowing: false,
    };
  }

  private normalizeUser(user: Partial<DemoUser> & { userId?: number; userName?: string }): DemoUser {
    return {
      ...DEMO_USER,
      id: user.id ?? user.userId ?? DEMO_USER.id,
      displayName: user.displayName ?? user.userName ?? DEMO_USER.displayName,
      handle: user.handle ?? (user.userName ? `@${user.userName}` : DEMO_USER.handle),
      avatarUrl: user.avatarUrl ?? DEMO_USER.avatarUrl,
      followers: user.followers ?? DEMO_USER.followers,
      following: user.following ?? DEMO_USER.following,
      posts: user.posts ?? DEMO_USER.posts,
    };
  }
}

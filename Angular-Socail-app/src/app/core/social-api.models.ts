import { Observable } from 'rxjs';

export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export type ApiArea =
  | 'Auth'
  | 'Users'
  | 'Posts'
  | 'Images'
  | 'Engagement'
  | 'Network'
  | 'System';

export type ApiFilter = ApiArea | 'All';

export interface SocialApiFunction {
  id: string;
  area: ApiArea;
  name: string;
  method: HttpMethod;
  path: string;
  auth: 'Public' | 'Bearer token' | 'Owner token' | 'Admin token';
  goal: string;
  request: string;
  response: string;
  uiMoments: string[];
  backendNotes: string[];
  failureModes: string[];
}

export interface DemoUser {
  id: number;
  displayName: string;
  handle: string;
  avatarUrl: string;
  followers: number;
  following: number;
  posts: number;
}

export interface SuggestedUser extends DemoUser {
  bio: string;
  isFollowing: boolean;
}

export interface DemoPost {
  id: number;
  author: string;
  handle: string;
  avatarUrl: string;
  imageUrl: string;
  caption: string;
  likes: number;
  comments: number;
  liked: boolean;
  createdAt: string;
}

export interface DemoNotification {
  id: number;
  title: string;
  body: string;
  unread: boolean;
}

export interface DemoRun {
  id: number;
  functionId: string;
  status: number;
  endpoint: string;
  headline: string;
  request: string;
  response: string;
  timeline: string[];
  createdAt: string;
}

export interface RegisterRequest {
  displayName: string;
  userName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken?: string;
  expiresAtUtc?: string;
  user?: DemoUser;
}

export interface RefreshSessionResponse {
  accessToken: string;
  refreshToken?: string;
  expiresAtUtc?: string;
}

export interface ProfileUpdateRequest {
  displayName: string;
  bio: string;
  location: string;
  website?: string;
}

export interface PostCreateRequest {
  caption: string;
  imageId: number;
  visibility: 'public' | 'followers';
}

export interface CommentCreateRequest {
  body: string;
}

export type ApiCall<T> = Observable<T>;

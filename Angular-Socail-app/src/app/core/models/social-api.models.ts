import { Observable } from 'rxjs';

export type EntityId = string | number;

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
  id: EntityId;
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
  id: EntityId;
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
  id: EntityId;
  title: string;
  body: string;
  unread: boolean;
}

export interface SystemActivity {
  id: EntityId;
  title: string;
  detail: string;
  level: 'info' | 'success' | 'warning' | 'error';
  createdAt: string;
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

export const POST_VISIBILITY = {
  public: 1,
  followersOnly: 2,
  private: 3,
} as const;

export type PostVisibility = (typeof POST_VISIBILITY)[keyof typeof POST_VISIBILITY];

export interface PostCreateRequest {
  caption?: string;
  mediaId: string;
  visibility: PostVisibility;
}

export interface BackendPostResponse {
  postId: string;
  userId?: string;
  mediaId: string;
  caption?: string | null;
  visibility?: PostVisibility;
  likeCount?: number;
  commentCount?: number;
  createdAtUtc?: string;
}

export interface BackendPostsResponse {
  items: BackendPostResponse[];
}

export interface CreateUploadSessionResponse {
  mediaId: string;
  status: number;
}

export interface UploadContentResponse {
  mediaId: string;
  publicUrl: string;
  status: number;
}

export interface MediaResponse {
  mediaId: string;
  originalFileName?: string | null;
  publicUrl?: string | null;
  contentType?: string | null;
  fileSizeInBytes?: number | null;
  mediaType: number;
  status: number;
  createdAtUtc: string;
}

export interface CommentCreateRequest {
  body: string;
}

export type ApiCall<T> = Observable<T>;

import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiCall, BackendPostResponse, BackendPostsResponse, EntityId, PostCreateRequest } from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class PostsApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  createPost(payload: PostCreateRequest): ApiCall<BackendPostResponse> {
    return this.session.withAutoRefresh(() =>
      this.http.post<BackendPostResponse>(this.session.endpointPath('/posts'), payload, this.session.authOptions()),
    );
  }

  getUserPosts(userId: EntityId, page = 1, pageSize = 20): ApiCall<BackendPostsResponse> {
    return this.session.withAutoRefresh(() =>
      this.http.get<BackendPostsResponse>(
        this.session.endpointPath(`/posts/user/${userId}?page=${page}&pageSize=${pageSize}`),
        this.session.authOptions(),
      ),
    );
  }

  getPost(postId: EntityId): ApiCall<BackendPostResponse> {
    return this.session.withAutoRefresh(() =>
      this.http.get<BackendPostResponse>(this.session.endpointPath(`/posts/${postId}`), this.session.authOptions()),
    );
  }

  deletePost(postId: EntityId): ApiCall<void> {
    return this.session.withAutoRefresh(() =>
      this.http.delete<void>(this.session.endpointPath(`/posts/${postId}`), this.session.authOptions()),
    );
  }
}

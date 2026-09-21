import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import {
  ApiCall,
  CreateUploadSessionResponse,
  EntityId,
  MediaResponse,
  UploadContentResponse,
} from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class MediaApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  createUploadSession(): ApiCall<CreateUploadSessionResponse> {
    return this.session.withAutoRefresh(() =>
      this.http.post<CreateUploadSessionResponse>(
        this.session.endpointPath('/media/upload-session'),
        {
          mediaType: 1,
        },
        this.session.authOptions(),
      ),
    );
  }

  uploadMediaContent(mediaId: EntityId, payload: FormData): ApiCall<UploadContentResponse> {
    return this.session.withAutoRefresh(() =>
      this.http.post<UploadContentResponse>(
        this.session.endpointPath(`/media/${mediaId}/content`),
        payload,
        this.session.authOptions(),
      ),
    );
  }

  getMedia(mediaId: EntityId): ApiCall<MediaResponse> {
    return this.http.get<MediaResponse>(this.session.endpointPath(`/media/${mediaId}`));
  }

  publicAssetUrl(publicUrl?: string | null): string | undefined {
    return this.session.publicAssetUrl(publicUrl);
  }
}

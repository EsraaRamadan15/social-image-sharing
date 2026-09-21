import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { ApiCall } from '../models/social-api.models';
import { ApiSessionService } from './api-session.service';

@Injectable({ providedIn: 'root' })
export class SystemApiService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(ApiSessionService);

  healthCheck(): ApiCall<{ api: string; database: string; storage: string }> {
    return this.http.get<{ api: string; database: string; storage: string }>(this.session.endpointPath('/health'));
  }
}

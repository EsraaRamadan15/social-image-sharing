# Social Image Sharing Angular App

This is an Angular v22 + Tailwind v4 frontend for a Core social image-sharing backend.

The app is split into real UI components and connects through backend-facing services:

- `ConnectionBarComponent`: changes the backend API URL and checks connection health.
- `AuthPanelComponent`: calls login and register backend services.
- `ProfilePanelComponent`: loads the current user profile.
- `PostComposerComponent`: uploads an image and creates a post.
- `FeedComponent` and `PostCardComponent`: load posts, like, comment, and delete.
- `PeoplePanelComponent`: searches people and follows/unfollows users.
- `NotificationsPanelComponent`: loads activity and marks notifications as read.

The backend service layer lives in `src/app/core/social-api-client.service.ts`. The app store in `src/app/core/social-app.store.ts` calls those services and uses local fallback data only when the backend is not reachable.

## Auth flow

- Access token is stored only in memory inside `SocialApiClient`, using an Angular signal.
- Refresh token is also stored in memory after login/register, and is sent to `/auth/refresh` only when a protected request returns `401`.
- Protected API calls retry once after refresh succeeds.
- Logout calls `/auth/logout` with the refresh token, then clears both tokens from memory.
- Because tokens are memory-only, refreshing the browser page clears the session unless the backend uses HttpOnly cookies or another restore mechanism.

## Run

```bash
npm start
```

Then open `http://localhost:4200/`.

The project includes a local Node 24.15 runtime as a dev dependency because Angular v22 requires Node 24.15 or newer, while this machine currently has Node 24.12 installed globally.

## Build

```bash
npm run build
```

## Test

```bash
npm test
```

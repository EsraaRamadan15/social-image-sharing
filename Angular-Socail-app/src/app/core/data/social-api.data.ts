import { DemoNotification, DemoPost, DemoUser, SuggestedUser } from '../models/social-api.models';

export const EMPTY_USER: DemoUser = {
  id: '',
  displayName: 'Not signed in',
  handle: '',
  avatarUrl: '',
  followers: 0,
  following: 0,
  posts: 0,
};

export const EMPTY_POSTS: DemoPost[] = [];

export const EMPTY_NOTIFICATIONS: DemoNotification[] = [];

export const EMPTY_SUGGESTED_USERS: SuggestedUser[] = [];

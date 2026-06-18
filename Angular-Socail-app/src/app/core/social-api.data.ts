import { DemoNotification, DemoPost, DemoUser, SuggestedUser } from './social-api.models';

export const DEMO_USER: DemoUser = {
  id: 18,
  displayName: 'Mona Kamel',
  handle: '@mona',
  avatarUrl: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=120&q=80',
  followers: 1284,
  following: 311,
  posts: 42,
};

export const DEMO_POSTS: DemoPost[] = [
  {
    id: 9041,
    author: 'Mona Kamel',
    handle: '@mona',
    avatarUrl: DEMO_USER.avatarUrl,
    imageUrl: 'https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80',
    caption: 'Evening colors from the bridge.',
    likes: 88,
    comments: 12,
    liked: false,
    createdAt: '8 min ago',
  },
  {
    id: 9038,
    author: 'Nour Adel',
    handle: '@nour',
    avatarUrl: 'https://images.unsplash.com/photo-1508214751196-bcfd4ca60f91?auto=format&fit=crop&w=120&q=80',
    imageUrl: 'https://images.unsplash.com/photo-1518005020951-eccb494ad742?auto=format&fit=crop&w=900&q=80',
    caption: 'Geometry hunt on a quiet morning.',
    likes: 241,
    comments: 31,
    liked: true,
    createdAt: '34 min ago',
  },
];

export const DEMO_NOTIFICATIONS: DemoNotification[] = [
  {
    id: 1,
    title: 'New comment',
    body: '@nour commented on your bridge photo.',
    unread: true,
  },
  {
    id: 2,
    title: 'Profile follow',
    body: '@yara started following you.',
    unread: true,
  },
  {
    id: 3,
    title: 'Upload ready',
    body: 'Your last image passed validation.',
    unread: false,
  },
];

export const SUGGESTED_USERS: SuggestedUser[] = [
  {
    id: 42,
    displayName: 'Yara Samir',
    handle: '@yara',
    avatarUrl: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80',
    followers: 2481,
    following: 186,
    posts: 77,
    bio: 'Portraits, motion blur, and rooftop light.',
    isFollowing: false,
  },
  {
    id: 51,
    displayName: 'Omar Helmy',
    handle: '@omar',
    avatarUrl: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=120&q=80',
    followers: 972,
    following: 204,
    posts: 31,
    bio: 'Minimal city frames and backend notes.',
    isFollowing: true,
  },
  {
    id: 63,
    displayName: 'Laila Hassan',
    handle: '@laila',
    avatarUrl: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=120&q=80',
    followers: 5302,
    following: 411,
    posts: 118,
    bio: 'Food, travel, and color-rich photo sets.',
    isFollowing: false,
  },
];

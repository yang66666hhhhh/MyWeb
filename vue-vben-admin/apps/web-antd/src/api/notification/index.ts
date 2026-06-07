import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface NotificationItem {
  id: string;
  userId: string;
  title: string;
  content?: string;
  type: 'Ai' | 'Finance' | 'Habit' | 'System' | 'Task';
  isRead: boolean;
  readAt?: string;
  link?: string;
  createdAt: string;
}

export interface NotificationQuery extends PageQuery {
  type?: string;
  isRead?: boolean;
  keyword?: string;
}

export interface UnreadCountDto {
  count: number;
}

export const notificationApi = {
  getPage: (params?: NotificationQuery) =>
    requestClient.get<PageResult<NotificationItem>>('/notifications', { params }),

  getUnreadCount: () =>
    requestClient.get<UnreadCountDto>('/notifications/unread-count'),

  markAsRead: (id: string) =>
    requestClient.put(`/notifications/${id}/read`),

  markAllAsRead: () =>
    requestClient.put('/notifications/read-all'),

  delete: (id: string) =>
    requestClient.delete(`/notifications/${id}`),
};

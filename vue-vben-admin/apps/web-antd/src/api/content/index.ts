import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface Article {
  id: string;
  userId?: string;
  title: string;
  content?: string;
  status: string;
  tags?: string;
  category?: string;
  publishedAt?: string;
  remark?: string;
  createdAt: string;
}

export interface MediaItem {
  id: string;
  userId?: string;
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  tags?: string;
  remark?: string;
  createdAt: string;
}

export interface PublishingCalendar {
  id: string;
  userId?: string;
  plannedDate: string;
  platform: string;
  title: string;
  status: string;
  remark?: string;
  createdAt: string;
}

export interface ContentQuery extends PageQuery {
  status?: string;
  category?: string;
  keyword?: string;
  startDate?: string;
  endDate?: string;
}

export interface CreateArticleInput {
  title: string;
  content?: string;
  status?: string;
  tags?: string;
  category?: string;
  remark?: string;
}

export interface UpdateArticleInput {
  title?: string;
  content?: string;
  status?: string;
  tags?: string;
  category?: string;
  remark?: string;
}

export interface CreateMediaItemInput {
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  tags?: string;
  remark?: string;
}

export interface UpdateMediaItemInput {
  fileName?: string;
  fileUrl?: string;
  fileType?: string;
  fileSize?: number;
  tags?: string;
  remark?: string;
}

export interface CreatePublishingCalendarInput {
  plannedDate: string;
  platform: string;
  title: string;
  status?: string;
  remark?: string;
}

export interface UpdatePublishingCalendarInput {
  plannedDate?: string;
  platform?: string;
  title?: string;
  status?: string;
  remark?: string;
}

export const contentApi = {
  getArticles: (params: ContentQuery) =>
    requestClient.get<PageResult<Article>>('/content/articles', { params }),

  getArticleById: (id: string) =>
    requestClient.get<Article>(`/content/articles/${id}`),

  createArticle: (data: CreateArticleInput) =>
    requestClient.post<Article>('/content/articles', data),

  updateArticle: (id: string, data: UpdateArticleInput) =>
    requestClient.put<Article>(`/content/articles/${id}`, data),

  deleteArticle: (id: string) =>
    requestClient.delete(`/content/articles/${id}`),

  getMediaItems: (params: ContentQuery) =>
    requestClient.get<PageResult<MediaItem>>('/content/media', { params }),

  getMediaItemById: (id: string) =>
    requestClient.get<MediaItem>(`/content/media/${id}`),

  createMediaItem: (data: CreateMediaItemInput) =>
    requestClient.post<MediaItem>('/content/media', data),

  updateMediaItem: (id: string, data: UpdateMediaItemInput) =>
    requestClient.put<MediaItem>(`/content/media/${id}`, data),

  deleteMediaItem: (id: string) =>
    requestClient.delete(`/content/media/${id}`),

  getCalendarItems: (params: ContentQuery) =>
    requestClient.get<PageResult<PublishingCalendar>>('/content/calendar', { params }),

  getCalendarItemById: (id: string) =>
    requestClient.get<PublishingCalendar>(`/content/calendar/${id}`),

  createCalendarItem: (data: CreatePublishingCalendarInput) =>
    requestClient.post<PublishingCalendar>('/content/calendar', data),

  updateCalendarItem: (id: string, data: UpdatePublishingCalendarInput) =>
    requestClient.put<PublishingCalendar>(`/content/calendar/${id}`, data),

  deleteCalendarItem: (id: string) =>
    requestClient.delete(`/content/calendar/${id}`),
};

export const getArticlePageApi = contentApi.getArticles;
export const createArticleApi = contentApi.createArticle;
export const updateArticleApi = contentApi.updateArticle;
export const deleteArticleApi = contentApi.deleteArticle;
export const getMediaPageApi = contentApi.getMediaItems;
export const createMediaApi = contentApi.createMediaItem;
export const updateMediaApi = contentApi.updateMediaItem;
export const deleteMediaApi = contentApi.deleteMediaItem;
export const getCalendarPageApi = contentApi.getCalendarItems;
export const createCalendarApi = contentApi.createCalendarItem;
export const updateCalendarApi = contentApi.updateCalendarItem;
export const deleteCalendarApi = contentApi.deleteCalendarItem;

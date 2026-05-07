import type { BaseEntity, PageQuery, PageResult } from './types';

import { requestClient } from '#/api/request';

export interface KnowledgeArticle extends BaseEntity {
  category: string;
  content?: null | string;
  isPublished: boolean;
  tags?: null | string;
  title: string;
  viewCount: number;
}

export interface KnowledgeArticleQuery extends PageQuery {
  category?: string;
  isPublished?: boolean;
  keyword?: string;
}

export interface SaveKnowledgeArticleInput {
  category: string;
  content?: null | string;
  isPublished?: boolean;
  tags?: null | string;
  title: string;
}

export async function getKnowledgeArticlePageApi(params: KnowledgeArticleQuery) {
  return requestClient.get<PageResult<KnowledgeArticle>>('/growth/knowledge-base', { params });
}

export async function getKnowledgeArticleApi(id: string) {
  return requestClient.get<KnowledgeArticle>(`/growth/knowledge-base/${id}`);
}

export async function createKnowledgeArticleApi(data: SaveKnowledgeArticleInput) {
  return requestClient.post<KnowledgeArticle>('/growth/knowledge-base', data);
}

export async function updateKnowledgeArticleApi(
  id: string,
  data: SaveKnowledgeArticleInput,
) {
  return requestClient.put<KnowledgeArticle>(`/growth/knowledge-base/${id}`, data);
}

export async function deleteKnowledgeArticleApi(id: string) {
  return requestClient.delete(`/growth/knowledge-base/${id}`);
}
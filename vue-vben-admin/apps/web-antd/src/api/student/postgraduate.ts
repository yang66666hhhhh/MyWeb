import type { BaseEntity, PageQuery, PageResult } from '../growth/types';

import { requestClient } from '#/api/request';

const postgraduateBaseUrl = '/student/postgraduate';

export interface PostgraduateTask extends BaseEntity {
  title: string;
  description?: null | string;
  dueDate?: null | string;
  status: number;
  priority: number;
  type: number;
}

export interface ExamMistake extends BaseEntity {
  question: string;
  answer?: null | string;
  explanation?: null | string;
  subject: string;
  tags?: null | string;
  status: number;
  reviewCount: number;
  lastReviewDate?: null | string;
  nextReviewDate?: null | string;
}

export interface ExamMaterial extends BaseEntity {
  title: string;
  content?: null | string;
  subject: string;
  tags?: null | string;
  type: number;
}

export interface PostgraduateTaskQuery extends PageQuery {
  keyword?: string;
  status?: number;
  type?: number;
}

export interface ExamMistakeQuery extends PageQuery {
  keyword?: string;
  subject?: string;
}

export interface ExamMaterialQuery extends PageQuery {
  keyword?: string;
  subject?: string;
  type?: number;
}

export type SavePostgraduateTaskInput = Omit<PostgraduateTask, 'createdAt' | 'id' | 'updatedAt'>;
export interface SaveExamMistakeInput {
  question: string;
  answer?: null | string;
  explanation?: null | string;
  subject: string;
  tags?: null | string;
  status?: number;
  reviewCount?: number;
  lastReviewDate?: null | string;
  nextReviewDate?: null | string;
}
export type SaveExamMaterialInput = Omit<ExamMaterial, 'createdAt' | 'id' | 'updatedAt'>;

export async function getPostgraduateTaskPageApi(params: PostgraduateTaskQuery) {
  return requestClient.get<PageResult<PostgraduateTask>>(`${postgraduateBaseUrl}/tasks`, { params });
}

export async function getPostgraduateTaskApi(id: string) {
  return requestClient.get<PostgraduateTask>(`${postgraduateBaseUrl}/tasks/${id}`);
}

export async function createPostgraduateTaskApi(data: SavePostgraduateTaskInput) {
  return requestClient.post<PostgraduateTask>(`${postgraduateBaseUrl}/tasks`, data);
}

export async function updatePostgraduateTaskApi(id: string, data: SavePostgraduateTaskInput) {
  return requestClient.put<PostgraduateTask>(`${postgraduateBaseUrl}/tasks/${id}`, data);
}

export async function deletePostgraduateTaskApi(id: string) {
  return requestClient.delete(`${postgraduateBaseUrl}/tasks/${id}`);
}

export async function getMistakePageApi(params: ExamMistakeQuery) {
  return requestClient.get<PageResult<ExamMistake>>(`${postgraduateBaseUrl}/mistakes`, { params });
}

export async function getMistakeApi(id: string) {
  return requestClient.get<ExamMistake>(`${postgraduateBaseUrl}/mistakes/${id}`);
}

export async function createMistakeApi(data: SaveExamMistakeInput) {
  return requestClient.post<ExamMistake>(`${postgraduateBaseUrl}/mistakes`, data);
}

export async function updateMistakeApi(id: string, data: SaveExamMistakeInput) {
  return requestClient.put<ExamMistake>(`${postgraduateBaseUrl}/mistakes/${id}`, data);
}

export async function updateMistakeReviewStatusApi(id: string, status: string) {
  return requestClient.put<ExamMistake>(`${postgraduateBaseUrl}/mistakes/${id}`, { status: status === 'reviewed' ? 1 : (status === 'mastered' ? 2 : 0) });
}

export async function deleteMistakeApi(id: string) {
  return requestClient.delete(`${postgraduateBaseUrl}/mistakes/${id}`);
}

export async function getMaterialPageApi(params: ExamMaterialQuery) {
  return requestClient.get<PageResult<ExamMaterial>>(`${postgraduateBaseUrl}/materials`, { params });
}

export async function getMaterialApi(id: string) {
  return requestClient.get<ExamMaterial>(`${postgraduateBaseUrl}/materials/${id}`);
}

export async function createMaterialApi(data: SaveExamMaterialInput) {
  return requestClient.post<ExamMaterial>(`${postgraduateBaseUrl}/materials`, data);
}

export async function updateMaterialApi(id: string, data: SaveExamMaterialInput) {
  return requestClient.put<ExamMaterial>(`${postgraduateBaseUrl}/materials/${id}`, data);
}

export async function deleteMaterialApi(id: string) {
  return requestClient.delete(`${postgraduateBaseUrl}/materials/${id}`);
}

export interface ExamDashboard {
  todayTasks: PostgraduateTask[];
  weeklyHours: number;
  mistakeCount: number;
  materialCount: number;
  reviewTaskCount: number;
  subjects: Array<{
    color: string;
    id: string;
    name: string;
    progress: number;
    targetHours: number;
    weeklyHours: number;
  }>;
  recentRecords: Array<{
    durationMinutes: number;
    id: string;
    recordDate: string;
    subject: string;
    summary: string;
  }>;
}

export async function getExamDashboardApi() {
  return requestClient.get<ExamDashboard>(`${postgraduateBaseUrl}/dashboard`);
}

import type { BaseEntity, PageQuery, PageResult } from '../growth/types';

import { requestClient } from '#/api/request';

const studentBaseUrl = '/student';

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

export interface StudentSubject extends BaseEntity {
  color: string;
  description?: null | string;
  isActive: boolean;
  name: string;
  sort: number;
  targetHours: number;
}

export interface StudentStudyRecord extends BaseEntity {
  durationMinutes: number;
  recordDate: string;
  remark?: null | string;
  subject: string;
  summary: string;
  taskId?: null | string;
  taskTitle?: null | string;
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

export interface StudentSubjectQuery extends PageQuery {
  isActive?: boolean;
  keyword?: string;
}

export interface StudentStudyRecordQuery extends PageQuery {
  endDate?: string;
  keyword?: string;
  startDate?: string;
  subject?: string;
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
export type SaveStudentSubjectInput = Omit<StudentSubject, 'createdAt' | 'id' | 'updatedAt' | 'userId'>;
export type SaveStudentStudyRecordInput = Omit<StudentStudyRecord, 'createdAt' | 'id' | 'updatedAt' | 'userId'>;

export async function getPostgraduateTaskPageApi(params: PostgraduateTaskQuery) {
  return requestClient.get<PageResult<PostgraduateTask>>(`${studentBaseUrl}/tasks`, { params });
}

export async function getPostgraduateTaskApi(id: string) {
  return requestClient.get<PostgraduateTask>(`${studentBaseUrl}/tasks/${id}`);
}

export async function createPostgraduateTaskApi(data: SavePostgraduateTaskInput) {
  return requestClient.post<PostgraduateTask>(`${studentBaseUrl}/tasks`, data);
}

export async function updatePostgraduateTaskApi(id: string, data: SavePostgraduateTaskInput) {
  return requestClient.put<PostgraduateTask>(`${studentBaseUrl}/tasks/${id}`, data);
}

export async function deletePostgraduateTaskApi(id: string) {
  return requestClient.delete(`${studentBaseUrl}/tasks/${id}`);
}

export async function getMistakePageApi(params: ExamMistakeQuery) {
  return requestClient.get<PageResult<ExamMistake>>(`${studentBaseUrl}/mistakes`, { params });
}

export async function getMistakeApi(id: string) {
  return requestClient.get<ExamMistake>(`${studentBaseUrl}/mistakes/${id}`);
}

export async function createMistakeApi(data: SaveExamMistakeInput) {
  return requestClient.post<ExamMistake>(`${studentBaseUrl}/mistakes`, data);
}

export async function updateMistakeApi(id: string, data: SaveExamMistakeInput) {
  return requestClient.put<ExamMistake>(`${studentBaseUrl}/mistakes/${id}`, data);
}

export async function updateMistakeReviewStatusApi(id: string, status: string) {
  return requestClient.put<ExamMistake>(`${studentBaseUrl}/mistakes/${id}`, { status: status === 'reviewed' ? 1 : (status === 'mastered' ? 2 : 0) });
}

export async function deleteMistakeApi(id: string) {
  return requestClient.delete(`${studentBaseUrl}/mistakes/${id}`);
}

export async function getMaterialPageApi(params: ExamMaterialQuery) {
  return requestClient.get<PageResult<ExamMaterial>>(`${studentBaseUrl}/materials`, { params });
}

export async function getMaterialApi(id: string) {
  return requestClient.get<ExamMaterial>(`${studentBaseUrl}/materials/${id}`);
}

export async function createMaterialApi(data: SaveExamMaterialInput) {
  return requestClient.post<ExamMaterial>(`${studentBaseUrl}/materials`, data);
}

export async function updateMaterialApi(id: string, data: SaveExamMaterialInput) {
  return requestClient.put<ExamMaterial>(`${studentBaseUrl}/materials/${id}`, data);
}

export async function deleteMaterialApi(id: string) {
  return requestClient.delete(`${studentBaseUrl}/materials/${id}`);
}

export interface ExamDashboard {
  todayTasks: PostgraduateTask[];
  weeklyHours: number;
  mistakeCount: number;
  materialCount: number;
  overdueTaskCount: number;
  pendingTaskCount: number;
  reviewTaskCount: number;
  subjectCount: number;
  subjects: Array<{
    color: string;
    id: string;
    name: string;
    materialCount: number;
    mistakeCount: number;
    progress: number;
    targetHours: number;
    weeklyHours: number;
  }>;
  todayReviewCount: number;
  recentRecords: Array<{
    durationMinutes: number;
    id: string;
    recordDate: string;
    remark?: null | string;
    subject: string;
    summary: string;
    taskTitle?: null | string;
  }>;
}

export async function getExamDashboardApi() {
  return requestClient.get<ExamDashboard>(`${studentBaseUrl}/dashboard`);
}

export async function getStudentSubjectPageApi(params: StudentSubjectQuery) {
  return requestClient.get<PageResult<StudentSubject>>(`${studentBaseUrl}/subjects`, { params });
}

export async function createStudentSubjectApi(data: SaveStudentSubjectInput) {
  return requestClient.post<StudentSubject>(`${studentBaseUrl}/subjects`, data);
}

export async function updateStudentSubjectApi(id: string, data: Partial<SaveStudentSubjectInput>) {
  return requestClient.put<StudentSubject>(`${studentBaseUrl}/subjects/${id}`, data);
}

export async function deleteStudentSubjectApi(id: string) {
  return requestClient.delete(`${studentBaseUrl}/subjects/${id}`);
}

export async function getStudyRecordPageApi(params: StudentStudyRecordQuery) {
  return requestClient.get<PageResult<StudentStudyRecord>>(`${studentBaseUrl}/records`, { params });
}

export async function createStudyRecordApi(data: SaveStudentStudyRecordInput) {
  return requestClient.post<StudentStudyRecord>(`${studentBaseUrl}/records`, data);
}

export async function updateStudyRecordApi(id: string, data: Partial<SaveStudentStudyRecordInput>) {
  return requestClient.put<StudentStudyRecord>(`${studentBaseUrl}/records/${id}`, data);
}

export async function deleteStudyRecordApi(id: string) {
  return requestClient.delete(`${studentBaseUrl}/records/${id}`);
}

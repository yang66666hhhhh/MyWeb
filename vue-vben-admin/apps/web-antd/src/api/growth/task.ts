import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface TaskItem {
  id: string;
  userId?: string;
  planDate: string;
  title: string;
  description?: string;
  type: 'Personal' | 'Work';
  source: 'Growth' | 'Work';
  projectId?: string;
  projectName?: string;
  priority: number;
  status: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  convertedWorkLogId?: string;
  remark?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface TaskItemQuery extends PageQuery {
  keyword?: string;
  taskType?: string;
  source?: string;
  planDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  status?: number;
  priority?: number;
}

export interface CreateTaskItemInput {
  planDate: string;
  title: string;
  description?: string;
  taskType?: string;
  source?: string;
  projectId?: string;
  priority?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  remark?: string;
}

export interface UpdateTaskItemInput {
  planDate?: string;
  title?: string;
  description?: string;
  taskType?: string;
  source?: string;
  projectId?: string;
  priority?: number;
  status?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  remark?: string;
}

export const taskApi = {
  getPage: (params?: TaskItemQuery) =>
    requestClient.get<PageResult<TaskItem>>('/growth/tasks', { params }),

  getById: (id: string) =>
    requestClient.get<TaskItem>(`/growth/tasks/${id}`),

  create: (data: CreateTaskItemInput) =>
    requestClient.post<TaskItem>('/growth/tasks', data),

  update: (id: string, data: UpdateTaskItemInput) =>
    requestClient.put<TaskItem>(`/growth/tasks/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/growth/tasks/${id}`),

  complete: (id: string) =>
    requestClient.post(`/growth/tasks/${id}/complete`),

  convertToLog: (data: { originalContent?: string; taskId: string; totalHours?: number; workDate: string; }) =>
    requestClient.post('/growth/tasks/convert-to-log', data),
};

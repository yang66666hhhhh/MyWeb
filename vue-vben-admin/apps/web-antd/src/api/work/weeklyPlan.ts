import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface WeeklyPlan {
  id: string;
  userId: string;
  year: number;
  weekNumber: number;
  weekCode: string;
  startDate: string;
  endDate: string;
  goals: string;
  summary?: string;
  totalHours: number;
  status: number;
  tasks: WeeklyPlanTask[];
  createdAt: string;
  updatedAt?: string;
}

export interface WeeklyPlanTask {
  id: string;
  weeklyPlanId: string;
  title: string;
  description?: string;
  priority: number;
  status: number;
  estimatedHours?: number;
  actualHours?: number;
  createdAt: string;
}

export interface WeeklyPlanQuery extends PageQuery {
  year?: number;
  weekNumber?: number;
  status?: number;
  keyword?: string;
}

export interface CreateWeeklyPlanInput {
  year: number;
  weekNumber: number;
  goals: string;
  status?: number;
}

export interface UpdateWeeklyPlanInput {
  goals?: string;
  summary?: string;
  status?: number;
}

export interface CreateWeeklyPlanTaskInput {
  title: string;
  description?: string;
  priority?: number;
  estimatedHours?: number;
}

export interface UpdateWeeklyPlanTaskInput {
  title?: string;
  description?: string;
  priority?: number;
  status?: number;
  estimatedHours?: number;
  actualHours?: number;
}

export const weeklyPlanApi = {
  getPage: (params: WeeklyPlanQuery) =>
    requestClient.get<PageResult<WeeklyPlan>>('/work/weekly-plans', { params }),

  getById: (id: string) =>
    requestClient.get<WeeklyPlan>(`/work/weekly-plans/${id}`),

  create: (data: CreateWeeklyPlanInput) =>
    requestClient.post<WeeklyPlan>('/work/weekly-plans', data),

  update: (id: string, data: UpdateWeeklyPlanInput) =>
    requestClient.put<WeeklyPlan>(`/work/weekly-plans/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/weekly-plans/${id}`),

  addTask: (id: string, data: CreateWeeklyPlanTaskInput) =>
    requestClient.post<WeeklyPlan>(`/work/weekly-plans/${id}/tasks`, data),

  updateTask: (taskId: string, data: UpdateWeeklyPlanTaskInput) =>
    requestClient.put<WeeklyPlan>(`/work/weekly-plans/tasks/${taskId}`, data),

  deleteTask: (taskId: string) =>
    requestClient.delete(`/work/weekly-plans/tasks/${taskId}`),
};

export const getWeeklyPlanPageApi = weeklyPlanApi.getPage;
export const getWeeklyPlanApi = weeklyPlanApi.getById;
export const createWeeklyPlanApi = weeklyPlanApi.create;
export const updateWeeklyPlanApi = weeklyPlanApi.update;
export const deleteWeeklyPlanApi = weeklyPlanApi.delete;

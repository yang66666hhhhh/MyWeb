import type { BaseEntity, HabitStatus, PageQuery, PageResult } from './types';

import { requestClient } from '#/api/request';

export interface Habit extends BaseEntity {
  userId?: string;
  name: string;
  habitType: string;
  description?: null | string;
  targetFrequency: string;
  status: HabitStatus;
  currentStreak: number;
  longestStreak: number;
  totalCheckIns: number;
  lastCheckInDate?: null | string;
}

export interface HabitDetail extends Habit {
  recentCheckIns: Array<{
    id: string;
    checkInDate: string;
    remark?: null | string;
  }>;
}

export interface HabitQuery extends PageQuery {
  habitType?: string;
  keyword?: string;
  status?: HabitStatus;
}

export interface SaveHabitInput {
  description?: null | string;
  habitType: string;
  name: string;
  status?: HabitStatus;
  targetFrequency?: string;
}

export async function getHabitPageApi(params: HabitQuery) {
  return requestClient.get<PageResult<Habit>>('/growth/habits', { params });
}

export async function getHabitApi(id: string) {
  return requestClient.get<HabitDetail>(`/growth/habits/${id}`);
}

export async function createHabitApi(data: SaveHabitInput) {
  return requestClient.post<Habit>('/growth/habits', data);
}

export async function updateHabitApi(id: string, data: SaveHabitInput) {
  return requestClient.put<Habit>(`/growth/habits/${id}`, data);
}

export async function checkInHabitApi(id: string, remark?: string) {
  return requestClient.post<Habit>(`/growth/habits/${id}/check-in`, { remark });
}

export async function updateHabitStatusApi(id: string, status: HabitStatus) {
  return requestClient.put<Habit>(`/growth/habits/${id}/status`, { status });
}

export async function deleteHabitApi(id: string) {
  return requestClient.delete(`/growth/habits/${id}`);
}
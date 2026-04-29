import { requestClient } from '#/api/request';

import type {
  WorkDailyPlan,
  WorkDevice,
  WorkImportBatch,
  WorkImportPreviewItem,
  WorkLog,
  WorkProject,
  WorkTaskType,
} from '../../work/types';

import type { PageResult } from '../types';

export type { WorkDailyPlan, WorkDevice, WorkImportBatch, WorkImportPreviewItem, WorkLog, WorkProject, WorkTaskType };

export { WorkLogStatus } from '../../../enums/workEnum';
export type { WorkLogStatus as WorkLogStatusType };

export const getWorkDevicePageApi = (params?: any) =>
  requestClient.get<PageResult<WorkDevice>>('/work/devices', { params });

export const getWorkDeviceApi = (id: string) =>
  requestClient.get<WorkDevice>(`/work/devices/${id}`);

export const createWorkDeviceApi = (data: Partial<WorkDevice>) =>
  requestClient.post<WorkDevice>('/work/devices', data);

export const updateWorkDeviceApi = (id: string, data: Partial<WorkDevice>) =>
  requestClient.put<WorkDevice>(`/work/devices/${id}`, data);

export const deleteWorkDeviceApi = (id: string) =>
  requestClient.delete(`/work/devices/${id}`);

export const getWorkProjectPageApi = (params?: any) =>
  requestClient.get<PageResult<WorkProject>>('/work/projects', { params });

export const getWorkProjectApi = (id: string) =>
  requestClient.get<WorkProject>(`/work/projects/${id}`);

export const createWorkProjectApi = (data: Partial<WorkProject>) =>
  requestClient.post<WorkProject>('/work/projects', data);

export const updateWorkProjectApi = (id: string, data: Partial<WorkProject>) =>
  requestClient.put<WorkProject>(`/work/projects/${id}`, data);

export const deleteWorkProjectApi = (id: string) =>
  requestClient.delete(`/work/projects/${id}`);

export const getWorkTaskTypePageApi = (params?: any) =>
  requestClient.get<PageResult<WorkTaskType>>('/work/task-types', { params });

export const getWorkTaskTypeApi = (id: string) =>
  requestClient.get<WorkTaskType>(`/work/task-types/${id}`);

export const createWorkTaskTypeApi = (data: Partial<WorkTaskType>) =>
  requestClient.post<WorkTaskType>('/work/task-types', data);

export const updateWorkTaskTypeApi = (id: string, data: Partial<WorkTaskType>) =>
  requestClient.put<WorkTaskType>(`/work/task-types/${id}`, data);

export const deleteWorkTaskTypeApi = (id: string) =>
  requestClient.delete(`/work/task-types/${id}`);

export const getWorkLogPageApi = (params?: any) =>
  requestClient.get<PageResult<WorkLog>>('/work/logs', { params });

export const getWorkLogApi = (id: string) =>
  requestClient.get<WorkLog>(`/work/logs/${id}`);

export const createWorkLogApi = (data: Partial<WorkLog>) =>
  requestClient.post<WorkLog>('/work/logs', data);

export const updateWorkLogApi = (id: string, data: Partial<WorkLog>) =>
  requestClient.put<WorkLog>(`/work/logs/${id}`, data);

export const deleteWorkLogApi = (id: string) =>
  requestClient.delete(`/work/logs/${id}`);

export const getWorkDailyPlanPageApi = (params?: any) =>
  requestClient.get<PageResult<WorkDailyPlan>>('/work/daily-plans', { params });

export const getWorkDailyPlanApi = (id: string) =>
  requestClient.get<WorkDailyPlan>(`/work/daily-plans/${id}`);

export const createWorkDailyPlanApi = (data: Partial<WorkDailyPlan>) =>
  requestClient.post<WorkDailyPlan>('/work/daily-plans', data);

export const updateWorkDailyPlanApi = (id: string, data: Partial<WorkDailyPlan>) =>
  requestClient.put<WorkDailyPlan>(`/work/daily-plans/${id}`, data);

export const deleteWorkDailyPlanApi = (id: string) =>
  requestClient.delete(`/work/daily-plans/${id}`);

export const completeWorkDailyPlanApi = (id: string) =>
  requestClient.post<WorkDailyPlan>(`/work/daily-plans/${id}/complete`);

export interface WorkStatisticsOverview {
  totalLogs: number;
  totalHours: number;
  totalProjects: number;
  totalDevices: number;
  todayLogs: number;
  todayHours: number;
  missingDataCount: number;
  pendingSupplementCount: number;
}

export const getWorkStatisticsOverviewApi = () =>
  requestClient.get<WorkStatisticsOverview>('/work/statistics/overview');

export interface WorkStatisticsDailyHours {
  date: string;
  hours: number;
}

export const getWorkStatisticsDailyHoursApi = (params?: any) =>
  requestClient.get<WorkStatisticsDailyHours[]>('/work/statistics/daily-hours', { params });

export interface WorkStatisticsProjectHours {
  projectId: string;
  projectName: string;
  hours: number;
}

export const getWorkStatisticsProjectHoursApi = (params?: any) =>
  requestClient.get<WorkStatisticsProjectHours[]>('/work/statistics/project-hours', { params });

export interface WorkStatisticsTaskTypeDistribution {
  taskTypeId: string;
  taskTypeName: string;
  count: number;
}

export const getWorkStatisticsTaskTypeDistributionApi = (params?: any) =>
  requestClient.get<WorkStatisticsTaskTypeDistribution[]>('/work/statistics/task-type-distribution', { params });

export interface WorkStatisticsDeviceRanking {
  deviceId: string;
  deviceName: string;
  hours: number;
}

export const getWorkStatisticsDeviceRankingApi = (params?: any) =>
  requestClient.get<WorkStatisticsDeviceRanking[]>('/work/statistics/device-ranking', { params });

export const getWorkImportBatchPageApi = (params?: any) =>
  requestClient.get<PageResult<WorkImportBatch>>('/work/import', { params });

export const previewWorkImportApi = (data: any) =>
  requestClient.post<WorkImportPreviewItem[]>('/work/import/preview', data);

export const confirmWorkImportApi = (id: string) =>
  requestClient.post<WorkImportBatch>(`/work/import/${id}/execute`);

export const getWorkImportTemplateUrl = () => '/api/work/import/template';

export type CreateWorkLogInput = Partial<WorkLog>;

export async function convertToWorkLogApi(data: {
  originalContent?: null | string;
  planId: string;
  workDate: string;
}) {
  const result = await requestClient.post<{ workLogId: string }>('/work/daily-plans/convert-to-log', data);
  return result;
}
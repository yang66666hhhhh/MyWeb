import type { PageQuery } from '#/types/api';
import { requestClient } from '#/api/request';

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

export interface WorkStatisticsDailyHours {
  date: string;
  hours: number;
  logCount: number;
}

export interface WorkStatisticsProjectHours {
  projectId: string;
  projectName: string;
  totalHours: number;
  logCount: number;
  percentage: number;
}

export interface WorkStatisticsTaskTypeDistribution {
  taskTypeId: string;
  taskTypeName: string;
  totalHours: number;
  logCount: number;
  percentage: number;
}

export interface WorkStatisticsDeviceRanking {
  deviceId: string;
  deviceName: string;
  totalHours: number;
  logCount: number;
  ranking: number;
}

export interface WorkStatisticsQuery extends PageQuery {
  startDate?: string;
  endDate?: string;
  projectId?: string;
}

export const statisticsApi = {
  getOverview: (params?: WorkStatisticsQuery) =>
    requestClient.get<WorkStatisticsOverview>('/work/statistics/overview', { params }),

  getDailyHours: (params?: WorkStatisticsQuery) =>
    requestClient.get<WorkStatisticsDailyHours[]>('/work/statistics/daily-hours', { params }),

  getProjectHours: (params?: WorkStatisticsQuery) =>
    requestClient.get<WorkStatisticsProjectHours[]>('/work/statistics/project-hours', { params }),

  getTaskTypeDistribution: (params?: WorkStatisticsQuery) =>
    requestClient.get<WorkStatisticsTaskTypeDistribution[]>('/work/statistics/task-type-distribution', { params }),

  getDeviceRanking: (params?: WorkStatisticsQuery) =>
    requestClient.get<WorkStatisticsDeviceRanking[]>('/work/statistics/device-ranking', { params }),
};

export const getWorkStatisticsOverviewApi = statisticsApi.getOverview;
export const getWorkStatisticsDailyHoursApi = statisticsApi.getDailyHours;
export const getWorkStatisticsProjectHoursApi = statisticsApi.getProjectHours;
export const getWorkStatisticsTaskTypeDistributionApi = statisticsApi.getTaskTypeDistribution;
export const getWorkStatisticsDeviceRankingApi = statisticsApi.getDeviceRanking;
export const getOverviewApi = statisticsApi.getOverview;

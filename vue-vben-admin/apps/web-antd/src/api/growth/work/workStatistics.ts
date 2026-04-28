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

export interface WorkStatisticsQuery {
  startDate?: string;
  endDate?: string;
  projectId?: string;
}

export async function getWorkStatisticsOverviewApi(params?: WorkStatisticsQuery) {
  return requestClient.get<WorkStatisticsOverview>('/work/statistics/overview', { params });
}

export async function getWorkStatisticsDailyHoursApi(params?: WorkStatisticsQuery) {
  return requestClient.get<WorkStatisticsDailyHours[]>('/work/statistics/daily-hours', { params });
}

export async function getWorkStatisticsProjectHoursApi(params?: WorkStatisticsQuery) {
  return requestClient.get<WorkStatisticsProjectHours[]>('/work/statistics/project-hours', { params });
}

export async function getWorkStatisticsTaskTypeDistributionApi(params?: WorkStatisticsQuery) {
  return requestClient.get<WorkStatisticsTaskTypeDistribution[]>(
    '/work/statistics/task-type-distribution',
    { params },
  );
}

export async function getWorkStatisticsDeviceRankingApi(params?: WorkStatisticsQuery) {
  return requestClient.get<WorkStatisticsDeviceRanking[]>('/work/statistics/device-ranking', { params });
}

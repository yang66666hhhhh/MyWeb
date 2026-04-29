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

export const statisticsApi = {
  getOverview: () => requestClient.get<WorkStatisticsOverview>('/work/statistics/overview'),
  getDailyHours: (params?: any) => requestClient.get('/work/statistics/daily-hours', { params }),
  getProjectHours: (params?: any) => requestClient.get('/work/statistics/project-hours', { params }),
  getTaskTypeDistribution: (params?: any) => requestClient.get('/work/statistics/task-type-distribution', { params }),
  getDeviceRanking: (params?: any) => requestClient.get('/work/statistics/device-ranking', { params }),
};

export const getOverviewApi = statisticsApi.getOverview;

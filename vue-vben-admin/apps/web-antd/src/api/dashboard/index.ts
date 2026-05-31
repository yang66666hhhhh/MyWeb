import { requestClient } from '#/api/request';

export interface DashboardOverview {
  todayTasks: number;
  todayCompletedTasks: number;
  todayWorkHours: number;
  weekTasks: number;
  weekCompletedTasks: number;
  weekWorkHours: number;
  totalProjects: number;
  totalHabits: number;
  totalKnowledge: number;
}

export interface RecentTask {
  id: string;
  title: string;
  status: number;
  priority: number;
  planDate: string;
  projectName?: string;
}

export interface RecentWorkLog {
  id: string;
  title: string;
  workDate: string;
  totalHours: number;
  projectName?: string;
}

export interface TodayPlan {
  id: string;
  title: string;
  status: number;
  priority: number;
  planDate: string;
}

export const dashboardApi = {
  getOverview: () =>
    requestClient.get<DashboardOverview>('/growth/analytics/dashboard'),

  getRecentTasks: (params?: { pageSize?: number }) =>
    requestClient.get<{ items: RecentTask[]; total: number }>('/growth/tasks', { params: { ...params, page: 1, source: 'Work' } }),

  getRecentWorkLogs: (params?: { pageSize?: number }) =>
    requestClient.get<{ items: RecentWorkLog[]; total: number }>('/work/logs', { params: { ...params, page: 1 } }),

  getTodayPlans: (params?: { pageSize?: number }) =>
    requestClient.get<{ items: TodayPlan[]; total: number }>('/growth/tasks', { params: { ...params, page: 1, source: 'Personal' } }),
};

export const getDashboardOverviewApi = dashboardApi.getOverview;
export const getRecentTasksApi = dashboardApi.getRecentTasks;
export const getRecentWorkLogsApi = dashboardApi.getRecentWorkLogs;
export const getTodayPlansApi = dashboardApi.getTodayPlans;

import { requestClient } from '#/api/request';

export interface DashboardOverview {
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  completionRate: number;
  totalWorkLogs: number;
  totalWorkHours: number;
  todayTasks: number;
  todayCompletedTasks: number;
  todayWorkHours: number;
}

export interface TaskTrend {
  date: string;
  created: number;
  completed: number;
  completionRate: number;
}

export interface TaskDistribution {
  type: string;
  count: number;
  percentage: number;
}

export interface WorkVsGrowth {
  category: string;
  taskCount: number;
  hours: number;
}

export interface TaskPriorityDistribution {
  priority: string;
  count: number;
  percentage: number;
}

export const analyticsApi = {
  getDashboard: () =>
    requestClient.get<DashboardOverview>('/growth/analytics/dashboard'),

  getTaskTrends: (startDate: string, endDate: string) =>
    requestClient.get<TaskTrend[]>('/growth/analytics/task-trends', { params: { startDate, endDate } }),

  getTaskDistribution: () =>
    requestClient.get<TaskDistribution[]>('/growth/analytics/task-distribution'),

  getWorkVsGrowth: () =>
    requestClient.get<WorkVsGrowth[]>('/growth/analytics/work-vs-growth'),

  getPriorityDistribution: () =>
    requestClient.get<TaskPriorityDistribution[]>('/growth/analytics/priority-distribution'),
};

export * from './extended';

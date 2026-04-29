import { requestClient } from '#/api/request';

export const workspaceApi = {
  getOverview: () => requestClient.get('/dashboard/workspace/overview'),
  getRecentActivities: () => requestClient.get('/dashboard/workspace/activities'),
  getQuickStats: () => requestClient.get('/dashboard/workspace/quick-stats'),
};

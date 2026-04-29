import { requestClient } from '#/api/request';

export const dashboardApi = {
  getOverview: () => requestClient.get('/dashboard/overview'),
  getStats: () => requestClient.get('/dashboard/stats'),
};

export const statisticsApi = {
  getStatistics: () => requestClient.get('/statistics'),
};

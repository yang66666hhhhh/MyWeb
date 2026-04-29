import { requestClient } from '#/api/request';

export const growthAnalyticsApi = {
  getAnalytics: (params?: any) => requestClient.get('/analytics/growth', { params }),
};

export const workAnalyticsApi = {
  getAnalytics: (params?: any) => requestClient.get('/analytics/work', { params }),
};

export const timeAnalyticsApi = {
  getAnalytics: (params?: any) => requestClient.get('/analytics/time', { params }),
};

export const financeAnalyticsApi = {
  getAnalytics: (params?: any) => requestClient.get('/analytics/finance', { params }),
};

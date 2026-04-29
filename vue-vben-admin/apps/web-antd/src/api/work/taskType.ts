import { requestClient } from '#/api/request';

export const taskTypeApi = {
  getTaskTypes: (params?: any) => requestClient.get('/work/task-types', { params }),
  getTaskTypeById: (id: string) => requestClient.get(`/work/task-types/${id}`),
  createTaskType: (data: any) => requestClient.post('/work/task-types', data),
  updateTaskType: (id: string, data: any) => requestClient.put(`/work/task-types/${id}`, data),
  deleteTaskType: (id: string) => requestClient.delete(`/work/task-types/${id}`),
};

export const workLogApi = {
  getLogs: (params?: any) => requestClient.get('/work/logs', { params }),
  getLogById: (id: string) => requestClient.get(`/work/logs/${id}`),
  createLog: (data: any) => requestClient.post('/work/logs', data),
  updateLog: (id: string, data: any) => requestClient.put(`/work/logs/${id}`, data),
  deleteLog: (id: string) => requestClient.delete(`/work/logs/${id}`),
};

export const dailyPlanApi = {
  getPlans: (params?: any) => requestClient.get('/work/daily-plans', { params }),
  getPlanById: (id: string) => requestClient.get(`/work/daily-plans/${id}`),
  createPlan: (data: any) => requestClient.post('/work/daily-plans', data),
  updatePlan: (id: string, data: any) => requestClient.put(`/work/daily-plans/${id}`, data),
  deletePlan: (id: string) => requestClient.delete(`/work/daily-plans/${id}`),
  completePlan: (id: string) => requestClient.post(`/work/daily-plans/${id}/complete`),
};

export const statisticsApi = {
  getOverview: () => requestClient.get('/work/statistics/overview'),
  getDailyHours: (params?: any) => requestClient.get('/work/statistics/daily-hours', { params }),
  getProjectHours: (params?: any) => requestClient.get('/work/statistics/project-hours', { params }),
  getTaskTypeDistribution: (params?: any) => requestClient.get('/work/statistics/task-type-distribution', { params }),
  getDeviceRanking: (params?: any) => requestClient.get('/work/statistics/device-ranking', { params }),
};

export const workImportApi = {
  getImportList: (params?: any) => requestClient.get('/work/import', { params }),
  createImport: (data: any) => requestClient.post('/work/import', data),
  previewImport: (data: any) => requestClient.post('/work/import/preview', data),
  executeImport: (id: string) => requestClient.post(`/work/import/${id}/execute`),
};

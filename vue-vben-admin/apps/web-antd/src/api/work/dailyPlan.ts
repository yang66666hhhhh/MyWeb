import { requestClient } from '#/api/request';

export const dailyPlanApi = {
  getPlans: (params?: any) => requestClient.get('/work/daily-plans', { params }),
  getPlanById: (id: string) => requestClient.get(`/work/daily-plans/${id}`),
  createPlan: (data: any) => requestClient.post('/work/daily-plans', data),
  updatePlan: (id: string, data: any) => requestClient.put(`/work/daily-plans/${id}`, data),
  deletePlan: (id: string) => requestClient.delete(`/work/daily-plans/${id}`),
  completePlan: (id: string) => requestClient.post(`/work/daily-plans/${id}/complete`),
  convertToLog: (id: string) => requestClient.post('/work/daily-plans/convert-to-log', { id }),
};

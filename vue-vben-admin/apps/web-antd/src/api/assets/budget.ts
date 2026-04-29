import { requestClient } from '#/api/request';

export const budgetApi = {
  getList: (params?: any) => requestClient.get('/assets/budgets', { params }),
  getById: (id: string) => requestClient.get(`/assets/budgets/${id}`),
  create: (data: any) => requestClient.post('/assets/budgets', data),
  update: (id: string, data: any) => requestClient.put(`/assets/budgets/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/budgets/${id}`),
};

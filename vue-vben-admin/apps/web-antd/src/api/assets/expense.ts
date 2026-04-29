import { requestClient } from '#/api/request';

export const expenseApi = {
  getList: (params?: any) => requestClient.get('/assets/expenses', { params }),
  getById: (id: string) => requestClient.get(`/assets/expenses/${id}`),
  create: (data: any) => requestClient.post('/assets/expenses', data),
  update: (id: string, data: any) => requestClient.put(`/assets/expenses/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/expenses/${id}`),
};

import { requestClient } from '#/api/request';

export const incomeApi = {
  getList: (params: any) => requestClient.get('/assets/incomes', { params }),
  getById: (id: string) => requestClient.get(`/assets/incomes/${id}`),
  create: (data: any) => requestClient.post('/assets/incomes', data),
  update: (id: string, data: any) => requestClient.put(`/assets/incomes/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/incomes/${id}`),
};

export const expenseApi = {
  getList: (params: any) => requestClient.get('/assets/expenses', { params }),
  getById: (id: string) => requestClient.get(`/assets/expenses/${id}`),
  create: (data: any) => requestClient.post('/assets/expenses', data),
  update: (id: string, data: any) => requestClient.put(`/assets/expenses/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/expenses/${id}`),
};

export const budgetApi = {
  getList: (params: any) => requestClient.get('/assets/budgets', { params }),
  getById: (id: string) => requestClient.get(`/assets/budgets/${id}`),
  create: (data: any) => requestClient.post('/assets/budgets', data),
  update: (id: string, data: any) => requestClient.put(`/assets/budgets/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/budgets/${id}`),
};

export const investmentApi = {
  getList: (params: any) => requestClient.get('/assets/investments', { params }),
  getById: (id: string) => requestClient.get(`/assets/investments/${id}`),
  create: (data: any) => requestClient.post('/assets/investments', data),
  update: (id: string, data: any) => requestClient.put(`/assets/investments/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/investments/${id}`),
};

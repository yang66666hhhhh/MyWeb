import { requestClient } from '#/api/request';

export const investmentApi = {
  getList: (params?: any) => requestClient.get('/assets/investments', { params }),
  getById: (id: string) => requestClient.get(`/assets/investments/${id}`),
  create: (data: any) => requestClient.post('/assets/investments', data),
  update: (id: string, data: any) => requestClient.put(`/assets/investments/${id}`, data),
  delete: (id: string) => requestClient.delete(`/assets/investments/${id}`),
};

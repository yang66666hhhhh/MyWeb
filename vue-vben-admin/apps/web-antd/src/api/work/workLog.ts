import { requestClient } from '#/api/request';

export const workLogApi = {
  getLogs: (params?: any) => requestClient.get('/work/logs', { params }),
  getLogById: (id: string) => requestClient.get(`/work/logs/${id}`),
  createLog: (data: any) => requestClient.post('/work/logs', data),
  updateLog: (id: string, data: any) => requestClient.put(`/work/logs/${id}`, data),
  deleteLog: (id: string) => requestClient.delete(`/work/logs/${id}`),
};

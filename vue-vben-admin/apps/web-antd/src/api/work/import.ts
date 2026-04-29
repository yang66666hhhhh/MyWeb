import { requestClient } from '#/api/request';

export const workImportApi = {
  getList: (params?: any) => requestClient.get('/work/import', { params }),
  create: (data: any) => requestClient.post('/work/import', data),
  preview: (data: any) => requestClient.post('/work/import/preview', data),
  execute: (id: string) => requestClient.post(`/work/import/${id}/execute`),
};

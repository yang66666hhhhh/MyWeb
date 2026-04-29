import { requestClient } from '#/api/request';

export const systemApi = {
  getSettings: () => requestClient.get('/system/settings'),
  updateSettings: (data: any) => requestClient.put('/system/settings', data),
};

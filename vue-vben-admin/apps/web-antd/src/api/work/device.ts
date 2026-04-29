import { requestClient } from '#/api/request';

export const deviceApi = {
  getDevices: (params?: any) => requestClient.get('/work/devices', { params }),
  getDeviceById: (id: string) => requestClient.get(`/work/devices/${id}`),
  createDevice: (data: any) => requestClient.post('/work/devices', data),
  updateDevice: (id: string, data: any) => requestClient.put(`/work/devices/${id}`, data),
  deleteDevice: (id: string) => requestClient.delete(`/work/devices/${id}`),
};

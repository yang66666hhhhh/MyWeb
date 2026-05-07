import { requestClient } from '#/api/request';

export interface ImplLog {
  id: string;
  userId: string;
  workDate: string;
  weekDay: string;
  title: string;
  projectName?: string;
  totalHours: number;
  personaCode?: string;
  templateId?: string;
  templateName?: string;
  extraData?: Record<string, any>;
  createdAt: string;
  updatedAt?: string;
}

export interface ImplLogQuery {
  workDate?: string;
  startDate?: string;
  endDate?: string;
  keyword?: string;
  templateId?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateImplLogRequest {
  workDate: string;
  title: string;
  projectName?: string;
  totalHours: number;
  templateId?: string;
  extraData?: Record<string, any>;
}

export interface UpdateImplLogRequest {
  workDate?: string;
  title?: string;
  projectName?: string;
  totalHours?: number;
  templateId?: string;
  extraData?: Record<string, any>;
}

export function getImplLogsApi(query: ImplLogQuery) {
  return requestClient.get<{ items: ImplLog[]; total: number }>('/work/impl-logs', { params: query });
}

export function getImplLogByIdApi(id: string) {
  return requestClient.get<ImplLog>(`/work/impl-logs/${id}`);
}

export function createImplLogApi(data: CreateImplLogRequest) {
  return requestClient.post<ImplLog>('/work/impl-logs', data);
}

export function updateImplLogApi(id: string, data: UpdateImplLogRequest) {
  return requestClient.put<ImplLog>(`/work/impl-logs/${id}`, data);
}

export function deleteImplLogApi(id: string) {
  return requestClient.delete(`/work/impl-logs/${id}`);
}

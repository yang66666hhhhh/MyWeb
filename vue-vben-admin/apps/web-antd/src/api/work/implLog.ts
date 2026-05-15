import { requestClient } from '#/api/request';
export type { WorkLogTemplate } from './logTemplate';
export { getMyWorkLogTemplateApi } from './logTemplate';

export interface ImplLogExtraData {
  equipment?: string[];
  location?: string;
  taskTypes?: string[];
  [key: string]: unknown;
}

export interface ImplLog {
  id: string;
  userId: string;
  workDate: string;
  weekDay: string;
  title: string;
  projectId?: string;
  projectName?: string;
  totalHours: number;
  personaCode?: string;
  templateId?: string;
  templateName?: string;
  extraData?: ImplLogExtraData;
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

export interface ImplLogSummary {
  totalCount: number;
  totalHours: number;
  uniqueEquipmentCount: number;
  uniqueTaskTypeCount: number;
}

export interface CreateImplLogRequest {
  workDate: string;
  title: string;
  projectId?: string;
  projectName?: string;
  totalHours: number;
  templateId?: string;
  extraData?: ImplLogExtraData;
}

export interface UpdateImplLogRequest {
  workDate?: string;
  title?: string;
  projectId?: string;
  projectName?: string;
  totalHours?: number;
  templateId?: string;
  extraData?: ImplLogExtraData;
}

export function getImplLogsApi(query: ImplLogQuery) {
  return requestClient.get<{ items: ImplLog[]; total: number }>('/work/impl-logs', { params: query });
}

export function getImplLogSummaryApi(query: ImplLogQuery) {
  return requestClient.get<ImplLogSummary>('/work/impl-logs/summary', { params: query });
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

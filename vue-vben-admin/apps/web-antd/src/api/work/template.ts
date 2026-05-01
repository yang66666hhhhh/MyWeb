import { requestClient } from '#/api/request';

export interface IndustryTemplate {
  id: string;
  name: string;
  description?: string;
  industry: string;
  isDefault: boolean;
  fields: TemplateField[];
  createdAt: string;
}

export interface TemplateField {
  id: string;
  templateId: string;
  fieldName: string;
  fieldLabel: string;
  fieldType: FieldType;
  options?: string;
  isRequired: boolean;
  sort: number;
  defaultValue?: string;
}

export type FieldType = 0 | 1 | 2 | 3 | 4 | 5 | 6;

export const FieldTypeLabels: Record<FieldType, string> = {
  0: '文本',
  1: '数字',
  2: '日期',
  3: '下拉单选',
  4: '下拉多选',
  5: '多行文本',
  6: '文件',
};

export interface CreateTemplateInput {
  name: string;
  description?: string;
  industry: string;
  isDefault?: boolean;
  fields: CreateTemplateFieldInput[];
}

export interface CreateTemplateFieldInput {
  fieldName: string;
  fieldLabel: string;
  fieldType: FieldType;
  options?: string;
  isRequired?: boolean;
  sort: number;
  defaultValue?: string;
}

export interface PageResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface PageQuery {
  page?: number;
  pageSize?: number;
}

export async function getTemplatePageApi(params?: PageQuery) {
  return requestClient.get<PageResult<IndustryTemplate>>('/templates', { params });
}

export async function getTemplateApi(id: string) {
  return requestClient.get<IndustryTemplate>(`/templates/${id}`);
}

export async function getTemplateFieldsApi(id: string) {
  return requestClient.get<TemplateField[]>(`/templates/${id}/fields`);
}

export async function createTemplateApi(data: CreateTemplateInput) {
  return requestClient.post<IndustryTemplate>('/templates', data);
}

export async function updateTemplateApi(id: string, data: CreateTemplateInput) {
  return requestClient.put<IndustryTemplate>(`/templates/${id}`, data);
}

export async function deleteTemplateApi(id: string) {
  return requestClient.delete(`/templates/${id}`);
}

export async function setDefaultTemplateApi(id: string) {
  return requestClient.post(`/templates/${id}/set-default`);
}
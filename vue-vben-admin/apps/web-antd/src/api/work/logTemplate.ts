import { requestClient } from '#/api/request';

export interface WorkLogTemplate {
  id: string;
  personaCode: string;
  name: string;
  description?: string;
  fieldDefinitions: {
    fields: FieldDefinition[];
  };
  isActive: boolean;
  sort: number;
}

export interface FieldDefinition {
  key: string;
  label: string;
  type: 'date' | 'multi-select' | 'number' | 'select' | 'task-list' | 'text' | 'textarea';
  options?: string[];
  required?: boolean;
  placeholder?: string;
  fields?: string[];
}

export function getWorkLogTemplatesApi() {
  return requestClient.get<WorkLogTemplate[]>('/work/log-templates');
}

export function getWorkLogTemplateByPersonaApi(personaCode: string) {
  return requestClient.get<WorkLogTemplate>(`/work/log-templates/${personaCode}`);
}

export function getMyWorkLogTemplateApi() {
  return requestClient.get<WorkLogTemplate>('/work/log-templates/my');
}

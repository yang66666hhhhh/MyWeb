import { requestClient } from '#/api/request';

export interface PersonaType {
  id: string;
  code: string;
  name: string;
  icon: string;
  description?: string;
  defaultHomeRoute?: string;
  sort: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreatePersonaTypeDto {
  code: string;
  name: string;
  icon: string;
  description?: string;
  defaultHomeRoute?: string;
  sort: number;
}

export interface UpdatePersonaTypeDto {
  name: string;
  icon: string;
  description?: string;
  defaultHomeRoute?: string;
  sort: number;
  isActive: boolean;
}

export const personaApi = {
  list(params?: { isActive?: boolean }) {
    return requestClient.get<PersonaType[]>('/admin/persona-types/all', { params });
  },
  getPage(params: { page?: number; pageSize?: number; keyword?: string; isActive?: boolean }) {
    return requestClient.get<{ items: PersonaType[]; total: number }>('/admin/persona-types', { params });
  },
  getById(id: string) {
    return requestClient.get<PersonaType>(`/admin/persona-types/${id}`);
  },
  create(data: CreatePersonaTypeDto) {
    return requestClient.post<PersonaType>('/admin/persona-types', data);
  },
  update(id: string, data: UpdatePersonaTypeDto) {
    return requestClient.put<PersonaType>(`/admin/persona-types/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/admin/persona-types/${id}`);
  },
};

export const currentPersonaApi = {
  getCurrent() {
    return requestClient.get<PersonaType>('/user/persona/current');
  },
  getAvailable() {
    return requestClient.get<PersonaType[]>('/user/persona/available');
  },
  switch(personaTypeId: string) {
    return requestClient.put('/user/persona/switch', { personaTypeId });
  },
};

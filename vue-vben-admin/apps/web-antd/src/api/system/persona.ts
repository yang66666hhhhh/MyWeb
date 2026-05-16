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
  getAll(params?: { isActive?: boolean }) {
    return requestClient.get<PersonaType[]>('/system/persona-types/all', { params });
  },
  list(params?: { isActive?: boolean }) {
    return requestClient.get<PersonaType[]>('/system/persona-types/all', { params });
  },
  getPage(params: { isActive?: boolean; keyword?: string; page?: number; pageSize?: number; }) {
    return requestClient.get<{ items: PersonaType[]; total: number }>('/system/persona-types', { params });
  },
  getById(id: string) {
    return requestClient.get<PersonaType>(`/system/persona-types/${id}`);
  },
  create(data: CreatePersonaTypeDto) {
    return requestClient.post<PersonaType>('/system/persona-types', data);
  },
  update(id: string, data: UpdatePersonaTypeDto) {
    return requestClient.put<PersonaType>(`/system/persona-types/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/system/persona-types/${id}`);
  },
};

export const currentPersonaApi = {
  getCurrent() {
    return requestClient.get<PersonaType>('/account/persona/current');
  },
  getAvailable() {
    return requestClient.get<PersonaType[]>('/account/persona/available');
  },
  switch(personaTypeId: string) {
    return requestClient.put(`/account/persona/set-primary/${personaTypeId}`);
  },
};

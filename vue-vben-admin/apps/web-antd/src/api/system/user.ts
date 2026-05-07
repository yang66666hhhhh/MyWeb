import { requestClient } from '#/api/request';

export interface PersonaTypeInfo {
  id: string;
  code: string;
  name: string;
  icon: string;
  isPrimary: boolean;
}

export interface UserDto {
  id: string;
  username: string;
  realName: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles?: string;
  status: number;
  personas: PersonaTypeInfo[];
  lastLoginAt?: string;
  createdAt: string;
}

export interface CreateUserDto {
  username: string;
  password: string;
  realName: string;
  email?: string;
  phone?: string;
  roles?: string;
}

export interface UpdateUserDto {
  realName: string;
  email?: string;
  phone?: string;
  roles?: string;
  status?: number;
}

export function getUserPageApi(params: {
  page?: number;
  pageSize?: number;
  keyword?: string;
  status?: number;
}) {
  return requestClient.get<{ items: UserDto[]; total: number }>('/users', { params });
}

export function getUserApi(id: string) {
  return requestClient.get<UserDto>(`/users/${id}`);
}

export function createUserApi(data: CreateUserDto) {
  return requestClient.post<UserDto>('/users', data);
}

export function updateUserApi(id: string, data: UpdateUserDto) {
  return requestClient.put<UserDto>(`/users/${id}`, data);
}

export function deleteUserApi(id: string) {
  return requestClient.delete(`/users/${id}`);
}

export function resetPasswordApi(id: string, data: { newPassword: string }) {
  return requestClient.post(`/users/${id}/reset-password`, data);
}

export function changePasswordApi(id: string, data: { oldPassword: string; newPassword: string }) {
  return requestClient.post(`/users/${id}/change-password`, data);
}

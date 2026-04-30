import { requestClient } from '#/api/request';

export interface UserDto {
  id: string;
  username: string;
  realName: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles: string;
  status: number;
  lastLoginAt?: string;
  lastLoginIp?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface UserQueryDto {
  keyword?: string;
  status?: number;
  page?: number;
  pageSize?: number;
}

export interface CreateUserDto {
  username: string;
  password: string;
  realName: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles?: string;
}

export interface UpdateUserDto {
  realName?: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles?: string;
  status?: number;
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
}

export interface ResetPasswordDto {
  newPassword: string;
}

export interface PageResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const getUserPageApi = (params?: UserQueryDto) =>
  requestClient.get<PageResult<UserDto>>('/users', { params });

export const getUserApi = (id: string) =>
  requestClient.get<UserDto>(`/users/${id}`);

export const createUserApi = (data: CreateUserDto) =>
  requestClient.post<UserDto>('/users', data);

export const updateUserApi = (id: string, data: UpdateUserDto) =>
  requestClient.put<UserDto>(`/users/${id}`, data);

export const deleteUserApi = (id: string) =>
  requestClient.delete(`/users/${id}`);

export const changePasswordApi = (id: string, data: ChangePasswordDto) =>
  requestClient.post(`/users/${id}/change-password`, data);

export const resetPasswordApi = (id: string, data: ResetPasswordDto) =>
  requestClient.post(`/users/${id}/reset-password`, data);
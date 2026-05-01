import { requestClient } from '#/api/request';

export interface TagDto {
  id: string;
  name: string;
  description?: string;
  color: string;
  sort: number;
}

export interface CreateTagDto {
  name: string;
  description?: string;
  color?: string;
  sort: number;
}

export interface MenuTreeAdminDto {
  id: string;
  name: string;
  path: string;
  icon?: string;
  sort: number;
  isActive: boolean;
  parentId?: string;
  tagIds: string[];
  tagNames: string[];
  children: MenuTreeAdminDto[];
}

export interface CreateMenuItemDto {
  name: string;
  path: string;
  icon?: string;
  sort: number;
  parentId?: string;
  tagIds?: string[];
}

export interface MenuPathDto {
  path: string;
  name: string;
  icon?: string;
  description?: string;
  exists: boolean;
}

export interface UserTagDto {
  userId: string;
  username: string;
  userTypeId?: string;
  userTypeName?: string;
  tagIds: string[];
  tagNames: string[];
}

export interface UserTypeDto {
  id: string;
  name: string;
  code: string;
  description?: string;
  color: string;
  sort: number;
  tagIds: string[];
  tagNames: string[];
}

export interface CreateUserTypeDto {
  name: string;
  code: string;
  description?: string;
  color?: string;
  sort: number;
  tagIds?: string[];
}

export const tagApi = {
  list: () => requestClient.get<TagDto[]>('/tags'),
  create: (data: CreateTagDto) => requestClient.post<TagDto>('/tags', data),
  update: (id: string, data: CreateTagDto) => requestClient.put<TagDto>(`/tags/${id}`, data),
  delete: (id: string) => requestClient.delete(`/tags/${id}`),
};

export const menuAdminApi = {
  getAll: () => requestClient.get<MenuTreeAdminDto[]>('/admin/menus'),
  getPaths: () => requestClient.get<MenuPathDto[]>('/admin/menus/paths'),
  create: (data: CreateMenuItemDto) => requestClient.post('/admin/menus', data),
  update: (id: string, data: CreateMenuItemDto) => requestClient.put(`/admin/menus/${id}`, data),
  delete: (id: string) => requestClient.delete(`/admin/menus/${id}`),
};

export const userTagApi = {
  getUsers: () => requestClient.get<UserTagDto[]>('/admin/user-tags/users'),
  getUserTags: (userId: string) => requestClient.get<string[]>(`/admin/user-tags/${userId}`),
  updateUserTags: (userId: string, tagIds: string[]) => requestClient.put(`/admin/user-tags/${userId}`, tagIds),
};

export const userTypeApi = {
  list: () => requestClient.get<UserTypeDto[]>('/admin/user-types'),
  create: (data: CreateUserTypeDto) => requestClient.post<UserTypeDto>('/admin/user-types', data),
  update: (id: string, data: CreateUserTypeDto) => requestClient.put<UserTypeDto>(`/admin/user-types/${id}`, data),
  delete: (id: string) => requestClient.delete(`/admin/user-types/${id}`),
  assign: (userId: string, userTypeId?: string) => requestClient.put(`/admin/user-types/${userId}/assign`, userTypeId),
};
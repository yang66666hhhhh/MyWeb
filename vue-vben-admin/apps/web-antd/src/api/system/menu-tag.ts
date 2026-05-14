import { requestClient } from '#/api/request';

export interface TagDto {
  id: string;
  name: string;
  description?: string;
  color?: string;
  sort?: number;
  createdAt: string;
}

export interface CreateTagDto {
  name: string;
  description?: string;
  color?: string;
  sort?: number;
}

export interface MenuTreeAdminDto {
  id: string;
  parentId?: string;
  name: string;
  path: string;
  icon?: string;
  sort?: number;
  tagIds?: string[];
  children?: MenuTreeAdminDto[];
}

export interface MenuPathDto {
  path: string;
  name: string;
  icon?: string;
  exists?: boolean;
}

export interface UserTagDto {
  id: string;
  userId?: string;
  userTypeId?: string;
  username: string;
  realName: string;
  tags: string[];
}

export interface UserTypeDto {
  id: string;
  name: string;
  code: string;
  description?: string;
  color?: string;
  sort: number;
  tagIds?: string[];
  isActive: boolean;
}

export interface CreateUserTypeDto {
  name: string;
  code: string;
  description?: string;
  color?: string;
  sort: number;
  tagIds?: string[];
}

export interface CreateMenuItemDto {
  parentId?: string;
  path: string;
  name: string;
  icon?: string;
  sort?: number;
  tagIds?: string[];
}

export const tagApi = {
  list() {
    return requestClient.get<TagDto[]>('/tags');
  },
  create(data: CreateTagDto) {
    return requestClient.post<TagDto>('/tags', data);
  },
  update(id: string, data: CreateTagDto) {
    return requestClient.put<TagDto>(`/tags/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/tags/${id}`);
  },
};

export const menuAdminApi = {
  getAll() {
    return requestClient.get<MenuTreeAdminDto[]>('/admin/menus');
  },
  getPaths() {
    return requestClient.get<MenuPathDto[]>('/admin/menus/paths');
  },
  create(data: any) {
    return requestClient.post('/admin/menus', data);
  },
  update(id: string, data: any) {
    return requestClient.put(`/admin/menus/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/admin/menus/${id}`);
  },
};

export const userTagApi = {
  getUsers() {
    return requestClient.get<UserTagDto[]>('/admin/user-tags/users');
  },
  getUserTags(userId: string) {
    return requestClient.get<string[]>(`/admin/user-tags/${userId}`);
  },
  updateUserTags(userId: string, tagIds: string[]) {
    return requestClient.put(`/admin/user-tags/${userId}`, tagIds);
  },
};

export const userTypeApi = {
  list() {
    return requestClient.get<UserTypeDto[]>('/admin/user-types');
  },
  create(data: CreateUserTypeDto) {
    return requestClient.post<UserTypeDto>('/admin/user-types', data);
  },
  update(id: string, data: CreateUserTypeDto) {
    return requestClient.put<UserTypeDto>(`/admin/user-types/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/admin/user-types/${id}`);
  },
  assign(userId: string, userTypeId?: string) {
    return requestClient.put(`/admin/user-types/${userId}/assign`, userTypeId);
  },
};

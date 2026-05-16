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
    return requestClient.get<TagDto[]>('/system/tags');
  },
  create(data: CreateTagDto) {
    return requestClient.post<TagDto>('/system/tags', data);
  },
  update(id: string, data: CreateTagDto) {
    return requestClient.put<TagDto>(`/system/tags/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/system/tags/${id}`);
  },
};

export const menuAdminApi = {
  getAll() {
    return requestClient.get<MenuTreeAdminDto[]>('/system/menus');
  },
  getPaths() {
    return requestClient.get<MenuPathDto[]>('/system/menus/paths');
  },
  create(data: any) {
    return requestClient.post('/system/menus', data);
  },
  update(id: string, data: any) {
    return requestClient.put(`/system/menus/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/system/menus/${id}`);
  },
};

export const userTagApi = {
  getUsers() {
    return requestClient.get<UserTagDto[]>('/system/user-tags/users');
  },
  getUserTags(userId: string) {
    return requestClient.get<string[]>(`/system/user-tags/${userId}`);
  },
  updateUserTags(userId: string, tagIds: string[]) {
    return requestClient.put(`/system/user-tags/${userId}`, tagIds);
  },
};

export const userTypeApi = {
  list() {
    return requestClient.get<UserTypeDto[]>('/system/user-types');
  },
  create(data: CreateUserTypeDto) {
    return requestClient.post<UserTypeDto>('/system/user-types', data);
  },
  update(id: string, data: CreateUserTypeDto) {
    return requestClient.put<UserTypeDto>(`/system/user-types/${id}`, data);
  },
  delete(id: string) {
    return requestClient.delete(`/system/user-types/${id}`);
  },
  assign(userId: string, userTypeId?: string) {
    return requestClient.put(`/system/user-types/${userId}/assign`, userTypeId);
  },
};

import { requestClient } from '#/api/request';

export interface RoleMenuItem {
  id: string;
  parentId?: string;
  name: string;
  path: string;
  icon?: string;
  component?: string;
  sort: number;
  isVisible: boolean;
  isEnabled: boolean;
  permission?: string;
  redirect?: string;
  isExternal: boolean;
  badge?: string;
  tag?: string;
  minRoleLevel: number;
  personaTag?: string;
  isBaseMenu: boolean;
  menuCategory: string;
  featureCode?: string;
  children?: RoleMenuItem[];
}

export type UpsertRoleMenuInput = Omit<RoleMenuItem, 'children' | 'id'>;

/**
 * 获取当前用户菜单（基于角色+身份+标签）
 */
export async function getMyMenusApi(): Promise<RoleMenuItem[]> {
  return requestClient.get<RoleMenuItem[]>('/system/role-menus/mine');
}

/**
 * 获取菜单列表（管理用）
 */
export async function getMenuListApi(): Promise<RoleMenuItem[]> {
  return requestClient.get<RoleMenuItem[]>('/system/role-menus');
}

/**
 * 创建菜单
 */
export async function createRoleMenuApi(data: UpsertRoleMenuInput) {
  return requestClient.post<RoleMenuItem>('/system/role-menus', data);
}

/**
 * 更新菜单
 */
export async function updateRoleMenuApi(id: string, data: UpsertRoleMenuInput) {
  return requestClient.put<RoleMenuItem>(`/system/role-menus/${id}`, data);
}

/**
 * 删除菜单
 */
export async function deleteRoleMenuApi(id: string) {
  return requestClient.delete(`/system/role-menus/${id}`);
}

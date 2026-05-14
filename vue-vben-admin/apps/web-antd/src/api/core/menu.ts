import { requestClient } from '#/api/request';

export interface RoleMenuItem {
  id: string;
  bindingType?: 'Persona' | 'Role' | 'Tag';
  bindingValue?: string;
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

/**
 * 获取当前用户菜单（基于角色+身份+标签）
 */
export async function getMyMenusApi(): Promise<RoleMenuItem[]> {
  return requestClient.get<RoleMenuItem[]>('/role-menus/mine');
}

/**
 * 获取菜单列表（管理用，支持按绑定类型/值筛选）
 */
export async function getMenuListApi(bindingType?: string, bindingValue?: string): Promise<RoleMenuItem[]> {
  return requestClient.get<RoleMenuItem[]>('/role-menus', { params: { bindingType, bindingValue } });
}

/**
 * 创建菜单
 */
export async function createRoleMenuApi(data: Partial<RoleMenuItem>) {
  return requestClient.post<RoleMenuItem>('/role-menus', data);
}

/**
 * 更新菜单
 */
export async function updateRoleMenuApi(id: string, data: Partial<RoleMenuItem>) {
  return requestClient.put<RoleMenuItem>(`/role-menus/${id}`, data);
}

/**
 * 删除菜单
 */
export async function deleteRoleMenuApi(id: string) {
  return requestClient.delete(`/role-menus/${id}`);
}

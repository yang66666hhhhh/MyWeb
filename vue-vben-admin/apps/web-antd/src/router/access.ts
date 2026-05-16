import type {
  ComponentRecordType,
  GenerateMenuAndRoutesOptions,
} from '@vben/types';

import type { RoleMenuItem } from '#/api/core/menu';

import { generateAccessible } from '@vben/access';
import { preferences } from '@vben/preferences';

import { message } from 'ant-design-vue';

import { getMyMenusApi } from '#/api/core/menu';
import { BasicLayout, IFrameView } from '#/layouts';
import { $t } from '#/locales';

const forbiddenComponent = () => import('#/views/_core/fallback/forbidden.vue');

function normalizeMenuIcon(icon?: string) {
  // 后端菜单可能存有 lucide 图标名；Iconify 在未内置集合时会请求公网。
  // 本地业务菜单优先保持可用性，避免离线环境出现 api.iconify.design 报错。
  return icon?.startsWith('lucide:') ? undefined : icon;
}

function convertToRouteRecords(menus: RoleMenuItem[]): any[] {
  return menus
    .filter((menu) => menu && menu.path && typeof menu.path === 'string' && menu.path.length > 0)
    .map((menu) => {
      const childRoutes = menu.children && menu.children.length > 0
        ? convertToRouteRecords(menu.children)
        : [];

      return {
        component: menu.component || undefined,
        meta: {
          icon: normalizeMenuIcon(menu.icon),
          title: menu.name,
          badge: menu.badge,
          tag: menu.tag,
        },
        name: `${menu.path.replaceAll('/', '-').replace(/^-/, '')}`,
        path: menu.path,
        ...(menu.redirect ? { redirect: menu.redirect } : {}),
        ...(childRoutes.length > 0 ? { children: childRoutes } : {}),
      };
    });
}

async function generateAccess(options: GenerateMenuAndRoutesOptions) {
  const pageMap: ComponentRecordType = import.meta.glob('../views/**/*.vue');

  const layoutMap: ComponentRecordType = {
    BasicLayout,
    IFrameView,
  };

  return await generateAccessible(preferences.app.accessMode, {
    ...options,
    fetchMenuListAsync: async () => {
      message.loading({
        content: `${$t('common.loadingMenu')}...`,
        duration: 1.5,
      });
      const menus = await getMyMenusApi();
      return convertToRouteRecords(menus);
    },
    forbiddenComponent,
    layoutMap,
    pageMap,
  });
}

export { generateAccess };

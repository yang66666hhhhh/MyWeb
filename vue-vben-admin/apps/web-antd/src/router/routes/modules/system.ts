import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      access: ['pro', 'owner'],
      icon: 'lucide:settings',
      order: 90,
      title: '平台管理',
    },
    name: 'System',
    path: '/system',
    children: [
      {
        component: () => import('#/views/system/user/index.vue'),
        meta: { access: ['pro', 'owner'], icon: 'lucide:users', title: '用户管理' },
        name: 'SystemUser',
        path: '/system/user',
      },
      {
        component: () => import('#/views/system/role-menu/index.vue'),
        meta: { access: ['owner'], icon: 'lucide:shield', title: '角色菜单' },
        name: 'SystemRoleMenu',
        path: '/system/role-menu',
      },
      {
        component: () => import('#/views/system/persona/index.vue'),
        meta: { access: ['owner'], icon: 'lucide:user-check', title: '身份管理' },
        name: 'SystemPersona',
        path: '/system/persona',
      },
      {
        component: () => import('#/views/system/menu-tag/index.vue'),
        meta: { access: ['pro', 'owner'], icon: 'lucide:tag', title: '菜单标签' },
        name: 'SystemMenuTag',
        path: '/system/menu-tag',
      },
    ],
  },
];

export default routes;

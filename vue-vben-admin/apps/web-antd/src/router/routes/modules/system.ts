import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      access: ['admin', 'super'],
      icon: 'lucide:settings',
      order: 9997,
      title: '系统',
    },
    name: 'System',
    path: '/system',
    children: [
      {
        component: () => import('#/views/system/user/index.vue'),
        meta: { access: ['admin', 'super'], icon: 'lucide:users', title: '用户管理' },
        name: 'SystemUser',
        path: '/system/user',
      },
    ],
  },
];

export default routes;
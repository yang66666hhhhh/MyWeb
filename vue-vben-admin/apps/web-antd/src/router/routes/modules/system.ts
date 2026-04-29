import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:settings',
      order: 9997,
      title: '系统',
    },
    name: 'System',
    path: '/system',
    component: () => import('#/views/system/index.vue'),
  },
];

export default routes;
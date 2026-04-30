import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/profile',
    component: () => import('#/views/system/profile/index.vue'),
    meta: { title: '个人中心' },
  },
];

export default routes;
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:users',
      order: 60,
      title: '人脉网络',
    },
    name: 'Network',
    path: '/network',
    children: [
      {
        component: () => import('#/views/network/contact/index.vue'),
        meta: { icon: 'lucide:user-plus', title: '联系人' },
        name: 'NetworkContact',
        path: '/network/contact',
      },
      {
        component: () => import('#/views/network/interaction/index.vue'),
        meta: { icon: 'lucide:message-circle', title: '互动记录' },
        name: 'NetworkInteraction',
        path: '/network/interaction',
      },
    ],
  },
];

export default routes;

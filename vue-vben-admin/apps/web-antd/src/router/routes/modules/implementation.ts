import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:wrench',
      order: 50,
      title: '实施中心',
    },
    name: 'Implementation',
    path: '/implementation',
    children: [
      {
        component: () => import('#/views/implementation/kanban/index.vue'),
        meta: { icon: 'lucide:kanban', title: '项目看板' },
        name: 'ImplementationKanban',
        path: '/implementation/kanban',
      },
      {
        component: () => import('#/views/implementation/customers/index.vue'),
        meta: { icon: 'lucide:building', title: '客户管理' },
        name: 'ImplementationCustomers',
        path: '/implementation/customers',
      },
    ],
  },
];

export default routes;

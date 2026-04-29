import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:wallet',
      order: 30,
      title: '财务资产',
    },
    name: 'Assets',
    path: '/assets',
    children: [
      {
        component: () => import('#/views/assets/dashboard/index.vue'),
        meta: { icon: 'lucide:layout-dashboard', title: '财务看板' },
        name: 'AssetsDashboard',
        path: '/assets/dashboard',
      },
      {
        component: () => import('#/views/assets/income/index.vue'),
        meta: { icon: 'lucide:trending-up', title: '收入管理' },
        name: 'AssetsIncome',
        path: '/assets/income',
      },
      {
        component: () => import('#/views/assets/expenses/index.vue'),
        meta: { icon: 'lucide:trending-down', title: '支出管理' },
        name: 'AssetsExpenses',
        path: '/assets/expenses',
      },
      {
        component: () => import('#/views/assets/budget/index.vue'),
        meta: { icon: 'lucide:calculator', title: '预算规划' },
        name: 'AssetsBudget',
        path: '/assets/budget',
      },
      {
        component: () => import('#/views/assets/investments/index.vue'),
        meta: { icon: 'lucide:line-chart', title: '投资记录' },
        name: 'AssetsInvestments',
        path: '/assets/investments',
      },
      {
        component: () => import('#/views/assets/resources/index.vue'),
        meta: { icon: 'lucide:box', title: '资产清单' },
        name: 'AssetsResources',
        path: '/assets/resources',
      },
    ],
  },
];

export default routes;
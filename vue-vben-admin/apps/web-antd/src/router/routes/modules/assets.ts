import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:wallet',
      order: 40,
      title: '财务资产',
    },
    name: 'Assets',
    path: '/assets',
    children: [
      {
        component: () => import('#/views/assets/dashboard/index.vue'),
        meta: { icon: 'lucide:layout-dashboard', title: '资产看板' },
        name: 'AssetDashboard',
        path: '/assets/dashboard',
      },
      {
        component: () => import('#/views/assets/income/index.vue'),
        meta: { icon: 'lucide:trending-up', title: '收入记录' },
        name: 'AssetIncome',
        path: '/assets/income',
      },
      {
        component: () => import('#/views/assets/expenses/index.vue'),
        meta: { icon: 'lucide:trending-down', title: '支出记录' },
        name: 'AssetExpenses',
        path: '/assets/expenses',
      },
      {
        component: () => import('#/views/assets/budget/index.vue'),
        meta: { icon: 'lucide:pie-chart', title: '预算管理' },
        name: 'AssetBudget',
        path: '/assets/budget',
      },
      {
        component: () => import('#/views/assets/investments/index.vue'),
        meta: { icon: 'lucide:bar-chart-3', title: '投资管理' },
        name: 'AssetInvestments',
        path: '/assets/investments',
      },
      {
        component: () => import('#/views/assets/resources/index.vue'),
        meta: { icon: 'lucide:package', title: '资源中心' },
        name: 'AssetResources',
        path: '/assets/resources',
      },
    ],
  },
];

export default routes;

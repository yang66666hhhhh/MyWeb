import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:bar-chart-2',
      order: 60,
      title: '数据分析',
    },
    name: 'Analytics',
    path: '/analytics',
    children: [
      {
        component: () => import('#/views/analytics/growth/index.vue'),
        meta: { icon: 'lucide:sprout', title: '成长分析' },
        name: 'AnalyticsGrowth',
        path: '/analytics/growth',
      },
      {
        component: () => import('#/views/analytics/work/index.vue'),
        meta: { icon: 'lucide:briefcase', title: '工作分析' },
        name: 'AnalyticsWork',
        path: '/analytics/work',
      },
      {
        component: () => import('#/views/analytics/finance/index.vue'),
        meta: { icon: 'lucide:wallet', title: '财务分析' },
        name: 'AnalyticsFinance',
        path: '/analytics/finance',
      },
      {
        component: () => import('#/views/analytics/time/index.vue'),
        meta: { icon: 'lucide:clock', title: '时间分析' },
        name: 'AnalyticsTime',
        path: '/analytics/time',
      },
      {
        component: () => import('#/views/analytics/habits/index.vue'),
        meta: { icon: 'lucide:badge-check', title: '习惯分析' },
        name: 'AnalyticsHabits',
        path: '/analytics/habits',
      },
      {
        component: () => import('#/views/analytics/custom-reports/index.vue'),
        meta: { icon: 'lucide:file-bar-chart', title: '自定义报表' },
        name: 'AnalyticsCustom',
        path: '/analytics/custom-reports',
      },
      {
        component: () => import('#/views/analytics/ai-insights/index.vue'),
        meta: { icon: 'lucide:bot', title: 'AI洞察' },
        name: 'AnalyticsAi',
        path: '/analytics/ai-insights',
      },
    ],
  },
];

export default routes;
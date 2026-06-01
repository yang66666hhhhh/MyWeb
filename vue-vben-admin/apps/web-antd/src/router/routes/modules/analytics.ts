import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:bar-chart',
      order: 90,
      title: '数据分析',
    },
    name: 'Analytics',
    path: '/analytics',
    children: [
      {
        component: () => import('#/views/analytics/growth/index.vue'),
        meta: { icon: 'lucide:trending-up', title: '成长分析' },
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
        meta: { icon: 'lucide:dollar-sign', title: '财务分析' },
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
        meta: { icon: 'lucide:check-circle', title: '习惯分析' },
        name: 'AnalyticsHabits',
        path: '/analytics/habits',
      },
      {
        component: () => import('#/views/analytics/custom-reports/index.vue'),
        meta: { icon: 'lucide:file-text', title: '自定义报表' },
        name: 'AnalyticsCustomReports',
        path: '/analytics/custom-reports',
      },
      {
        component: () => import('#/views/analytics/ai-insights/index.vue'),
        meta: { icon: 'lucide:brain', title: 'AI洞察' },
        name: 'AnalyticsAiInsights',
        path: '/analytics/ai-insights',
      },
    ],
  },
];

export default routes;

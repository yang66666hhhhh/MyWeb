import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:sprout',
      order: 10,
      title: '个人成长',
    },
    name: 'Growth',
    path: '/growth',
    children: [
      {
        component: () => import('#/views/growth/dashboard/index.vue'),
        meta: {
          icon: 'lucide:gauge',
          title: '成长看板',
        },
        name: 'GrowthDashboard',
        path: '/growth/dashboard',
      },
      {
        component: () => import('#/views/growth/daily-plans/index.vue'),
        meta: {
          icon: 'lucide:calendar-check',
          keepAlive: true,
          title: '每日计划',
        },
        name: 'GrowthDailyPlans',
        path: '/growth/daily-plans',
      },
      {
        component: () => import('#/views/growth/habits/index.vue'),
        meta: {
          icon: 'lucide:badge-check',
          keepAlive: true,
          title: '习惯打卡',
        },
        name: 'GrowthHabits',
        path: '/growth/habits',
      },
      {
        component: () => import('#/views/growth/knowledge-base/index.vue'),
        meta: {
          icon: 'lucide:library',
          title: '知识库',
        },
        name: 'GrowthKnowledgeBase',
        path: '/growth/knowledge-base',
      },
    ],
  },
];

export default routes;

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
      {
        component: () => import('#/views/growth/skills/index.vue'),
        meta: {
          icon: 'lucide:award',
          title: '技能管理',
        },
        name: 'GrowthSkills',
        path: '/growth/skills',
      },
      {
        component: () => import('#/views/growth/reading-list/index.vue'),
        meta: {
          icon: 'lucide:book-open',
          title: '阅读清单',
        },
        name: 'GrowthReadingList',
        path: '/growth/reading-list',
      },
      {
        component: () => import('#/views/growth/mood-tracker/index.vue'),
        meta: {
          icon: 'lucide:smile',
          title: '心情追踪',
        },
        name: 'GrowthMoodTracker',
        path: '/growth/mood-tracker',
      },
      {
        component: () => import('#/views/growth/fitness/index.vue'),
        meta: {
          icon: 'lucide:dumbbell',
          title: '健身管理',
        },
        name: 'GrowthFitness',
        path: '/growth/fitness',
      },
      {
        component: () => import('#/views/growth/focus-timer/index.vue'),
        meta: {
          icon: 'lucide:timer',
          title: '专注计时',
        },
        name: 'GrowthFocusTimer',
        path: '/growth/focus-timer',
      },
      {
        component: () => import('#/views/growth/monthly-review/index.vue'),
        meta: {
          icon: 'lucide:calendar',
          title: '月度复盘',
        },
        name: 'GrowthMonthlyReview',
        path: '/growth/monthly-review',
      },
      {
        component: () => import('#/views/growth/year-plans/index.vue'),
        meta: {
          icon: 'lucide:flag',
          title: '年度计划',
        },
        name: 'GrowthYearPlans',
        path: '/growth/year-plans',
      },
    ],
  },
];

export default routes;

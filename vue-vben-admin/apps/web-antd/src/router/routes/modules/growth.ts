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
        meta: { icon: 'lucide:gauge', title: '成长看板' },
        name: 'GrowthDashboard',
        path: '/growth/dashboard',
      },
      {
        component: () => import('#/views/growth/daily-plans/index.vue'),
        meta: { icon: 'lucide:calendar-check', keepAlive: true, title: '每日计划' },
        name: 'GrowthDailyPlans',
        path: '/growth/daily-plans',
      },
      {
        component: () => import('#/views/growth/habits/index.vue'),
        meta: { icon: 'lucide:badge-check', keepAlive: true, title: '习惯打卡' },
        name: 'GrowthHabits',
        path: '/growth/habits',
      },
      {
        component: () => import('#/views/growth/knowledge-base/index.vue'),
        meta: { icon: 'lucide:library', title: '知识库' },
        name: 'GrowthKnowledgeBase',
        path: '/growth/knowledge-base',
      },
      {
        component: () => import('#/views/growth/skills/index.vue'),
        meta: { icon: 'lucide:zap', title: '技能管理' },
        name: 'GrowthSkills',
        path: '/growth/skills',
      },
      {
        component: () => import('#/views/growth/goals/index.vue'),
        meta: { icon: 'lucide:target', title: '目标管理' },
        name: 'GrowthGoals',
        path: '/growth/goals',
      },
      {
        component: () => import('#/views/growth/focus-timer/index.vue'),
        meta: { icon: 'lucide:timer', title: '专注计时' },
        name: 'GrowthFocusTimer',
        path: '/growth/focus-timer',
      },
      {
        component: () => import('#/views/growth/fitness/index.vue'),
        meta: { icon: 'lucide:dumbbell', title: '健身记录' },
        name: 'GrowthFitness',
        path: '/growth/fitness',
      },
      {
        component: () => import('#/views/growth/sleep/index.vue'),
        meta: { icon: 'lucide:moon', title: '睡眠记录' },
        name: 'GrowthSleep',
        path: '/growth/sleep',
      },
      {
        component: () => import('#/views/growth/mood-tracker/index.vue'),
        meta: { icon: 'lucide:smile', title: '心情记录' },
        name: 'GrowthMoodTracker',
        path: '/growth/mood-tracker',
      },
      {
        component: () => import('#/views/growth/reading-list/index.vue'),
        meta: { icon: 'lucide:book-open', title: '阅读清单' },
        name: 'GrowthReadingList',
        path: '/growth/reading-list',
      },
      {
        component: () => import('#/views/growth/courses/index.vue'),
        meta: { icon: 'lucide:graduation-cap', title: '课程学习' },
        name: 'GrowthCourses',
        path: '/growth/courses',
      },
      {
        component: () => import('#/views/growth/learning-path/index.vue'),
        meta: { icon: 'lucide:map', title: '学习路径' },
        name: 'GrowthLearningPath',
        path: '/growth/learning-path',
      },
      {
        component: () => import('#/views/growth/monthly-review/index.vue'),
        meta: { icon: 'lucide:calendar', title: '月度复盘' },
        name: 'GrowthMonthlyReview',
        path: '/growth/monthly-review',
      },
      {
        component: () => import('#/views/growth/year-plans/index.vue'),
        meta: { icon: 'lucide:calendar-days', title: '年度计划' },
        name: 'GrowthYearPlans',
        path: '/growth/year-plans',
      },
    ],
  },
];

export default routes;

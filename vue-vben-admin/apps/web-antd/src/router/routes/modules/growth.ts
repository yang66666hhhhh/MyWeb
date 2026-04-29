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
        component: () => import('#/views/growth/daily-plan/index.vue'),
        meta: {
          icon: 'lucide:calendar-check',
          keepAlive: true,
          title: '每日计划',
        },
        name: 'DailyPlanList',
        path: '/growth/daily-plans',
      },
      {
        component: () => import('#/views/growth/habit/index.vue'),
        meta: {
          icon: 'lucide:badge-check',
          keepAlive: true,
          title: '习惯打卡',
        },
        name: 'HabitList',
        path: '/growth/habits',
      },
      {
        component: () => import('#/views/growth/work-log/index.vue'),
        meta: {
          icon: 'lucide:book-open-check',
          title: '工作日志',
        },
        name: 'WorkLogList',
        path: '/growth/work-logs',
      },
      {
        component: () => import('#/views/growth/knowledge-base/index.vue'),
        meta: {
          icon: 'lucide:library',
          title: '知识库',
        },
        name: 'KnowledgeBaseList',
        path: '/growth/knowledge-base',
      },
      {
        meta: {
          icon: 'lucide:graduation-cap',
          title: '备考中心',
        },
        name: 'PostgraduateList',
        path: '/growth/postgraduate',
        children: [
          {
            component: () => import('#/views/growth/postgraduate/materials/index.vue'),
            meta: { icon: 'lucide:file-text', title: '备考资料' },
            name: 'PostgraduateMaterials',
            path: '/growth/postgraduate/materials',
          },
          {
            component: () => import('#/views/growth/postgraduate/mistakes/index.vue'),
            meta: { icon: 'lucide:x-circle', title: '错题本' },
            name: 'PostgraduateMistakes',
            path: '/growth/postgraduate/mistakes',
          },
          {
            component: () => import('#/views/growth/postgraduate/study-plans/index.vue'),
            meta: { icon: 'lucide:list-todo', title: '学习计划' },
            name: 'PostgraduateStudyPlans',
            path: '/growth/postgraduate/study-plans',
          },
        ],
      },
      {
        component: () => import('#/views/growth/project/index.vue'),
        meta: {
          icon: 'lucide:kanban',
          title: '项目管理',
        },
        name: 'ProjectList',
        path: '/growth/projects',
      },
    ],
  },
];

export default routes;

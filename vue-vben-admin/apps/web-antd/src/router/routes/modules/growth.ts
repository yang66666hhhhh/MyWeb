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
        meta: {
          icon: 'lucide:briefcase',
          order: 2,
          title: '工作管理',
        },
        name: 'WorkManagement',
        path: '/growth/work',
        children: [
          {
            component: () => import('#/views/growth/work/dashboard/index.vue'),
            meta: { icon: 'lucide:layout-dashboard', title: '工作看板' },
            name: 'WorkDashboard',
            path: '/growth/work/dashboard',
          },
          {
            component: () => import('#/views/growth/work/daily-plan/index.vue'),
            meta: { icon: 'lucide:calendar-check', title: '每日计划' },
            name: 'WorkDailyPlan',
            path: '/growth/work/daily-plan',
          },
          {
            component: () => import('#/views/growth/work/log/index.vue'),
            meta: { icon: 'lucide:file-text', title: '工作日志' },
            name: 'WorkLogManagement',
            path: '/growth/work/log',
          },
          {
            component: () => import('#/views/growth/work/import/index.vue'),
            meta: { icon: 'lucide:upload', title: '工作导入' },
            name: 'WorkImport',
            path: '/growth/work/import',
          },
          {
            component: () => import('#/views/growth/work/project/index.vue'),
            meta: { icon: 'lucide:folder', title: '项目管理' },
            name: 'WorkProject',
            path: '/growth/work/project',
          },
          {
            component: () => import('#/views/growth/work/device/index.vue'),
            meta: { icon: 'lucide:cpu', title: '设备管理' },
            name: 'WorkDevice',
            path: '/growth/work/device',
          },
          {
            component: () => import('#/views/growth/work/task-type/index.vue'),
            meta: { icon: 'lucide:tag', title: '任务类型' },
            name: 'WorkTaskType',
            path: '/growth/work/task-type',
          },
          {
            component: () => import('#/views/growth/work/statistics/index.vue'),
            meta: { icon: 'lucide:bar-chart-2', title: '工作统计' },
            name: 'WorkStatistics',
            path: '/growth/work/statistics',
          },
        ],
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
        component: () => import('#/views/growth/postgraduate/index.vue'),
        meta: {
          icon: 'lucide:graduation-cap',
          title: '备考中心',
        },
        name: 'ExamCenterDashboard',
        path: '/growth/exam-center',
        children: [
          {
            component: () => import('#/views/growth/postgraduate/study-plans/index.vue'),
            meta: { title: '学习计划' },
            name: 'StudyPlans',
            path: '/growth/exam-center/study-plans',
          },
          {
            component: () => import('#/views/growth/postgraduate/mistakes/index.vue'),
            meta: { title: '错题本' },
            name: 'Mistakes',
            path: '/growth/exam-center/mistakes',
          },
          {
            component: () => import('#/views/growth/postgraduate/materials/index.vue'),
            meta: { title: '资料库' },
            name: 'Materials',
            path: '/growth/exam-center/materials',
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

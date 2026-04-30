import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:briefcase',
      order: 20,
      title: '工作中心',
    },
    name: 'Work',
    path: '/work',
    children: [
      {
        component: () => import('#/views/work/dashboard/index.vue'),
        meta: { icon: 'lucide:layout-dashboard', title: '工作看板' },
        name: 'WorkDashboard',
        path: '/work/dashboard',
      },
      {
        component: () => import('#/views/work/daily-plan/index.vue'),
        meta: { icon: 'lucide:calendar-check', title: '每日计划' },
        name: 'WorkDailyPlan',
        path: '/work/daily-plan',
      },
      {
        component: () => import('#/views/work/task-type/index.vue'),
        meta: { icon: 'lucide:tag', title: '任务类型' },
        name: 'WorkTaskType',
        path: '/work/task-type',
      },
      {
        component: () => import('#/views/work/device/index.vue'),
        meta: { icon: 'lucide:monitor', title: '设备管理' },
        name: 'WorkDevice',
        path: '/work/device',
      },
      {
        component: () => import('#/views/work/project/index.vue'),
        meta: { icon: 'lucide:folder-kanban', title: '工作项目' },
        name: 'WorkProject',
        path: '/work/project',
      },
      {
        component: () => import('#/views/work/log/index.vue'),
        meta: { icon: 'lucide:clipboard-list', title: '工作日志' },
        name: 'WorkLog',
        path: '/work/log',
      },
      {
        component: () => import('#/views/work/import/index.vue'),
        meta: { icon: 'lucide:upload', title: '数据导入' },
        name: 'WorkImport',
        path: '/work/import',
      },
      {
        component: () => import('#/views/work/statistics/index.vue'),
        meta: { icon: 'lucide:bar-chart-3', title: '统计分析' },
        name: 'WorkStatistics',
        path: '/work/statistics',
      },
    ],
  },
];

export default routes;
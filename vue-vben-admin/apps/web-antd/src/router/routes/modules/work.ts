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
        component: () => import('#/views/work/log/index.vue'),
        meta: { icon: 'lucide:clipboard-list', title: '工作日志' },
        name: 'WorkLog',
        path: '/work/work-log',
      },
      {
        component: () => import('#/views/work/tasks/index.vue'),
        meta: { icon: 'lucide:check-square', title: '工作任务' },
        name: 'WorkTasks',
        path: '/work/tasks',
      },
      {
        component: () => import('#/views/work/project/index.vue'),
        meta: { icon: 'lucide:folder-kanban', title: '工作项目' },
        name: 'WorkProject',
        path: '/work/project',
      },
      {
        component: () => import('#/views/work/device/index.vue'),
        meta: { icon: 'lucide:monitor', title: '设备管理' },
        name: 'WorkDevice',
        path: '/work/device',
      },
      {
        component: () => import('#/views/work/statistics/index.vue'),
        meta: { icon: 'lucide:bar-chart-3', title: '统计分析' },
        name: 'WorkStatistics',
        path: '/work/statistics',
      },
      {
        component: () => import('#/views/work/import/index.vue'),
        meta: { icon: 'lucide:upload', title: '数据导入' },
        name: 'WorkImport',
        path: '/work/import',
      },
      {
        component: () => import('#/views/work/okr/index.vue'),
        meta: { icon: 'lucide:target', title: 'OKR管理' },
        name: 'WorkOkr',
        path: '/work/okr',
      },
      {
        component: () => import('#/views/work/gantt/index.vue'),
        meta: { icon: 'lucide:gantt-chart', title: '甘特图' },
        name: 'WorkGantt',
        path: '/work/gantt',
      },
      {
        component: () => import('#/views/work/impl-log/index.vue'),
        meta: { icon: 'lucide:file-text', title: '实施日志' },
        name: 'WorkImplLog',
        path: '/work/impl-log',
      },
      {
        component: () => import('#/views/work/files/index.vue'),
        meta: { icon: 'lucide:folder', title: '文件管理' },
        name: 'WorkFiles',
        path: '/work/files',
      },
      {
        component: () => import('#/views/work/risk-control/index.vue'),
        meta: { icon: 'lucide:shield', title: '风险管控' },
        name: 'WorkRiskControl',
        path: '/work/risk-control',
      },
      {
        component: () => import('#/views/work/software-assets/index.vue'),
        meta: { icon: 'lucide:package', title: '软件资产' },
        name: 'WorkSoftwareAssets',
        path: '/work/software-assets',
      },
      {
        component: () => import('#/views/work/task-type/index.vue'),
        meta: { icon: 'lucide:tags', title: '任务类型' },
        name: 'WorkTaskType',
        path: '/work/task-type',
      },
      {
        component: () => import('#/views/work/templates/index.vue'),
        meta: { icon: 'lucide:layout-template', title: '模板管理' },
        name: 'WorkTemplates',
        path: '/work/templates',
      },
      {
        component: () => import('#/views/work/weekly-plan/index.vue'),
        meta: { icon: 'lucide:calendar', title: '周计划' },
        name: 'WorkWeeklyPlan',
        path: '/work/weekly-plan',
      },
    ],
  },
];

export default routes;

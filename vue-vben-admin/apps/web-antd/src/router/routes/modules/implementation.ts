import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:wrench',
      order: 85,
      title: '实施中心',
    },
    name: 'Implementation',
    path: '/implementation',
    children: [
      {
        component: () => import('#/views/implementation/kanban/index.vue'),
        meta: { icon: 'lucide:layout-dashboard', title: '项目看板' },
        name: 'ImplementationKanban',
        path: '/implementation/kanban',
      },
      {
        component: () => import('#/views/implementation/customers/index.vue'),
        meta: { icon: 'lucide:users', title: '客户管理' },
        name: 'ImplementationCustomers',
        path: '/implementation/customers',
      },
      {
        component: () => import('#/views/implementation/weekly-report/index.vue'),
        meta: { icon: 'lucide:file-text', title: '周报' },
        name: 'ImplementationWeeklyReport',
        path: '/implementation/weekly-report',
      },
      {
        component: () => import('#/views/implementation/tasks/index.vue'),
        meta: { icon: 'lucide:check-square', title: '任务' },
        name: 'ImplementationTasks',
        path: '/implementation/tasks',
      },
      {
        component: () => import('#/views/implementation/impl-log/index.vue'),
        meta: { icon: 'lucide:clipboard-list', title: '实施日志' },
        name: 'ImplementationImplLog',
        path: '/implementation/impl-log',
      },
      {
        component: () => import('#/views/implementation/weekly-plan/index.vue'),
        meta: { icon: 'lucide:calendar', title: '周计划' },
        name: 'ImplementationWeeklyPlan',
        path: '/implementation/weekly-plan',
      },
    ],
  },
];

export default routes;

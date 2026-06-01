import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:code',
      order: 81,
      title: '开发者中心',
    },
    name: 'Dev',
    path: '/dev',
    children: [
      {
        component: () => import('#/views/dev/code-repository/index.vue'),
        meta: { icon: 'lucide:git-branch', title: '代码仓库' },
        name: 'DevCodeRepository',
        path: '/dev/code-repository',
      },
      {
        component: () => import('#/views/dev/issues/index.vue'),
        meta: { icon: 'lucide:alert-circle', title: '问题跟踪' },
        name: 'DevIssues',
        path: '/dev/issues',
      },
      {
        component: () => import('#/views/dev/pipelines/index.vue'),
        meta: { icon: 'lucide:git-pull-request', title: '流水线' },
        name: 'DevPipelines',
        path: '/dev/pipelines',
      },
    ],
  },
];

export default routes;

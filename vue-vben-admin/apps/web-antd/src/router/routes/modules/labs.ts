import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:flask-conical',
      order: 95,
      title: '实验中心',
    },
    name: 'Labs',
    path: '/labs',
    children: [
      {
        component: () => import('#/views/labs/ai-lab/index.vue'),
        meta: { icon: 'lucide:bot', title: 'AI 实验室' },
        name: 'LabsAiLab',
        path: '/labs/ai-lab',
      },
      {
        component: () => import('#/views/labs/data-lab/index.vue'),
        meta: { icon: 'lucide:database', title: '数据实验室' },
        name: 'LabsDataLab',
        path: '/labs/data-lab',
      },
      {
        component: () => import('#/views/labs/templates/index.vue'),
        meta: { icon: 'lucide:layout-template', title: '模板市场' },
        name: 'LabsTemplates',
        path: '/labs/templates',
      },
      {
        component: () => import('#/views/labs/ui-components/index.vue'),
        meta: { icon: 'lucide:component', title: 'UI 组件' },
        name: 'LabsUiComponents',
        path: '/labs/ui-components',
      },
    ],
  },
];

export default routes;
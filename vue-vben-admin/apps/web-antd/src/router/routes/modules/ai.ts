import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:bot',
      order: 70,
      title: 'AI智能',
    },
    name: 'Ai',
    path: '/ai',
    children: [
      {
        component: () => import('#/views/ai/assistant/index.vue'),
        meta: { icon: 'lucide:message-square', title: 'AI助手' },
        name: 'AiAssistant',
        path: '/ai/assistant',
      },
      {
        component: () => import('#/views/ai/planner/index.vue'),
        meta: { icon: 'lucide:calendar-heart', title: '智能计划' },
        name: 'AiPlanner',
        path: '/ai/planner',
      },
    ],
  },
];

export default routes;

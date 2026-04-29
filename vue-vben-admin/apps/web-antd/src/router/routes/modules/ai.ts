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
      {
        component: () => import('#/views/ai/reports/index.vue'),
        meta: { icon: 'lucide:file-text', title: '周报生成' },
        name: 'AiReports',
        path: '/ai/reports',
      },
      {
        component: () => import('#/views/ai/knowledge-chat/index.vue'),
        meta: { icon: 'lucide:book-open', title: '知识问答' },
        name: 'AiKnowledge',
        path: '/ai/knowledge-chat',
      },
      {
        component: () => import('#/views/ai/insights/index.vue'),
        meta: { icon: 'lucide:sparkles', title: '数据洞察' },
        name: 'AiInsights',
        path: '/ai/insights',
      },
      {
        component: () => import('#/views/ai/automation/index.vue'),
        meta: { icon: 'lucide:workflow', title: '自动化工作流' },
        name: 'AiAutomation',
        path: '/ai/automation',
      },
    ],
  },
];

export default routes;
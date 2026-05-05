import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:graduation-cap',
      order: 50,
      title: '学生中心',
    },
    name: 'Student',
    path: '/student',
    children: [
      {
        component: () => import('#/views/student/learning/index.vue'),
        meta: { icon: 'lucide:calendar-check', title: '学习计划' },
        name: 'StudentLearning',
        path: '/student/learning',
      },
      {
        component: () => import('#/views/student/mistakes/index.vue'),
        meta: { icon: 'lucide:alert-circle', title: '错题本' },
        name: 'StudentMistakes',
        path: '/student/mistakes',
      },
      {
        component: () => import('#/views/student/postgraduate/index.vue'),
        meta: { icon: 'lucide:graduation-cap', title: '考研备考' },
        name: 'StudentPostgraduate',
        path: '/student/postgraduate',
      },
    ],
  },
];

export default routes;

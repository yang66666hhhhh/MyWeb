import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:graduation-cap',
      order: 80,
      title: '学生中心',
    },
    name: 'Student',
    path: '/student',
    children: [
      {
        component: () => import('#/views/student/dashboard/index.vue'),
        meta: { icon: 'lucide:layout-dashboard', title: '学习总览' },
        name: 'StudentDashboard',
        path: '/student/dashboard',
      },
      {
        component: () => import('#/views/student/learning/index.vue'),
        meta: { icon: 'lucide:book-open', title: '学习计划' },
        name: 'StudentLearning',
        path: '/student/learning',
      },
      {
        component: () => import('#/views/student/review/index.vue'),
        meta: { icon: 'lucide:repeat', title: '复习日程' },
        name: 'StudentReview',
        path: '/student/review',
      },
      {
        component: () => import('#/views/student/mistakes/index.vue'),
        meta: { icon: 'lucide:x-circle', title: '错题本' },
        name: 'StudentMistakes',
        path: '/student/mistakes',
      },
      {
        component: () => import('#/views/student/materials/index.vue'),
        meta: { icon: 'lucide:folder', title: '学习资料' },
        name: 'StudentMaterials',
        path: '/student/materials',
      },
      {
        component: () => import('#/views/student/records/index.vue'),
        meta: { icon: 'lucide:clock', title: '学习记录' },
        name: 'StudentRecords',
        path: '/student/records',
      },
      {
        component: () => import('#/views/student/subjects/index.vue'),
        meta: { icon: 'lucide:target', title: '科目目标' },
        name: 'StudentSubjects',
        path: '/student/subjects',
      },
    ],
  },
];

export default routes;

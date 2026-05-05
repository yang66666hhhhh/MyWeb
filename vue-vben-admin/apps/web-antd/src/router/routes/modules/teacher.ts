import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:presentation',
      order: 50,
      title: '教师中心',
    },
    name: 'Teacher',
    path: '/teacher',
    children: [
      {
        component: () => import('#/views/teacher/courses/index.vue'),
        meta: { icon: 'lucide:book-open', title: '课程管理' },
        name: 'TeacherCourses',
        path: '/teacher/courses',
      },
      {
        component: () => import('#/views/teacher/students/index.vue'),
        meta: { icon: 'lucide:users', title: '学生管理' },
        name: 'TeacherStudents',
        path: '/teacher/students',
      },
    ],
  },
];

export default routes;

import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:file-text',
      order: 50,
      title: '内容管理',
    },
    name: 'Content',
    path: '/content',
    children: [
      {
        component: () => import('#/views/content/article/index.vue'),
        meta: { icon: 'lucide:file-edit', title: '文章管理' },
        name: 'ContentArticle',
        path: '/content/article',
      },
      {
        component: () => import('#/views/content/media/index.vue'),
        meta: { icon: 'lucide:image', title: '媒体文件' },
        name: 'ContentMedia',
        path: '/content/media',
      },
      {
        component: () => import('#/views/content/calendar/index.vue'),
        meta: { icon: 'lucide:calendar', title: '发布日历' },
        name: 'ContentCalendar',
        path: '/content/calendar',
      },
    ],
  },
];

export default routes;

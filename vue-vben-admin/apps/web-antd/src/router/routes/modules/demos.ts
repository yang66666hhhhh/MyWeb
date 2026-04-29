import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:sparkles',
      order: 1000,
      title: '示例中心',
    },
    name: 'Demos',
    path: '/demos',
    children: [
      {
        meta: {
          icon: 'lucide:layout-grid',
          title: 'Ant Design 示例',
        },
        name: 'AntdDemos',
        path: '/demos/antd',
        component: () => import('#/views/demos/antd/index.vue'),
      },
    ],
  },
];

export default routes;

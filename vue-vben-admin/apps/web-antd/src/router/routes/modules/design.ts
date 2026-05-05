import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:palette',
      order: 50,
      title: '设计师中心',
    },
    name: 'Design',
    path: '/design',
    children: [
      {
        component: () => import('#/views/design/assets/index.vue'),
        meta: { icon: 'lucide:image', title: '设计资产' },
        name: 'DesignAssets',
        path: '/design/assets',
      },
      {
        component: () => import('#/views/design/prototypes/index.vue'),
        meta: { icon: 'lucide:figma', title: '原型管理' },
        name: 'DesignPrototypes',
        path: '/design/prototypes',
      },
    ],
  },
];

export default routes;

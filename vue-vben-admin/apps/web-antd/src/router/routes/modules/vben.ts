import type { RouteRecordRaw } from 'vue-router';

import {
  VBEN_ANTDV_NEXT_PREVIEW_URL,
  VBEN_DOC_URL,
  VBEN_ELE_PREVIEW_URL,
  VBEN_GITHUB_URL,
  VBEN_LOGO_URL,
  VBEN_NAIVE_PREVIEW_URL,
  VBEN_TD_PREVIEW_URL,
} from '@vben/constants';
import { SvgAntdvNextLogoIcon, SvgTDesignIcon } from '@vben/icons';

import { IFrameView } from '#/layouts';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:external-link',
      order: 9998,
      title: '外部链接',
    },
    name: 'ExternalLinks',
    path: '/external-links',
    children: [
      {
        name: 'VbenDocument',
        path: '/external-links/document',
        component: IFrameView,
        meta: {
          icon: 'lucide:book-open',
          iframeSrc: VBEN_DOC_URL,
          title: '官方文档',
        },
      },
      {
        name: 'VbenGithub',
        path: '/external-links/github',
        component: IFrameView,
        meta: {
          icon: 'lucide:github',
          link: VBEN_GITHUB_URL,
          title: 'Github',
        },
      },
      {
        name: 'VbenAntdv',
        path: '/external-links/antdv',
        component: IFrameView,
        meta: {
          icon: 'lucide:layout-grid',
          link: VBEN_ANTDV_NEXT_PREVIEW_URL,
          title: 'Ant Design Vue Pro',
        },
      },
    ],
  },
  {
    name: 'About',
    path: '/about',
    component: () => import('#/views/about/index.vue'),
    meta: {
      icon: 'lucide:info',
      order: 9999,
      title: '关于',
    },
  },
  {
    name: 'Profile',
    path: '/profile',
    component: () => import('#/views/_core/profile/index.vue'),
    meta: {
      icon: 'lucide:user',
      hideInMenu: true,
      title: '个人设置',
    },
  },
];

export default routes;

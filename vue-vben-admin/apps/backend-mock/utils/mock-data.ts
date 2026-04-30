export interface UserInfo {
  id: number;
  password: string;
  realName: string;
  roles: string[];
  username: string;
  homePath?: string;
}

export interface TimezoneOption {
  offset: number;
  timezone: string;
}

export const MOCK_USERS: UserInfo[] = [
  {
    id: 0,
    password: '123456',
    realName: 'Vben',
    roles: ['super'],
    username: 'vben',
  },
  {
    id: 1,
    password: '123456',
    realName: 'Admin',
    roles: ['admin'],
    username: 'admin',
    homePath: '/workspace',
  },
  {
    id: 2,
    password: '123456',
    realName: 'Jack',
    roles: ['user'],
    username: 'jack',
    homePath: '/analytics',
  },
];

export const MOCK_CODES = [
  // super
  {
    codes: ['AC_100100', 'AC_100110', 'AC_100120', 'AC_100010'],
    username: 'vben',
  },
  {
    // admin
    codes: ['AC_100010', 'AC_100020', 'AC_100030'],
    username: 'admin',
  },
  {
    // user
    codes: ['AC_1000001', 'AC_1000002'],
    username: 'jack',
  },
];

const dashboardMenus = [
  {
    meta: {
      icon: 'lucide:home',
      order: -1,
      title: '首页',
    },
    name: 'Dashboard',
    path: '/dashboard',
    redirect: '/dashboard/workspace',
    children: [
      {
        name: 'Workspace',
        path: '/dashboard/workspace',
        component: '/dashboard/workspace/index',
        meta: {
          title: '工作台',
        },
      },
      {
        name: 'Analytics',
        path: '/dashboard/analytics',
        component: '/dashboard/analytics/index',
        meta: {
          affixTab: true,
          title: '数据分析',
        },
      },
    ],
  },
];

const growthMenus = [
  {
    meta: {
      icon: 'lucide:sprout',
      order: 10,
      title: '个人成长',
    },
    name: 'Growth',
    path: '/growth',
    children: [
      {
        component: '/growth/dashboard/index',
        meta: {
          icon: 'lucide:gauge',
          title: '成长看板',
        },
        name: 'GrowthDashboard',
        path: '/growth/dashboard',
      },
      {
        component: '/growth/daily-plans/index',
        meta: {
          icon: 'lucide:calendar-check',
          keepAlive: true,
          title: '每日计划',
        },
        name: 'GrowthDailyPlans',
        path: '/growth/daily-plans',
      },
      {
        component: '/growth/habits/index',
        meta: {
          icon: 'lucide:badge-check',
          keepAlive: true,
          title: '习惯打卡',
        },
        name: 'GrowthHabits',
        path: '/growth/habits',
      },
      {
        component: '/growth/work-log/index',
        meta: {
          icon: 'lucide:book-open-check',
          title: '工作日志',
        },
        name: 'GrowthWorkLog',
        path: '/growth/work-log',
      },
      {
        component: '/growth/knowledge-base/index',
        meta: {
          icon: 'lucide:library',
          title: '知识库',
        },
        name: 'GrowthKnowledgeBase',
        path: '/growth/knowledge-base',
      },
      {
        meta: {
          icon: 'lucide:graduation-cap',
          title: '备考中心',
        },
        name: 'GrowthPostgraduate',
        path: '/growth/postgraduate',
        children: [
          {
            component: '/growth/postgraduate/index',
            meta: {
              icon: 'lucide:home',
              title: '备考首页',
            },
            name: 'GrowthPostgraduateHome',
            path: '/growth/postgraduate',
          },
          {
            component: '/growth/postgraduate/materials/index',
            meta: {
              icon: 'lucide:file-text',
              title: '备考资料',
            },
            name: 'GrowthPostgraduateMaterials',
            path: '/growth/postgraduate/materials',
          },
          {
            component: '/growth/postgraduate/mistakes/index',
            meta: {
              icon: 'lucide:x-circle',
              title: '错题本',
            },
            name: 'GrowthPostgraduateMistakes',
            path: '/growth/postgraduate/mistakes',
          },
          {
            component: '/growth/postgraduate/study-plans/index',
            meta: {
              icon: 'lucide:list-todo',
              title: '学习计划',
            },
            name: 'GrowthPostgraduateStudyPlans',
            path: '/growth/postgraduate/study-plans',
          },
        ],
      },
      {
        component: '/growth/project/index',
        meta: {
          icon: 'lucide:kanban',
          title: '项目管理',
        },
        name: 'GrowthProjects',
        path: '/growth/projects',
      },
    ],
  },
];

const workMenus = [
  {
    meta: {
      icon: 'lucide:briefcase',
      order: 20,
      title: '工作中心',
    },
    name: 'Work',
    path: '/work',
    children: [
      {
        component: '/work/dashboard/index',
        meta: {
          icon: 'lucide:layout-dashboard',
          title: '工作看板',
        },
        name: 'WorkDashboard',
        path: '/work/dashboard',
      },
      {
        component: '/work/daily-plan/index',
        meta: {
          icon: 'lucide:calendar-check',
          title: '每日计划',
        },
        name: 'WorkDailyPlan',
        path: '/work/daily-plan',
      },
      {
        component: '/work/task-type/index',
        meta: {
          icon: 'lucide:tag',
          title: '任务类型',
        },
        name: 'WorkTaskType',
        path: '/work/task-type',
      },
      {
        component: '/work/device/index',
        meta: {
          icon: 'lucide:monitor',
          title: '设备管理',
        },
        name: 'WorkDevice',
        path: '/work/device',
      },
      {
        component: '/work/project/index',
        meta: {
          icon: 'lucide:folder-kanban',
          title: '工作项目',
        },
        name: 'WorkProject',
        path: '/work/project',
      },
      {
        component: '/work/log/index',
        meta: {
          icon: 'lucide:clipboard-list',
          title: '工作日志',
        },
        name: 'WorkLog',
        path: '/work/log',
      },
      {
        component: '/work/import/index',
        meta: {
          icon: 'lucide:upload',
          title: '数据导入',
        },
        name: 'WorkImport',
        path: '/work/import',
      },
      {
        component: '/work/statistics/index',
        meta: {
          icon: 'lucide:bar-chart-3',
          title: '统计分析',
        },
        name: 'WorkStatistics',
        path: '/work/statistics',
      },
    ],
  },
];

const createDemosMenus = (role: 'admin' | 'super' | 'user') => {
  return [
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
          name: 'AntdDemos',
          path: '/demos/antd',
          component: '/demos/antd/index',
          meta: {
            icon: 'lucide:layout-grid',
            title: 'Ant Design 示例',
          },
        },
      ],
    },
  ];
};

export const MOCK_MENUS = [
  {
    menus: [...dashboardMenus, ...growthMenus, ...workMenus, ...createDemosMenus('super')],
    username: 'vben',
  },
  {
    menus: [...dashboardMenus, ...growthMenus, ...workMenus, ...createDemosMenus('admin')],
    username: 'admin',
  },
  {
    menus: [...dashboardMenus, ...growthMenus, ...workMenus, ...createDemosMenus('user')],
    username: 'jack',
  },
];

export const MOCK_MENU_LIST = [
  {
    id: 1,
    name: 'Workspace',
    status: 1,
    type: 'menu',
    icon: 'lucide:home',
    path: '/dashboard/workspace',
    component: '/dashboard/workspace/index',
    meta: {
      icon: 'lucide:home',
      title: '工作台',
      order: -1,
    },
  },
  {
    id: 2,
    meta: {
      icon: 'lucide:settings',
      order: 9997,
      title: '系统',
    },
    status: 1,
    type: 'catalog',
    name: 'System',
    path: '/system',
    children: [
      {
        id: 201,
        pid: 2,
        path: '/system/menu',
        name: 'SystemMenu',
        authCode: 'System:Menu:List',
        status: 1,
        type: 'menu',
        meta: {
          icon: 'lucide:menu',
          title: '菜单管理',
        },
        component: '/system/menu/list',
        children: [
          {
            id: 20_101,
            pid: 201,
            name: 'SystemMenuCreate',
            status: 1,
            type: 'button',
            authCode: 'System:Menu:Create',
            meta: { title: '新增' },
          },
          {
            id: 20_102,
            pid: 201,
            name: 'SystemMenuEdit',
            status: 1,
            type: 'button',
            authCode: 'System:Menu:Edit',
            meta: { title: '编辑' },
          },
          {
            id: 20_103,
            pid: 201,
            name: 'SystemMenuDelete',
            status: 1,
            type: 'button',
            authCode: 'System:Menu:Delete',
            meta: { title: '删除' },
          },
        ],
      },
      {
        id: 202,
        pid: 2,
        path: '/system/dept',
        name: 'SystemDept',
        status: 1,
        type: 'menu',
        authCode: 'System:Dept:List',
        meta: {
          icon: 'lucide:building',
          title: '部门管理',
        },
        component: '/system/dept/list',
        children: [
          {
            id: 20_401,
            pid: 202,
            name: 'SystemDeptCreate',
            status: 1,
            type: 'button',
            authCode: 'System:Dept:Create',
            meta: { title: '新增' },
          },
          {
            id: 20_402,
            pid: 202,
            name: 'SystemDeptEdit',
            status: 1,
            type: 'button',
            authCode: 'System:Dept:Edit',
            meta: { title: '编辑' },
          },
          {
            id: 20_403,
            pid: 202,
            name: 'SystemDeptDelete',
            status: 1,
            type: 'button',
            authCode: 'System:Dept:Delete',
            meta: { title: '删除' },
          },
        ],
      },
    ],
  },
  {
    id: 9,
    meta: {
      order: 9998,
      title: '外部链接',
      icon: 'lucide:external-link',
    },
    name: 'ExternalLinks',
    path: '/vben-admin',
    type: 'catalog',
    status: 1,
    children: [
      {
        id: 901,
        pid: 9,
        name: 'VbenDocument',
        path: '/vben-admin/document',
        component: 'IFrameView',
        type: 'embedded',
        status: 1,
        meta: {
          icon: 'lucide:book-open',
          iframeSrc: 'https://doc.vben.pro',
          title: '官方文档',
        },
      },
      {
        id: 902,
        pid: 9,
        name: 'VbenGithub',
        path: '/vben-admin/github',
        component: 'IFrameView',
        type: 'link',
        status: 1,
        meta: {
          icon: 'lucide:github',
          link: 'https://github.com/vbenjs/vue-vben-admin',
          title: 'Github',
        },
      },
      {
        id: 903,
        pid: 9,
        name: 'VbenAntdv',
        path: '/vben-admin/antdv',
        component: 'IFrameView',
        type: 'link',
        status: 0,
        meta: {
          icon: 'lucide:layout-grid',
          badgeType: 'dot',
          link: 'https://ant.vben.pro',
          title: 'Ant Design Vue Pro',
        },
      },
    ],
  },
  {
    id: 10,
    component: '_core/about/index',
    type: 'menu',
    status: 1,
    meta: {
      icon: 'lucide:info',
      order: 9999,
      title: '关于',
    },
    name: 'About',
    path: '/about',
  },
];

export function getMenuIds(menus: any[]) {
  const ids: number[] = [];
  menus.forEach((item) => {
    ids.push(item.id);
    if (item.children && item.children.length > 0) {
      ids.push(...getMenuIds(item.children));
    }
  });
  return ids;
}

/**
 * 时区选项
 */
export const TIME_ZONE_OPTIONS: TimezoneOption[] = [
  {
    offset: -5,
    timezone: 'America/New_York',
  },
  {
    offset: 0,
    timezone: 'Europe/London',
  },
  {
    offset: 8,
    timezone: 'Asia/Shanghai',
  },
  {
    offset: 9,
    timezone: 'Asia/Tokyo',
  },
  {
    offset: 9,
    timezone: 'Asia/Seoul',
  },
];

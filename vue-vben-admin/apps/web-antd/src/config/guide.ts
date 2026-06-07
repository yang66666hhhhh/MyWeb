export interface GuideStep {
  element: string;
  popover: {
    title: string;
    description: string;
    side?: 'bottom' | 'left' | 'right' | 'top';
    align?: 'center' | 'end' | 'start';
  };
}

export interface FeatureTooltipConfig {
  id: string;
  title: string;
  description: string;
  placement?: 'bottom' | 'left' | 'right' | 'top';
}

export const GUIDE_STORAGE_KEYS = {
  TOUR_COMPLETED: 'guide_tour_completed',
  TOUR_SKIPPED: 'guide_tour_skipped',
  FEATURE_DISMISSED: 'guide_feature_dismissed_',
  PAGE_VISITED: 'guide_page_visited_',
} as const;

export const TOUR_STEPS: Record<string, GuideStep[]> = {
  dashboard: [
    {
      element: '[data-guide="sidebar"]',
      popover: {
        title: '导航菜单',
        description: '通过侧边栏快速访问各个功能模块',
        side: 'right',
      },
    },
    {
      element: '[data-guide="dashboard-stats"]',
      popover: {
        title: '数据概览',
        description: '这里展示您的核心成长数据指标',
        side: 'bottom',
      },
    },
    {
      element: '[data-guide="quick-actions"]',
      popover: {
        title: '快捷操作',
        description: '快速创建目标、记录成长瞬间',
        side: 'bottom',
      },
    },
  ],
  goals: [
    {
      element: '[data-guide="goal-list"]',
      popover: {
        title: '目标列表',
        description: '查看和管理您的所有成长目标',
        side: 'right',
      },
    },
    {
      element: '[data-guide="add-goal"]',
      popover: {
        title: '创建目标',
        description: '点击这里添加新的成长目标',
        side: 'bottom',
      },
    },
  ],
  growth: [
    {
      element: '[data-guide="growth-timeline"]',
      popover: {
        title: '成长时间线',
        description: '记录和回顾您的成长历程',
        side: 'right',
      },
    },
    {
      element: '[data-guide="add-record"]',
      popover: {
        title: '记录成长',
        description: '点击记录新的成长瞬间',
        side: 'bottom',
      },
    },
  ],
};

export const FEATURE_TOOLTIPS: FeatureTooltipConfig[] = [
  {
    id: 'ai-assistant',
    title: 'AI 助手',
    description: '智能分析您的成长数据，提供个性化建议',
    placement: 'bottom',
  },
  {
    id: 'analytics',
    title: '数据分析',
    description: '可视化展示您的成长轨迹和趋势',
    placement: 'bottom',
  },
  {
    id: 'persona-switch',
    title: '身份切换',
    description: '在不同角色视角间灵活切换',
    placement: 'bottom',
  },
];

export const EMPTY_STATE_CONFIGS: Record<string, { icon: string; title: string; description: string; actionText: string }> = {
  goals: {
    icon: '🎯',
    title: '设定您的第一个目标',
    description: '明确的目标是成长的起点。设定一个可衡量的目标，开始您的成长之旅。',
    actionText: '创建目标',
  },
  records: {
    icon: '📝',
    title: '记录您的成长',
    description: '记录每一个成长瞬间，见证自己的进步。',
    actionText: '记录成长',
  },
  analytics: {
    icon: '📊',
    title: '查看成长分析',
    description: '当您有足够的成长记录后，这里将展示您的成长趋势分析。',
    actionText: '了解详情',
  },
};

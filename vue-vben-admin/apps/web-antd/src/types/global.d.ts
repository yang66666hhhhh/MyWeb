// 全局类型定义

// API 响应类型
export interface ApiResponse<T = any> {
  code: number;
  message: string;
  data: T;
}

// 分页响应
export interface PageResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// 分页查询参数
export interface PageQuery {
  page?: number;
  pageSize?: number;
}

// 通用查询参数
export interface ListQuery extends PageQuery {
  keyword?: string;
  status?: number | string;
  startDate?: string;
  endDate?: string;
  [key: string]: any;
}

// 实体基础类型
export interface BaseEntity {
  id: string;
  createdAt: string;
  updatedAt?: string;
  isDeleted?: boolean;
}

// 用户类型
export interface User extends BaseEntity {
  username: string;
  realName: string;
  avatar?: string;
  email?: string;
  roles: string;
  status: number;
}

// 角色类型
export type RoleType = 'member' | 'owner' | 'pro';

// 状态类型
export type StatusType = 'cancelled' | 'completed' | 'in_progress' | 'pending';

// 优先级类型
export type PriorityType = 'high' | 'low' | 'medium' | 'urgent';

// 表格列配置
export interface TableColumn {
  align?: 'center' | 'left' | 'right';
  dataIndex: string;
  fixed?: 'left' | 'right';
  key?: string;
  sorter?: boolean;
  title: string;
  width?: number;
}

// 表单项配置
export interface FormField {
  component?: string;
  defaultValue?: any;
  disabled?: boolean;
  label: string;
  name: string;
  options?: Array<{ label: string; value: any }>;
  placeholder?: string;
  required?: boolean;
  rules?: any[];
  type?: 'checkbox' | 'date' | 'input' | 'number' | 'radio' | 'select' | 'textarea';
}

// 菜单项类型
export interface MenuItem {
  children?: MenuItem[];
  component?: string;
  icon?: string;
  id: string;
  name: string;
  order?: number;
  path: string;
  permission?: string;
}

// 通知类型
export interface Notification {
  content: string;
  id: string;
  read: boolean;
  time: string;
  title: string;
  type: 'error' | 'info' | 'success' | 'warning';
}

// 主题配置
export interface ThemeConfig {
  colorPrimary: string;
  darkMode: boolean;
  fontSize: number;
  layout: 'mix' | 'side' | 'top';
  siderCollapsed: boolean;
}

// 应用配置
export interface AppConfig {
  locale: string;
  theme: ThemeConfig;
  title: string;
  version: string;
}

// 图表数据类型
export interface ChartData {
  label: string;
  value: number;
}

export interface TimeSeriesData {
  date: string;
  value: number;
}

// 文件类型
export interface FileInfo {
  extension: string;
  mimeType: string;
  name: string;
  size: number;
  url: string;
}

// 树形结构
export interface TreeNode {
  children?: TreeNode[];
  disabled?: boolean;
  id: string | number;
  isLeaf?: boolean;
  key: string | number;
  title: string;
}

// 事件类型
export interface AppEvent {
  payload?: any;
  timestamp: number;
  type: string;
}

// 工具函数类型
export type AsyncFunction<T = void> = () => Promise<T>;
export type CallbackFunction<T = void> = (...args: any[]) => T;
export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type ReadonlyDeep<T> = {
  readonly [P in keyof T]: T[P] extends object ? ReadonlyDeep<T[P]> : T[P];
};

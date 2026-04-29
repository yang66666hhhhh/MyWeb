import type { AxiosRequestConfig } from 'axios';

export interface RequestOptions extends AxiosRequestConfig {
  /** 请求路径 */
  url: string;
  /** 请求方法 */
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';
  /** 是否显示加载中 */
  loading?: boolean;
  /** 是否忽略统一错误处理 */
  ignoreError?: boolean;
  /** 请求超时时间(ms) */
  timeout?: number;
}

export interface UploadFile {
  name: string;
  url: string;
  size?: number;
  uid?: string;
}

export interface UploadParams {
  file: File;
  name?: string;
  tip?: string;
}

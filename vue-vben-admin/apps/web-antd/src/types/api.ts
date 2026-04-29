import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';

export interface ApiResult<T = any> {
  code: number;
  data: T;
  message?: string;
}

export interface PageQuery {
  page?: number;
  pageSize?: number;
  keyword?: string;
}

export interface PageResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface UploadResult {
  url: string;
  fileName: string;
  fileSize: number;
}

export type ApiPromise<T> = Promise<ApiResult<T>>;

export interface RequestOptions {
  /** 请求路径 */
  url: string;
  /** 请求方法 */
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';
  /** 请求参数 */
  params?: any;
  /** 请求体 */
  data?: any;
  /** 配置选项 */
  options?: AxiosRequestConfig;
}

export { AxiosInstance, AxiosRequestConfig, AxiosResponse };
export type { AxiosInstance as HttpInstance };

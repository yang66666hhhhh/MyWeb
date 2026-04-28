import type { PageQuery, PageResult } from './types';

export function createId(prefix: string) {
  return `${prefix}-${Date.now()}-${Math.random().toString(16).slice(2, 8)}`;
}

export function today() {
  return new Date().toISOString().slice(0, 10);
}

export async function mockDelay<T>(data: T) {
  await new Promise((resolve) => setTimeout(resolve, 120));
  return data;
}

export function createPageResult<T>(
  source: T[],
  query: PageQuery = {},
): PageResult<T> {
  const page = query.page ?? 1;
  const pageSize = query.pageSize ?? 10;
  const start = (page - 1) * pageSize;
  const items = source.slice(start, start + pageSize);
  return {
    items,
    page,
    pageSize,
    total: source.length,
    totalPages: Math.ceil(source.length / pageSize),
  };
}

export function includesKeyword(value: unknown, keyword?: string) {
  if (!keyword?.trim()) {
    return true;
  }
  return String(value ?? '')
    .toLowerCase()
    .includes(keyword.trim().toLowerCase());
}

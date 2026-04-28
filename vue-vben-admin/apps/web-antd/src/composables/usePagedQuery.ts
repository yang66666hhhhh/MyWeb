import { reactive, ref, shallowRef } from 'vue';

import type { PageQuery, PageResult } from '#/api/growth';

export interface UsePagedQueryOptions<T, Q extends PageQuery> {
  defaultQuery: Q;
  fetcher: (query: Q) => Promise<PageResult<T>>;
}

export function usePagedQuery<T, Q extends PageQuery>({
  defaultQuery,
  fetcher,
}: UsePagedQueryOptions<T, Q>) {
  const loading = ref(false);
  const items = shallowRef<T[]>([]);
  const total = ref(0);
  const query = reactive({ ...defaultQuery }) as Q;

  async function load() {
    loading.value = true;
    try {
      const result = await fetcher({ ...query });
      items.value = result.items;
      total.value = result.total;
      query.page = result.page;
      query.pageSize = result.pageSize;
    } finally {
      loading.value = false;
    }
  }

  async function search() {
    query.page = 1;
    await load();
  }

  async function changePage(page: number, pageSize: number) {
    query.page = page;
    query.pageSize = pageSize;
    await load();
  }

  function resetQuery(nextQuery?: Partial<Q>) {
    Object.assign(query, defaultQuery, nextQuery);
  }

  return {
    changePage,
    items,
    load,
    loading,
    query,
    resetQuery,
    search,
    total,
  };
}

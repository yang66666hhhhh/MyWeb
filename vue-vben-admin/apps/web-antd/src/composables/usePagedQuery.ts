import { reactive, ref } from 'vue';

interface PagedResult<T> {
  items: T[];
  total: number;
}

interface PagedQueryOptions<T, Q> {
  defaultQuery: Q;
  fetcher: (query: Q) => Promise<PagedResult<T>>;
}

export function usePagedQuery<T, Q extends { page: number; pageSize: number }>(
  options: PagedQueryOptions<T, Q>
) {
  const { defaultQuery, fetcher } = options;

  const loading = ref(false);
  const items = ref<T[]>([]) as any;
  const total = ref(0);
  const query = reactive<Q>({ ...defaultQuery });

  async function load() {
    loading.value = true;
    try {
      const result = await fetcher(query);
      items.value = result.items;
      total.value = result.total;
    } catch {
      // Error handled by caller
    } finally {
      loading.value = false;
    }
  }

  function search() {
    query.page = 1;
    load();
  }

  function resetQuery() {
    Object.assign(query, { ...defaultQuery });
    load();
  }

  function changePage(page: number, pageSize?: number) {
    query.page = page;
    if (pageSize) {
      query.pageSize = pageSize;
    }
    load();
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

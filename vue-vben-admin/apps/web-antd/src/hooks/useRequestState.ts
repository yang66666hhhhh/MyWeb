import { ref } from 'vue';

interface RequestState {
  loading: boolean;
  error: Error | null;
  data: any;
}

export function useRequestState() {
  const states = ref(new Map<string, RequestState>());

  function getState(key: string): RequestState {
    if (!states.value.has(key)) {
      states.value.set(key, {
        loading: false,
        error: null,
        data: null,
      });
    }
    return states.value.get(key)!;
  }

  function setLoading(key: string, loading: boolean) {
    getState(key).loading = loading;
  }

  function setError(key: string, error: Error | null) {
    getState(key).error = error;
  }

  function setData(key: string, data: any) {
    getState(key).data = data;
  }

  function reset(key: string) {
    states.value.set(key, {
      loading: false,
      error: null,
      data: null,
    });
  }

  function resetAll() {
    states.value.clear();
  }

  async function execute<T>(key: string, fn: () => Promise<T>): Promise<T> {
    const state = getState(key);
    state.loading = true;
    state.error = null;

    try {
      const result = await fn();
      state.data = result;
      return result;
    } catch (error) {
      state.error = error as Error;
      throw error;
    } finally {
      state.loading = false;
    }
  }

  return {
    getState,
    setLoading,
    setError,
    setData,
    reset,
    resetAll,
    execute,
  };
}

export function useAsyncRequest<T>(key: string, fn: () => Promise<T>) {
  const { getState, execute } = useRequestState();
  const state = getState(key);

  async function run() {
    return execute(key, fn);
  }

  return {
    loading: ref(state.loading),
    error: ref(state.error),
    data: ref(state.data),
    run,
  };
}

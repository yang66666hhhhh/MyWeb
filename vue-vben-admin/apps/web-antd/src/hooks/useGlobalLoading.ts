import { computed, ref } from 'vue';

interface LoadingState {
  [key: string]: boolean;
}

const globalLoading = ref<LoadingState>({});
const loadingCount = ref(0);

export function useGlobalLoading() {
  const isLoading = computed(() => loadingCount.value > 0);

  const loadingText = computed(() => {
    const keys = Object.keys(globalLoading.value).filter(
      (k) => globalLoading.value[k],
    );
    if (keys.length === 0) return '';
    if (keys.length === 1) return keys[0];
    return `加载中 (${keys.length})...`;
  });

  function startLoading(key: string) {
    if (!globalLoading.value[key]) {
      globalLoading.value[key] = true;
      loadingCount.value++;
    }
  }

  function stopLoading(key: string) {
    if (globalLoading.value[key]) {
      globalLoading.value[key] = false;
      loadingCount.value--;
      delete globalLoading.value[key];
    }
  }

  function isLoadingKey(key: string): boolean {
    return globalLoading.value[key] || false;
  }

  async function withLoading<T>(key: string, fn: () => Promise<T>): Promise<T> {
    startLoading(key);
    try {
      return await fn();
    } finally {
      stopLoading(key);
    }
  }

  function clearAll() {
    globalLoading.value = {};
    loadingCount.value = 0;
  }

  return {
    isLoading,
    loadingText,
    startLoading,
    stopLoading,
    isLoadingKey,
    withLoading,
    clearAll,
  };
}

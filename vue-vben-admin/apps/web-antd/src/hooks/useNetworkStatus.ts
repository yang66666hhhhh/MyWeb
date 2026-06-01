import { computed, onMounted, onUnmounted, ref } from 'vue';

import { message } from 'ant-design-vue';

export function useNetworkStatus() {
  const isOnline = ref(navigator.onLine);
  const lastOnlineTime = ref<Date | null>(null);
  const lastOfflineTime = ref<Date | null>(null);
  const reconnectAttempts = ref(0);
  const isReconnecting = ref(false);

  let reconnectTimer: ReturnType<typeof setInterval> | null = null;

  const statusText = computed(() => {
    if (isReconnecting.value) return '重新连接中...';
    return isOnline.value ? '已连接' : '已断开';
  });

  const statusColor = computed(() => {
    if (isReconnecting.value) return 'warning';
    return isOnline.value ? 'success' : 'error';
  });

  function handleOnline() {
    if (!isOnline.value) {
      isOnline.value = true;
      lastOnlineTime.value = new Date();
      reconnectAttempts.value = 0;
      isReconnecting.value = false;
      message.success('网络已恢复');
    }
  }

  function handleOffline() {
    isOnline.value = false;
    lastOfflineTime.value = new Date();
    message.warning('网络已断开，正在尝试重连...');
    startReconnect();
  }

  function startReconnect() {
    if (reconnectTimer) {
      clearInterval(reconnectTimer);
    }

    isReconnecting.value = true;
    reconnectTimer = setInterval(() => {
      reconnectAttempts.value++;

      // 检查网络状态
      if (navigator.onLine) {
        handleOnline();
        stopReconnect();
      }

      // 超过最大重试次数
      if (reconnectAttempts.value >= 10) {
        stopReconnect();
        message.error('网络重连失败，请检查网络设置');
      }
    }, 5000);
  }

  function stopReconnect() {
    if (reconnectTimer) {
      clearInterval(reconnectTimer);
      reconnectTimer = null;
    }
    isReconnecting.value = false;
  }

  function checkConnection(): boolean {
    return navigator.onLine;
  }

  onMounted(() => {
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    // 初始检查
    if (!navigator.onLine) {
      handleOffline();
    }
  });

  onUnmounted(() => {
    window.removeEventListener('online', handleOnline);
    window.removeEventListener('offline', handleOffline);
    stopReconnect();
  });

  return {
    isOnline,
    lastOnlineTime,
    lastOfflineTime,
    reconnectAttempts,
    isReconnecting,
    statusText,
    statusColor,
    checkConnection,
  };
}

// 请求取消管理器
const pendingRequests = new Map<string, AbortController>();

export function getRequestKey(url: string, method: string): string {
  return `${method}:${url}`;
}

export function createCancelToken(key: string): AbortController {
  // 取消之前的相同请求
  cancelRequest(key);
  
  const controller = new AbortController();
  pendingRequests.set(key, controller);
  return controller;
}

export function cancelRequest(key: string): void {
  const controller = pendingRequests.get(key);
  if (controller) {
    controller.abort();
    pendingRequests.delete(key);
  }
}

export function cancelAllRequests(): void {
  pendingRequests.forEach((controller) => controller.abort());
  pendingRequests.clear();
}

// 页面卸载时取消所有请求
if (typeof window !== 'undefined') {
  window.addEventListener('beforeunload', cancelAllRequests);
}

<script lang="ts" setup>
import { onErrorCaptured, ref } from 'vue';

import { Button, Result } from 'ant-design-vue';

interface Props {
  fallbackTitle?: string;
  fallbackSubtitle?: string;
}

const props = withDefaults(defineProps<Props>(), {
  fallbackTitle: '页面出错了',
  fallbackSubtitle: '请刷新页面或联系管理员',
});

const hasError = ref(false);
const errorMessage = ref('');

onErrorCaptured((err, _instance, info) => {
  hasError.value = true;
  errorMessage.value = err.message || '未知错误';
  console.error('ErrorBoundary caught:', err, info);
  return false; // 阻止错误向上传播
});

function handleRetry() {
  hasError.value = false;
  errorMessage.value = '';
}

function handleReload() {
  globalThis.location.reload();
}
</script>

<template>
  <div v-if="hasError" class="flex items-center justify-center p-8">
    <Result
      status="error"
      :title="props.fallbackTitle"
      :sub-title="errorMessage || props.fallbackSubtitle"
    >
      <template #extra>
        <Button type="primary" @click="handleRetry">重试</Button>
        <Button @click="handleReload">刷新页面</Button>
      </template>
    </Result>
  </div>
  <slot v-else />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';

import { Button, notification } from 'ant-design-vue';

interface BeforeInstallPromptEvent extends Event {
  prompt(): Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
}

const deferredPrompt = ref<BeforeInstallPromptEvent | null>(null);
const showInstallPrompt = ref(false);

onMounted(() => {
  window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt.value = e as BeforeInstallPromptEvent;
    showInstallPrompt.value = true;
  });
});

async function handleInstall() {
  if (!deferredPrompt.value) return;

  deferredPrompt.value.prompt();
  const { outcome } = await deferredPrompt.value.userChoice;

  if (outcome === 'accepted') {
    notification.success({
      message: '安装成功',
      description: '应用已添加到主屏幕',
    });
  }

  deferredPrompt.value = null;
  showInstallPrompt.value = false;
}

function handleDismiss() {
  showInstallPrompt.value = false;
}
</script>

<template>
  <div
    v-if="showInstallPrompt"
    class="fixed bottom-4 left-4 right-4 z-50 rounded-lg bg-white p-4 shadow-lg dark:bg-gray-800 md:left-auto md:right-4 md:w-80"
  >
    <div class="mb-3 flex items-center gap-3">
      <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-blue-500">
        <span class="text-2xl">📱</span>
      </div>
      <div>
        <h3 class="font-semibold">安装应用</h3>
        <p class="text-sm text-gray-500">添加到主屏幕，获得更好的体验</p>
      </div>
    </div>
    <div class="flex gap-2">
      <Button type="primary" block @click="handleInstall">
        安装
      </Button>
      <Button @click="handleDismiss">
        稍后
      </Button>
    </div>
  </div>
</template>

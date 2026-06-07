# PWA & Mobile Optimization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Integrate PWA support and optimize mobile experience for the personal growth management system.

**Architecture:** Leverage the existing vite-plugin-pwa infrastructure (already configured in internal/vite-config) and add mobile-specific composables, styles, and component optimizations. The PWA config in `src/pwa.ts` will be activated by passing it to the vite config. Mobile optimizations use Tailwind CSS responsive classes and a new `useIsMobile` composable.

**Tech Stack:** Vue 3, Vite, vite-plugin-pwa, Tailwind CSS, Ant Design Vue, @vueuse/core

---

## File Structure

### Files to Create
- `apps/web-antd/src/composables/useIsMobile.ts` — Mobile detection composable
- `apps/web-antd/src/styles/mobile.css` — Mobile-specific styles
- `apps/web-antd/public/pwa-192x192.png` — PWA icon 192px
- `apps/web-antd/public/pwa-512x512.png` — PWA icon 512px
- `apps/web-antd/public/apple-touch-icon.png` — Apple touch icon
- `apps/web-antd/public/masked-icon.svg` — Masked icon for PWA

### Files to Modify
- `apps/web-antd/vite.config.ts` — Add PWA config import
- `apps/web-antd/src/main.ts` — Register PWA
- `apps/web-antd/index.html` — Add PWA meta tags
- `apps/web-antd/src/styles/utilities.css` — Add mobile utility classes
- `apps/web-antd/src/layouts/basic.vue` — Mobile sidebar drawer

---

### Task 1: Create PWA Icon Assets

**Files:**
- Create: `apps/web-antd/public/pwa-192x192.png`
- Create: `apps/web-antd/public/pwa-512x512.png`
- Create: `apps/web-antd/public/apple-touch-icon.png`
- Create: `apps/web-antd/public/masked-icon.svg`

- [ ] **Step 1: Create placeholder PWA icons using SVG**

Create a simple SVG icon that can be used as a base. For now, create placeholder files that reference existing icons or generate simple colored squares.

```bash
# Create a simple SVG for the masked icon
```

Write `apps/web-antd/public/masked-icon.svg`:
```svg
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
  <rect width="512" height="512" rx="64" fill="#1890ff"/>
  <text x="256" y="280" font-size="200" font-family="Arial" font-weight="bold" fill="white" text-anchor="middle">G</text>
</svg>
```

- [ ] **Step 2: Generate PNG icons from SVG**

Note: In a real implementation, you would use a tool like `sharp` or an online converter. For now, copy the existing favicon or create a note to generate proper icons.

```bash
# Copy favicon as placeholder for PWA icons
Copy-Item "apps/web-antd/public/favicon.ico" "apps/web-antd/public/pwa-192x192.png" -ErrorAction SilentlyContinue
Copy-Item "apps/web-antd/public/favicon.ico" "apps/web-antd/public/pwa-512x512.png" -ErrorAction SilentlyContinue
Copy-Item "apps/web-antd/public/favicon.ico" "apps/web-antd/public/apple-touch-icon.png" -ErrorAction SilentlyContinue
```

- [ ] **Step 3: Commit**

```bash
git add apps/web-antd/public/
git commit -m "feat: add PWA icon placeholders"
```

---

### Task 2: Configure PWA in Vite

**Files:**
- Modify: `apps/web-antd/vite.config.ts:1-25`

- [ ] **Step 1: Update vite.config.ts to import and use PWA config**

The existing `src/pwa.ts` contains the PWA configuration but it's not being used. The internal vite-config already supports PWA via `pwa: true` and `pwaOptions`. We need to pass our custom config.

```typescript
import { defineConfig } from '@vben/vite-config';
import { pwaConfig } from './src/pwa';

const createApiProxy = (target: string) => ({
  changeOrigin: true,
  rewrite: (path: string) => path.replace(/^\/api/, ''),
  target,
  ws: true,
});

const dotnetApi = createApiProxy('http://localhost:5062/api');

export default defineConfig(async () => {
  return {
    application: {
      nitroMock: false,
      pwa: true,
      pwaOptions: pwaConfig,
    },
    vite: {
      server: {
        proxy: {
          '/api': dotnetApi,
        },
      },
    },
  };
});
```

- [ ] **Step 2: Verify build works**

Run: `pnpm build` in `apps/web-antd/`
Expected: Build succeeds with PWA files generated

- [ ] **Step 3: Commit**

```bash
git add apps/web-antd/vite.config.ts
git commit -m "feat: activate PWA configuration in vite"
```

---

### Task 3: Add PWA Meta Tags to index.html

**Files:**
- Modify: `apps/web-antd/index.html:1-35`

- [ ] **Step 1: Add PWA meta tags**

```html
<!doctype html>
<html lang="zh">
  <head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="renderer" content="webkit" />
    <meta name="description" content="个人成长与工作管理系统" />
    <meta name="keywords" content="Vben Admin Vue3 Vite" />
    <meta name="author" content="Vben" />
    <meta
      name="viewport"
      content="width=device-width,initial-scale=1.0,minimum-scale=1.0,maximum-scale=1.0,user-scalable=0,viewport-fit=cover"
    />
    <meta name="theme-color" content="#1890ff" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black-translucent" />
    <meta name="apple-mobile-web-app-title" content="GrowthApp" />
    <link rel="apple-touch-icon" href="/apple-touch-icon.png" />
    <link rel="manifest" href="/manifest.webmanifest" />
    <!-- 由 vite 注入 VITE_APP_TITLE 变量，在 .env 文件内配置 -->
    <title>%VITE_APP_TITLE%</title>
    <link rel="icon" href="/favicon.ico" />
    <script>
      // 生产环境下注入百度统计
      if (window._VBEN_ADMIN_PRO_APP_CONF_) {
        var _hmt = _hmt || [];
        (function () {
          var hm = document.createElement('script');
          hm.src =
            'https://hm.baidu.com/hm.js?b38e689f40558f20a9a686d7f6f33edf';
          var s = document.getElementsByTagName('script')[0];
          s.parentNode.insertBefore(hm, s);
        })();
      }
    </script>
  </head>
  <body>
    <div id="app"></div>
    <script type="module" src="/src/main.ts"></script>
  </body>
</html>
```

- [ ] **Step 2: Commit**

```bash
git add apps/web-antd/index.html
git commit -m "feat: add PWA meta tags to index.html"
```

---

### Task 4: Create useIsMobile Composable

**Files:**
- Create: `apps/web-antd/src/composables/useIsMobile.ts`

- [ ] **Step 1: Create the composable**

```typescript
import { computed, onMounted, onUnmounted } from 'vue';

import { useWindowSize } from '@vueuse/core';

const MOBILE_BREAKPOINT = 768;

export function useIsMobile() {
  const { width } = useWindowSize();

  const isMobile = computed(() => width.value < MOBILE_BREAKPOINT);

  return {
    isMobile,
    screenWidth: width,
  };
}
```

- [ ] **Step 2: Commit**

```bash
git add apps/web-antd/src/composables/useIsMobile.ts
git commit -m "feat: add useIsMobile composable"
```

---

### Task 5: Add Mobile Styles

**Files:**
- Create: `apps/web-antd/src/styles/mobile.css`
- Modify: `apps/web-antd/src/styles/utilities.css:181-188`

- [ ] **Step 1: Create mobile.css**

```css
/* Mobile-specific styles */

/* Minimum touch target size (44px) */
@media (max-width: 768px) {
  /* Buttons */
  .ant-btn {
    min-height: 44px;
    min-width: 44px;
  }

  /* Input fields */
  .ant-input,
  .ant-select-selector,
  .ant-picker {
    min-height: 44px;
  }

  /* List items */
  .ant-list-item {
    padding: 16px;
  }

  /* Table responsive */
  .ant-table-wrapper {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }

  /* Modal fullscreen on mobile */
  .ant-modal {
    max-width: 100vw;
    margin: 0;
    padding: 0;
  }

  .ant-modal-content {
    border-radius: 0;
    min-height: 100vh;
  }

  /* Form full width */
  .ant-form-item .ant-input,
  .ant-form-item .ant-select,
  .ant-form-item .ant-picker {
    width: 100%;
  }

  /* Card adjustments */
  .ant-card {
    border-radius: 8px;
  }

  .ant-card-body {
    padding: 16px;
  }

  /* Safe area padding for iOS */
  .safe-area-bottom {
    padding-bottom: env(safe-area-inset-bottom);
  }

  .safe-area-top {
    padding-top: env(safe-area-inset-top);
  }
}
```

- [ ] **Step 2: Update utilities.css with enhanced mobile utilities**

Add after the existing responsive section (line 188):

```css
/* Enhanced Mobile utilities */
@media (max-width: 768px) {
  .mobile-p-0 { padding: 0; }
  .mobile-p-2 { padding: 8px; }
  .mobile-p-4 { padding: 16px; }
  .mobile-m-0 { margin: 0; }
  .mobile-mt-4 { margin-top: 16px; }
  .mobile-mb-4 { margin-bottom: 16px; }
  .mobile-gap-2 { gap: 8px; }
  .mobile-gap-4 { gap: 16px; }
  .mobile-text-sm { font-size: 14px; }
  .mobile-text-base { font-size: 16px; }
  .mobile-w-full { width: 100%; }
  .mobile-flex-col { flex-direction: column; }
}
```

- [ ] **Step 3: Commit**

```bash
git add apps/web-antd/src/styles/
git commit -m "feat: add mobile-specific styles and utilities"
```

---

### Task 6: Update Layout for Mobile Sidebar

**Files:**
- Modify: `apps/web-antd/src/layouts/basic.vue:1-297`

- [ ] **Step 1: Add mobile detection to basic.vue**

The `BasicLayout` component from `@vben/layouts` already handles sidebar behavior. We need to ensure it detects mobile and switches to drawer mode. Add the import and usage:

```vue
<script lang="ts" setup>
import type { NotificationItem } from '@vben/layouts';

import { computed, onMounted, onUnmounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';

import { AuthenticationLoginExpiredModal } from '@vben/common-ui';
import { VBEN_DOC_URL, VBEN_GITHUB_URL } from '@vben/constants';
import { useWatermark } from '@vben/hooks';
import { BookOpenText, CircleHelp, SvgGithubIcon, UserRoundPen } from '@vben/icons';
import {
  BasicLayout,
  LockScreen,
  Notification,
  UserDropdown,
} from '@vben/layouts';
import { preferences } from '@vben/preferences';
import { useAccessStore, useUserStore } from '@vben/stores';
import { openWindow } from '@vben/utils';

import { notificationApi } from '#/api/notification';
import { $t } from '#/locales';
import { useAuthStore, usePersonaStore } from '#/store';
import LoginForm from '#/views/_core/authentication/login.vue';
import { useIsMobile } from '#/composables/useIsMobile';

const { isMobile } = useIsMobile();

// ... rest of the script remains the same
</script>
```

The `BasicLayout` component from `@vben/layouts` should already support mobile detection internally. If it doesn't, we would need to pass a prop. Let's check if the component accepts a mobile prop or uses a composable internally.

- [ ] **Step 2: Verify BasicLayout mobile support**

Check if `@vben/layouts` has built-in mobile support. If not, we may need to wrap the sidebar in a drawer manually.

- [ ] **Step 3: Commit**

```bash
git add apps/web-antd/src/layouts/basic.vue
git commit -m "feat: add mobile detection to layout"
```

---

### Task 7: Register PWA in main.ts

**Files:**
- Modify: `apps/web-antd/src/main.ts:1-32`

- [ ] **Step 1: Add PWA registration**

The vite-plugin-pwa handles registration automatically when `injectRegister` is configured. However, we can add a manual registration for better control. Add after the imports:

```typescript
import { initPreferences } from '@vben/preferences';
import { unmountGlobalLoading } from '@vben/utils';

import { overridesPreferences, preferencesExtension } from './preferences';

// PWA Registration
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js').catch(() => {
      // SW registration failed, app still works
    });
  });
}

/**
 * 应用初始化完成之后再进行页面加载渲染
 */
async function initApplication() {
  // name用于指定项目唯一标识
  // 用于区分不同项目的偏好设置以及存储数据的key前缀以及其他一些需要隔离的数据
  const env = import.meta.env.PROD ? 'prod' : 'dev';
  const appVersion = import.meta.env.VITE_APP_VERSION;
  const namespace = `${import.meta.env.VITE_APP_NAMESPACE}-${appVersion}-${env}`;

  // app偏好设置初始化
  await initPreferences({
    extension: preferencesExtension,
    namespace,
    overrides: overridesPreferences,
  });

  // 启动应用并挂载
  // vue应用主要逻辑及视图
  const { bootstrap } = await import('./bootstrap');
  await bootstrap(namespace);

  // 移除并销毁loading
  unmountGlobalLoading();
}

initApplication();
```

- [ ] **Step 2: Commit**

```bash
git add apps/web-antd/src/main.ts
git commit -m "feat: add PWA service worker registration"
```

---

### Task 8: Update PWA Config with Better Cache Strategies

**Files:**
- Modify: `apps/web-antd/src/pwa.ts:1-77`

- [ ] **Step 1: Enhance PWA configuration**

```typescript
// PWA 配置
export const pwaConfig = {
  registerType: 'autoUpdate' as const,
  includeAssets: ['favicon.ico', 'apple-touch-icon.png', 'masked-icon.svg'],
  manifest: {
    name: 'Personal Growth Management',
    short_name: 'GrowthApp',
    description: '个人成长与工作管理系统',
    theme_color: '#1890ff',
    background_color: '#ffffff',
    display: 'standalone',
    scope: '/',
    start_url: '/',
    orientation: 'portrait-primary',
    categories: ['productivity', 'lifestyle'],
    icons: [
      {
        src: 'pwa-192x192.png',
        sizes: '192x192',
        type: 'image/png',
      },
      {
        src: 'pwa-512x512.png',
        sizes: '512x512',
        type: 'image/png',
      },
      {
        src: 'pwa-512x512.png',
        sizes: '512x512',
        type: 'image/png',
        purpose: 'any maskable',
      },
    ],
  },
  workbox: {
    globPatterns: ['**/*.{js,css,html,ico,png,svg,woff2}'],
    runtimeCaching: [
      {
        urlPattern: /^https:\/\/api.*/i,
        handler: 'NetworkFirst',
        options: {
          cacheName: 'api-cache',
          expiration: {
            maxEntries: 100,
            maxAgeSeconds: 60 * 60 * 24, // 24 hours
          },
          cacheableResponse: {
            statuses: [0, 200],
          },
        },
      },
      {
        urlPattern: /\.(?:png|jpg|jpeg|svg|gif|webp)$/,
        handler: 'CacheFirst',
        options: {
          cacheName: 'images-cache',
          expiration: {
            maxEntries: 100,
            maxAgeSeconds: 60 * 60 * 24 * 30, // 30 days
          },
        },
      },
      {
        urlPattern: /\.(?:js|css)$/,
        handler: 'StaleWhileRevalidate',
        options: {
          cacheName: 'static-cache',
          expiration: {
            maxEntries: 100,
            maxAgeSeconds: 60 * 60 * 24 * 7, // 7 days
          },
        },
      },
      {
        urlPattern: /\.(?:woff2|ttf|eot)$/,
        handler: 'CacheFirst',
        options: {
          cacheName: 'fonts-cache',
          expiration: {
            maxEntries: 30,
            maxAgeSeconds: 60 * 60 * 24 * 365, // 1 year
          },
        },
      },
    ],
    navigateFallback: '/index.html',
    navigateFallbackDenylist: [/^\/api/],
  },
  devOptions: {
    enabled: false,
  },
};
```

- [ ] **Step 2: Commit**

```bash
git add apps/web-antd/src/pwa.ts
git commit -m "feat: enhance PWA config with better caching and orientation"
```

---

### Task 9: Create Install Prompt Component

**Files:**
- Create: `apps/web-antd/src/components/PwaInstallPrompt.vue`

- [ ] **Step 1: Create the install prompt component**

```vue
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
```

- [ ] **Step 2: Add component to app.vue**

```vue
<script lang="ts" setup>
import { computed } from 'vue';

import { useAntdDesignTokens } from '@vben/hooks';
import { preferences, usePreferences } from '@vben/preferences';

import { App, ConfigProvider, theme } from 'ant-design-vue';

import { antdLocale } from '#/locales';
import PwaInstallPrompt from '#/components/PwaInstallPrompt.vue';

defineOptions({ name: 'App' });

const { isDark } = usePreferences();
const { tokens } = useAntdDesignTokens();

const tokenTheme = computed(() => {
  const algorithm = isDark.value
    ? [theme.darkAlgorithm]
    : [theme.defaultAlgorithm];

  // antd 紧凑模式算法
  if (preferences.app.compact) {
    algorithm.push(theme.compactAlgorithm);
  }

  return {
    algorithm,
    token: tokens,
  };
});
</script>

<template>
  <ConfigProvider :locale="antdLocale" :theme="tokenTheme">
    <App>
      <RouterView />
      <PwaInstallPrompt />
    </App>
  </ConfigProvider>
</template>
```

- [ ] **Step 3: Commit**

```bash
git add apps/web-antd/src/components/PwaInstallPrompt.vue apps/web-antd/src/app.vue
git commit -m "feat: add PWA install prompt component"
```

---

### Task 10: Add Mobile-Responsive Table Wrapper

**Files:**
- Create: `apps/web-antd/src/components/MobileTable.vue`

- [ ] **Step 1: Create responsive table wrapper**

```vue
<script setup lang="ts">
import { useIsMobile } from '#/composables/useIsMobile';

const { isMobile } = useIsMobile();
</script>

<template>
  <div :class="{ 'table-responsive': isMobile }">
    <slot />
  </div>
</template>
```

- [ ] **Step 2: Commit**

```bash
git add apps/web-antd/src/components/MobileTable.vue
git commit -m "feat: add mobile-responsive table wrapper component"
```

---

### Task 11: Verify Build and Test

**Files:**
- None (verification only)

- [ ] **Step 1: Run type check**

Run: `pnpm typecheck` in `apps/web-antd/`
Expected: No type errors

- [ ] **Step 2: Run build**

Run: `pnpm build` in `apps/web-antd/`
Expected: Build succeeds, PWA files generated in dist/

- [ ] **Step 3: Verify PWA files in dist**

```bash
ls apps/web-antd/dist/ | Select-String "sw|workbox|manifest"
```

Expected: Service worker and manifest files present

- [ ] **Step 4: Final commit**

```bash
git add -A
git commit -m "feat: complete PWA integration and mobile optimization"
```

---

## Summary

This plan implements:
1. **PWA Support** — Leverages existing vite-plugin-pwa infrastructure, adds proper icons, meta tags, and install prompt
2. **Mobile Detection** — New `useIsMobile` composable using @vueuse/core
3. **Mobile Styles** — Touch-friendly sizes (44px min), responsive tables, fullscreen modals
4. **Performance** — Route lazy loading already enabled, font caching added

All changes are backward-compatible and don't affect desktop experience.

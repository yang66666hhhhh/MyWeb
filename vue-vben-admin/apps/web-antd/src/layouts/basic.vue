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

const notifications = ref<NotificationItem[]>([]);
const unreadCount = ref(0);
let pollTimer: ReturnType<typeof setInterval> | null = null;

const typeAvatars: Record<string, string> = {
  System: 'https://avatar.vercel.sh/system?text=SY',
  Task: 'https://avatar.vercel.sh/task?text=TK',
  Habit: 'https://avatar.vercel.sh/habit?text=HB',
  Ai: 'https://avatar.vercel.sh/ai?text=AI',
  Finance: 'https://avatar.vercel.sh/finance?text=FN',
};

function formatTime(dateStr: string): string {
  const date = new Date(dateStr);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(diff / 3600000);
  const days = Math.floor(diff / 86400000);

  if (minutes < 1) return '刚刚';
  if (minutes < 60) return `${minutes}分钟前`;
  if (hours < 24) return `${hours}小时前`;
  if (days < 7) return `${days}天前`;
  return date.toLocaleDateString('zh-CN');
}

async function fetchNotifications() {
  try {
    const result = await notificationApi.getPage({ page: 1, pageSize: 10 });
    notifications.value = result.items.map((n) => ({
      id: n.id,
      avatar: typeAvatars[n.type] || 'https://avatar.vercel.sh/default?text=N',
      date: formatTime(n.createdAt),
      isRead: n.isRead,
      message: n.content || '',
      title: n.title,
      link: n.link,
    }));
  } catch {
    // silent fail
  }
}

async function fetchUnreadCount() {
  try {
    const result = await notificationApi.getUnreadCount();
    unreadCount.value = result.count;
  } catch {
    // silent fail
  }
}

const router = useRouter();
const userStore = useUserStore();
const authStore = useAuthStore();
const personaStore = usePersonaStore();
const accessStore = useAccessStore();
const { destroyWatermark, updateWatermark } = useWatermark();
const showDot = computed(() => unreadCount.value > 0);

const menus = computed(() => [
  {
    handler: () => {
      router.push({ name: 'Profile' });
    },
    icon: UserRoundPen,
    text: $t('page.auth.profile'),
  },
  {
    handler: () => {
      openWindow(VBEN_DOC_URL, {
        target: '_blank',
      });
    },
    icon: BookOpenText,
    text: $t('ui.widgets.document'),
  },
  {
    handler: () => {
      openWindow(VBEN_GITHUB_URL, {
        target: '_blank',
      });
    },
    icon: SvgGithubIcon,
    text: 'GitHub',
  },
  {
    handler: () => {
      openWindow(`${VBEN_GITHUB_URL}/issues`, {
        target: '_blank',
      });
    },
    icon: CircleHelp,
    text: $t('ui.widgets.qa'),
  },
]);

const avatar = computed(() => {
  return userStore.userInfo?.avatar ?? preferences.app.defaultAvatar;
});

const personaDisplay = computed(() => personaStore.personaDisplay);

const tagText = computed(() => {
  const role = userStore.userInfo?.roles?.[0];
  if (!role) return '';
  const roleMap: Record<string, string> = {
    owner: 'Owner',
    pro: 'Pro',
    member: 'Member',
  };
  return roleMap[role.toLowerCase()] || role.toUpperCase();
});

async function handleLogout() {
  await authStore.logout(false);
}

function handleNoticeClear() {
  notifications.value = [];
}

async function markRead(id: number | string) {
  try {
    await notificationApi.markAsRead(id as string);
    const item = notifications.value.find((item) => item.id === id);
    if (item) {
      item.isRead = true;
    }
    unreadCount.value = Math.max(0, unreadCount.value - 1);
  } catch {
    // silent
  }
}

async function remove(id: number | string) {
  try {
    await notificationApi.delete(id as string);
    const item = notifications.value.find((n) => n.id === id);
    notifications.value = notifications.value.filter((item) => item.id !== id);
    if (item && !item.isRead) {
      unreadCount.value = Math.max(0, unreadCount.value - 1);
    }
  } catch {
    // silent
  }
}

async function handleMakeAll() {
  try {
    await notificationApi.markAllAsRead();
    notifications.value.forEach((item) => (item.isRead = true));
    unreadCount.value = 0;
  } catch {
    // silent
  }
}

const viewAll = () => {
  router.push({ name: 'Profile', query: { tab: 'notifications' } });
};

const handleClick = (item: NotificationItem) => {
  if (item.link) {
    navigateTo(item.link, item.query, item.state);
  }
};

function navigateTo(
  link: string,
  query?: Record<string, any>,
  state?: Record<string, any>,
) {
  if (link.startsWith('http://') || link.startsWith('https://')) {
    window.open(link, '_blank');
  } else {
    router.push({
      path: link,
      query: query || {},
      state,
    });
  }
}

onMounted(() => {
  if (!personaStore.initialized) {
    personaStore.loadPersonaData();
  }
  fetchNotifications();
  fetchUnreadCount();
  pollTimer = setInterval(() => {
    fetchUnreadCount();
    fetchNotifications();
  }, 30000);
});

onUnmounted(() => {
  if (pollTimer) {
    clearInterval(pollTimer);
  }
});

watch(
  () => ({
    enable: preferences.app.watermark,
    content: preferences.app.watermarkContent,
  }),
  async ({ enable, content }) => {
    if (enable) {
      await updateWatermark({
        content:
          content ||
          `${userStore.userInfo?.username} - ${userStore.userInfo?.realName}`,
      });
    } else {
      destroyWatermark();
    }
  },
  {
    immediate: true,
  },
);
</script>

<template>
  <BasicLayout @clear-preferences-and-logout="handleLogout">
    <template #user-dropdown>
      <div class="flex items-center gap-2">
        <!-- 身份标识 -->
        <div
          v-if="personaDisplay"
          class="flex items-center gap-1 rounded-full bg-blue-50 px-3 py-1 text-sm text-blue-600 dark:bg-blue-900/30 dark:text-blue-400"
        >
          <span>{{ personaDisplay.icon }}</span>
          <span>{{ personaDisplay.name }}</span>
        </div>
        <UserDropdown
          :avatar
          :menus
          :text="userStore.userInfo?.realName"
          :description="userStore.userInfo?.username"
          :tag-text="tagText"
          @logout="handleLogout"
        />
      </div>
    </template>
    <template #notification>
      <Notification
        :dot="showDot"
        :notifications="notifications"
        @clear="handleNoticeClear"
        @read="(item) => item.id && markRead(item.id)"
        @remove="(item) => item.id && remove(item.id)"
        @make-all="handleMakeAll"
        @on-click="handleClick"
        @view-all="viewAll"
      />
    </template>
    <template #extra>
      <AuthenticationLoginExpiredModal
        v-model:open="accessStore.loginExpired"
        :avatar
      >
        <LoginForm />
      </AuthenticationLoginExpiredModal>
    </template>
    <template #lock-screen>
      <LockScreen :avatar @to-login="handleLogout" />
    </template>
  </BasicLayout>
</template>

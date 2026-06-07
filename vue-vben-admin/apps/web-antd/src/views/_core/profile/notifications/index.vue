<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { useRouter } from 'vue-router';

import { MailCheck, Search, X } from '@vben/icons';

import {
  Avatar,
  Badge,
  Button,
  Card,
  Input,
  List,
  message,
  Select,
  Tag,
} from 'ant-design-vue';

import type { NotificationItem } from '#/api/notification';
import { notificationApi } from '#/api/notification';

const router = useRouter();
const loading = ref(false);
const notifications = ref<NotificationItem[]>([]);
const total = ref(0);
const page = ref(1);
const pageSize = ref(20);
const activeTab = ref('all');
const searchText = ref('');
const typeFilter = ref<string | undefined>(undefined);
const unreadCount = ref(0);
let pollTimer: ReturnType<typeof setInterval> | null = null;

const typeColors: Record<string, string> = {
  System: 'blue',
  Task: 'green',
  Habit: 'orange',
  Ai: 'purple',
  Finance: 'gold',
};

const typeLabels: Record<string, string> = {
  System: '系统',
  Task: '任务',
  Habit: '习惯',
  Ai: 'AI',
  Finance: '财务',
};

const typeIcons: Record<string, string> = {
  System: '📢',
  Task: '✅',
  Habit: '🔄',
  Ai: '🤖',
  Finance: '💰',
};

const tabItems = computed(() => [
  { key: 'all', label: '全部' },
  { key: 'unread', label: `未读${unreadCount.value > 0 ? `(${unreadCount.value})` : ''}` },
  { key: 'read', label: '已读' },
]);

async function fetchNotifications() {
  loading.value = true;
  try {
    const params: Record<string, any> = {
      page: page.value,
      pageSize: pageSize.value,
    };
    if (activeTab.value === 'unread') {
      params.isRead = false;
    } else if (activeTab.value === 'read') {
      params.isRead = true;
    }
    if (typeFilter.value) {
      params.type = typeFilter.value;
    }
    if (searchText.value.trim()) {
      params.keyword = searchText.value.trim();
    }

    const result = await notificationApi.getPage(params);
    notifications.value = result.items;
    total.value = result.total;
  } catch {
    message.error('获取通知列表失败');
  } finally {
    loading.value = false;
  }
}

async function fetchUnreadCount() {
  try {
    const result = await notificationApi.getUnreadCount();
    unreadCount.value = result.count;
  } catch {
    // silent
  }
}

async function handleMarkAsRead(item: NotificationItem) {
  try {
    await notificationApi.markAsRead(item.id);
    item.isRead = true;
    item.readAt = new Date().toISOString();
    unreadCount.value = Math.max(0, unreadCount.value - 1);
    message.success('已标记为已读');
  } catch {
    message.error('操作失败');
  }
}

async function handleMarkAllAsRead() {
  try {
    await notificationApi.markAllAsRead();
    notifications.value.forEach((n) => {
      n.isRead = true;
    });
    unreadCount.value = 0;
    message.success('已全部标记为已读');
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(item: NotificationItem) {
  try {
    await notificationApi.delete(item.id);
    notifications.value = notifications.value.filter((n) => n.id !== item.id);
    total.value--;
    if (!item.isRead) {
      unreadCount.value = Math.max(0, unreadCount.value - 1);
    }
    message.success('已删除');
  } catch {
    message.error('删除失败');
  }
}

function handleClick(item: NotificationItem) {
  if (!item.isRead) {
    handleMarkAsRead(item);
  }
  if (item.link) {
    if (item.link.startsWith('http://') || item.link.startsWith('https://')) {
      window.open(item.link, '_blank');
    } else {
      router.push(item.link);
    }
  }
}

function handleTabChange(key: string) {
  activeTab.value = key;
  page.value = 1;
  fetchNotifications();
}

function handleTypeChange(value: any) {
  typeFilter.value = value as string | undefined;
  page.value = 1;
  fetchNotifications();
}

function handleSearch() {
  page.value = 1;
  fetchNotifications();
}

function handlePageChange(p: number, ps: number) {
  page.value = p;
  pageSize.value = ps;
  fetchNotifications();
}

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

onMounted(() => {
  fetchNotifications();
  fetchUnreadCount();
  pollTimer = setInterval(fetchUnreadCount, 30000);
});

onUnmounted(() => {
  if (pollTimer) {
    clearInterval(pollTimer);
  }
});
</script>

<template>
  <div class="mx-auto max-w-4xl p-4">
    <Card>
      <template #title>
        <div class="flex items-center gap-2">
          <span class="text-lg font-semibold">通知中心</span>
          <Badge
            v-if="unreadCount > 0"
            :count="unreadCount"
            :overflow-count="99"
            class="ml-2"
          />
        </div>
      </template>
      <template #extra>
        <Button
          :disabled="unreadCount === 0"
          type="link"
          @click="handleMarkAllAsRead"
        >
          <template #icon><MailCheck class="size-4" /></template>
          全部已读
        </Button>
      </template>

      <div class="mb-4 flex flex-wrap items-center gap-3">
        <Input
          v-model:value="searchText"
          placeholder="搜索通知..."
          class="w-64"
          allow-clear
          @press-enter="handleSearch"
        >
          <template #prefix><Search class="size-4 text-gray-400" /></template>
        </Input>

        <Select
          v-model:value="typeFilter"
          placeholder="通知类型"
          allow-clear
          class="w-32"
          @change="handleTypeChange"
        >
          <Select.Option value="System">系统</Select.Option>
          <Select.Option value="Task">任务</Select.Option>
          <Select.Option value="Habit">习惯</Select.Option>
          <Select.Option value="Ai">AI</Select.Option>
          <Select.Option value="Finance">财务</Select.Option>
        </Select>
      </div>

      <div class="mb-4 flex gap-4 border-b border-gray-200 pb-2">
        <button
          v-for="tab in tabItems"
          :key="tab.key"
          class="border-b-2 px-1 pb-1 text-sm transition-colors"
          :class="
            activeTab === tab.key
              ? 'border-blue-500 text-blue-500 font-medium'
              : 'border-transparent text-gray-500 hover:text-gray-700'
          "
          @click="handleTabChange(tab.key)"
        >
          {{ tab.label }}
        </button>
      </div>

      <List
        :data-source="notifications"
        :loading="loading"
        :pagination="{
          current: page,
          pageSize,
          total,
          showSizeChanger: true,
          showTotal: (t: number) => `共 ${t} 条`,
          onChange: handlePageChange,
        }"
        :locale="{ emptyText: '暂无通知' }"
        item-layout="horizontal"
      >
        <template #renderItem="{ item }">
          <List.Item
            class="cursor-pointer transition-colors hover:bg-gray-50 dark:hover:bg-gray-800"
            :class="{ 'bg-blue-50/50 dark:bg-blue-900/10': !item.isRead }"
            @click="handleClick(item)"
          >
            <List.Item.Meta>
              <template #avatar>
                <Avatar
                  :style="{ backgroundColor: typeColors[item.type] || '#1890ff' }"
                  size="large"
                >
                  {{ typeIcons[item.type] || '🔔' }}
                </Avatar>
              </template>
              <template #title>
                <div class="flex items-center gap-2">
                  <span
                    :class="{ 'font-bold': !item.isRead }"
                    class="flex-1"
                  >
                    {{ item.title }}
                  </span>
                  <Tag :color="typeColors[item.type]">
                    {{ typeLabels[item.type] || item.type }}
                  </Tag>
                  <span
                    v-if="!item.isRead"
                    class="size-2 rounded-full bg-blue-500"
                  />
                </div>
              </template>
              <template #description>
                <div>
                  <p v-if="item.content" class="mb-1 text-gray-600 dark:text-gray-400">
                    {{ item.content }}
                  </p>
                  <div class="flex items-center gap-3 text-xs text-gray-400">
                    <span class="flex items-center gap-1">
                      {{ formatTime(item.createdAt) }}
                    </span>
                    <span v-if="item.isRead && item.readAt" class="text-green-500">
                      已读
                    </span>
                  </div>
                </div>
              </template>
            </List.Item.Meta>
            <template #actions>
              <Button
                v-if="!item.isRead"
                type="link"
                size="small"
                @click.stop="handleMarkAsRead(item)"
              >
                标为已读
              </Button>
              <Button
                type="link"
                size="small"
                danger
                @click.stop="handleDelete(item)"
              >
                <template #icon><X class="size-3" /></template>
                删除
              </Button>
            </template>
          </List.Item>
        </template>
      </List>
    </Card>
  </div>
</template>

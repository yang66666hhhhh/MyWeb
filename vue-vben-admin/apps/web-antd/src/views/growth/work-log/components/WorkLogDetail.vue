<script lang="ts" setup>
import { Descriptions, Divider, Space, Tag } from 'ant-design-vue';

import type { WorkLog } from '#/api/growth';

defineProps<{ item?: null | WorkLog }>();

const categoryColorMap: Record<string, string> = {
  会议: 'gold',
  复盘: 'purple',
  学习: 'cyan',
  开发: 'blue',
  排障: 'red',
};

function formatDuration(minutes: number): string {
  if (minutes < 60) return `${minutes} 分钟`;
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return mins > 0 ? `${hours} 小时 ${mins} 分钟` : `${hours} 小时`;
}
</script>

<template>
  <div v-if="item">
    <Descriptions bordered :column="1" size="small">
      <Descriptions.Item label="工作日期">{{ item.logDate }}</Descriptions.Item>
      <Descriptions.Item label="工作标题" class="font-medium text-base">{{ item.title }}</Descriptions.Item>
      <Descriptions.Item label="工作分类">
        <Tag :color="categoryColorMap[item.category] || 'blue'">{{ item.category }}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="关联项目">{{ item.projectName || '-' }}</Descriptions.Item>
    </Descriptions>

    <Divider class="my-3" />

    <Descriptions bordered :column="1" size="small">
      <Descriptions.Item label="消耗时间">
        <span class="text-lg font-semibold text-blue-600">{{ formatDuration(item.durationMinutes) }}</span>
      </Descriptions.Item>
      <Descriptions.Item label="标签">
        <Space v-if="item.tags?.length" wrap>
          <Tag v-for="tag in item.tags" :key="tag">{{ tag }}</Tag>
        </Space>
        <span v-else>-</span>
      </Descriptions.Item>
    </Descriptions>

    <Divider class="my-3" />

    <div class="space-y-4">
      <div>
        <div class="text-sm text-text-secondary mb-1 font-medium">工作内容</div>
        <div class="rounded bg-gray-50 dark:bg-gray-800 p-3 text-sm whitespace-pre-wrap">{{ item.content || '-' }}</div>
      </div>

      <div v-if="item.summary">
        <div class="text-sm text-text-secondary mb-1 font-medium">今日总结</div>
        <div class="rounded bg-green-50 dark:bg-green-900/20 p-3 text-sm border-l-4 border-green-500">{{ item.summary }}</div>
      </div>

      <div v-if="item.issue">
        <div class="text-sm text-text-secondary mb-1 font-medium">遇到的问题</div>
        <div class="rounded bg-red-50 dark:bg-red-900/20 p-3 text-sm border-l-4 border-red-500">{{ item.issue }}</div>
      </div>

      <div v-if="item.nextPlan">
        <div class="text-sm text-text-secondary mb-1 font-medium">明日计划</div>
        <div class="rounded bg-blue-50 dark:bg-blue-900/20 p-3 text-sm border-l-4 border-blue-500">{{ item.nextPlan }}</div>
      </div>
    </div>
  </div>
</template>

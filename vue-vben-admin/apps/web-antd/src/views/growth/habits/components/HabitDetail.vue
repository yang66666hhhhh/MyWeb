<script lang="ts" setup>
import { computed } from 'vue';

import { Badge, Col, Descriptions, Progress, Row, Statistic, Tag } from 'ant-design-vue';

import type { Habit } from '#/api/growth';

defineProps<{ item?: Habit | null }>();

const habitTypeColorMap: Record<string, string> = {
  学习: 'blue',
  工作: 'purple',
  生活: 'orange',
  健康: 'green',
};

const habitTypeBadgeMap: Record<string, string> = {
  学习: 'processing',
  工作: 'purple',
  生活: 'warning',
  健康: 'success',
};

function getStreakStatus(streak: number, longest: number): { color: string; text: string } {
  if (streak === 0) return { color: 'default', text: '未开始' };
  if (streak >= longest && longest > 0) return { color: 'gold', text: '历史最佳!' };
  if (streak >= 7) return { color: 'green', text: '连续中' };
  if (streak >= 3) return { color: 'blue', text: '进行中' };
  return { color: 'default', text: '刚起步' };
}

function isTodayCompleted(item: Habit): boolean {
  if (!item.lastCheckInDate) return false;
  const today = new Date().toISOString().split('T')[0];
  return item.lastCheckInDate.split('T')[0] === today;
}
</script>

<template>
  <div v-if="item" class="space-y-4">
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-3">
        <div class="text-lg font-semibold">{{ item.name }}</div>
        <Badge :status="habitTypeBadgeMap[item.habitType] as any" :text="item.habitType" />
      </div>
      <Tag :color="item.status === 1 ? 'success' : 'default'" class="text-sm">
        {{ item.status === 1 ? '启用' : '停用' }}
      </Tag>
    </div>

    <div v-if="item.description" class="text-sm text-text-secondary bg-gray-50 dark:bg-gray-800 rounded p-3">
      {{ item.description }}
    </div>

    <Row :gutter="16">
      <Col :span="8">
        <Statistic title="当前连续" :value="item.currentStreak" suffix="天" :value-style="{ color: '#52c41a' }" />
      </Col>
      <Col :span="8">
        <Statistic title="最长连续" :value="item.longestStreak" suffix="天" :value-style="{ color: '#faad14' }" />
      </Col>
      <Col :span="8">
        <Statistic title="累计打卡" :value="item.totalCheckIns" suffix="次" />
      </Col>
    </Row>

    <div class="border-t pt-4">
      <div class="flex items-center justify-between mb-2">
        <span class="text-sm text-text-secondary">连续打卡进度</span>
        <Tag :color="getStreakStatus(item.currentStreak, item.longestStreak).color">
          {{ getStreakStatus(item.currentStreak, item.longestStreak).text }}
        </Tag>
      </div>
      <Progress
        :percent="item.longestStreak > 0 ? Math.round((item.currentStreak / item.longestStreak) * 100) : 0"
        :stroke-color="item.currentStreak >= item.longestStreak && item.longestStreak > 0 ? '#faad14' : '#52c41a'"
      />
      <div class="text-xs text-text-secondary mt-1">
        当前 {{ item.currentStreak }} 天 / 历史最长 {{ item.longestStreak }} 天
      </div>
    </div>

    <Descriptions bordered :column="1" size="small">
      <Descriptions.Item label="习惯类型">
        <Tag :color="habitTypeColorMap[item.habitType] || 'blue'">{{ item.habitType }}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="打卡频率">{{ item.targetFrequency }}</Descriptions.Item>
      <Descriptions.Item label="今日打卡">
        <Tag :color="isTodayCompleted(item) ? 'success' : 'default'">
          {{ isTodayCompleted(item) ? '已打卡' : '未打卡' }}
        </Tag>
      </Descriptions.Item>
      <Descriptions.Item label="最近打卡">{{ item.lastCheckInDate || '-' }}</Descriptions.Item>
    </Descriptions>
  </div>
</template>

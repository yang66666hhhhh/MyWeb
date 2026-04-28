<script lang="ts" setup>
import { computed } from 'vue';

import { Col, Descriptions, Progress, Row, Statistic, Tag } from 'ant-design-vue';

import type { Project } from '#/api/growth';

defineProps<{ item?: Project | null }>();

const projectTypeMap: Record<number, { color: string; label: string }> = {
  0: { color: 'blue', label: '工作' },
  1: { color: 'purple', label: '学习' },
  2: { color: 'cyan', label: '个人' },
  3: { color: 'default', label: '其他' },
};

const projectStatusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '未开始' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'warning', label: '暂停' },
};

function getDateRange(start?: string, end?: string): string {
  if (!start && !end) return '未定';
  if (start && !end) return `${start} - 至今`;
  if (!start && end) return `至 ${end}`;
  return `${start} - ${end}`;
}

function getProjectDuration(start?: string, end?: string): { days: number; text: string } {
  if (!start) return { days: 0, text: '-' };
  const startDate = new Date(start);
  const endDate = end ? new Date(end) : new Date();
  const diffTime = Math.abs(endDate.getTime() - startDate.getTime());
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  if (diffDays === 0) return { days: 1, text: '1 天' };
  return { days: diffDays, text: `${diffDays} 天` };
}
</script>

<template>
  <div v-if="item" class="space-y-4">
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-3">
        <div class="text-lg font-semibold">{{ item.name }}</div>
        <Tag :color="projectTypeMap[item.type].color">{{ projectTypeMap[item.type].label }}</Tag>
      </div>
      <Tag :color="projectStatusMap[item.status].color">{{ projectStatusMap[item.status].label }}</Tag>
    </div>

    <div v-if="item.description" class="text-sm text-text-secondary bg-gray-50 dark:bg-gray-800 rounded p-3">
      {{ item.description }}
    </div>

    <Row :gutter="16">
      <Col :span="8">
        <Statistic title="项目进度" :value="item.progress" suffix="%" :value-style="{ color: '#1890ff' }" />
      </Col>
      <Col :span="8">
        <Statistic title="关联任务" :value="item.taskCount" />
      </Col>
      <Col :span="8">
        <Statistic title="项目周期" :value="getProjectDuration(item.startDate, item.endDate).text" />
      </Col>
    </Row>

    <div class="border-t pt-4">
      <div class="flex items-center justify-between mb-2">
        <span class="text-sm text-text-secondary">完成进度</span>
        <span class="text-sm font-medium">{{ item.progress }}%</span>
      </div>
      <Progress
        :percent="item.progress"
        :stroke-color="item.progress === 100 ? '#52c41a' : '#1890ff'"
        :status="item.progress === 100 ? 'success' : 'active'"
      />
    </div>

    <Descriptions bordered :column="1" size="small">
      <Descriptions.Item label="项目类型">
        <Tag :color="projectTypeMap[item.type].color">{{ projectTypeMap[item.type].label }}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="项目状态">
        <Tag :color="projectStatusMap[item.status].color">{{ projectStatusMap[item.status].label }}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="项目周期">
        {{ getDateRange(item.startDate, item.endDate) }}
      </Descriptions.Item>
      <Descriptions.Item label="开始日期">{{ item.startDate || '-' }}</Descriptions.Item>
      <Descriptions.Item label="结束日期">{{ item.endDate || '-' }}</Descriptions.Item>
      <Descriptions.Item label="关联任务数">{{ item.taskCount }} 个</Descriptions.Item>
    </Descriptions>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const tasks = ref([
  { id: '1', name: '需求分析', start: '2024-01-01', end: '2024-01-05', progress: 100, dependencies: '', status: '已完成' },
  { id: '2', name: '系统设计', start: '2024-01-06', end: '2024-01-10', progress: 100, dependencies: '需求分析', status: '已完成' },
  { id: '3', name: '前端开发', start: '2024-01-11', end: '2024-01-25', progress: 60, dependencies: '系统设计', status: '进行中' },
  { id: '4', name: '后端开发', start: '2024-01-11', end: '2024-01-25', progress: 50, dependencies: '系统设计', status: '进行中' },
  { id: '5', name: '测试', start: '2024-01-26', end: '2024-02-05', progress: 0, dependencies: '前端开发,后端开发', status: '待开始' },
  { id: '6', name: '部署上线', start: '2024-02-06', end: '2024-02-10', progress: 0, dependencies: '测试', status: '待开始' },
]);

const columns = [
  { title: '任务名称', dataIndex: 'name', key: 'name', width: 150 },
  { title: '开始日期', dataIndex: 'start', key: 'start', width: 120 },
  { title: '结束日期', dataIndex: 'end', key: 'end', width: 120 },
  { title: '进度', dataIndex: 'progress', key: 'progress', width: 200 },
  { title: '依赖', dataIndex: 'dependencies', key: 'dependencies', width: 200 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
];

const statusColors: Record<string, string> = {
  '已完成': 'success',
  '进行中': 'processing',
  '待开始': 'default',
};
</script>

<template>
  <Page description="使用甘特图管理项目进度" title="甘特图">
    <Card title="项目进度">
      <Table :columns="columns" :data-source="tasks" :loading="loading" row-key="id" :scroll="{ x: 1000 }">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="w-32 h-2 bg-gray-200 rounded-full">
                <div class="h-full bg-blue-500 rounded-full" :style="{ width: `${record.progress}%` }" />
              </div>
              <span>{{ record.progress }}%</span>
            </div>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ record.status }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>

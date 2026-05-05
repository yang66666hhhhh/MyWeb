<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, List, Tag } from 'ant-design-vue';

const loading = ref(false);

const reports = ref([
  { id: '1', name: '月度成长报告', type: '月报', period: '2024-01', status: '已生成', createdAt: '2024-02-01' },
  { id: '2', name: '工作效率周报', type: '周报', period: '2024-W03', status: '已生成', createdAt: '2024-01-22' },
  { id: '3', name: '习惯养成报告', type: '自定义', period: '2024-01', status: '已生成', createdAt: '2024-01-31' },
]);

const typeColors: Record<string, string> = {
  '月报': 'blue',
  '周报': 'green',
  '年报': 'purple',
  '自定义': 'orange',
};
</script>

<template>
  <Page description="创建和查看自定义报表" title="自定义报表">
    <Card title="报表列表">
      <template #extra><Button type="primary">创建报表</Button></template>
      <List :data-source="reports" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name" :description="`周期: ${item.period} | 生成时间: ${item.createdAt}`" />
            <div class="flex items-center gap-2">
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Tag color="success">{{ item.status }}</Tag>
              <Button type="link">查看</Button>
              <Button type="link">导出</Button>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>

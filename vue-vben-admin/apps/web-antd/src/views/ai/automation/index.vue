<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const workflows = ref([
  { id: '1', name: '日报自动生成', trigger: '每天18:00', action: '汇总工作日志并生成日报', status: '运行中', lastRun: '2024-01-15 18:00' },
  { id: '2', name: '周报提醒', trigger: '每周五17:00', action: '发送周报填写提醒', status: '运行中', lastRun: '2024-01-12 17:00' },
  { id: '3', name: '习惯打卡提醒', trigger: '每天21:00', action: '检查并提醒未打卡习惯', status: '运行中', lastRun: '2024-01-15 21:00' },
  { id: '4', name: '学习计划推送', trigger: '每天08:00', action: '推送今日学习计划', status: '已暂停', lastRun: '2024-01-14 08:00' },
]);

const statusColors: Record<string, string> = {
  '运行中': 'success',
  '已暂停': 'warning',
  '已停止': 'error',
};
</script>

<template>
  <Page description="创建和管理自动化工作流" title="自动化工作流">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="工作流总数" :value="workflows.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="运行中" :value="3" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已暂停" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="今日执行" :value="3" /></Card>
      </Col>
    </Row>

    <Card title="工作流列表">
      <template #extra><Button type="primary">创建工作流</Button></template>
      <List :data-source="workflows" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name">
              <template #description>
                <div>
                  <div>触发: {{ item.trigger }}</div>
                  <div>动作: {{ item.action }}</div>
                  <div>最后执行: {{ item.lastRun }}</div>
                </div>
              </template>
            </List.Item.Meta>
            <Space>
              <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
              <Button type="link">{{ item.status === '运行中' ? '暂停' : '启动' }}</Button>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateSleepRecordInput, SleepRecord, UpdateSleepRecordInput } from '#/api/growth/extended';

import {
  createSleepRecordApi,
  deleteSleepRecordApi,
  getSleepRecordsApi,
  updateSleepRecordApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateSleepRecordInput & UpdateSleepRecordInput>({
  bedTime: '',
  wakeTime: '',
  quality: 3,
  notes: '',
  tags: '',
});

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  SleepRecord,
  { keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getSleepRecordsApi,
});

const qualityMap: Record<number, { color: string; label: string }> = {
  1: { color: 'error', label: '很差' },
  2: { color: 'warning', label: '较差' },
  3: { color: 'processing', label: '一般' },
  4: { color: 'success', label: '良好' },
  5: { color: 'green', label: '极好' },
};

const columns = [
  { dataIndex: 'createdAt', key: 'createdAt', title: '日期', width: 120 },
  { dataIndex: 'bedTime', key: 'bedTime', title: '就寝时间', width: 120 },
  { dataIndex: 'wakeTime', key: 'wakeTime', title: '起床时间', width: 120 },
  { dataIndex: 'durationMinutes', key: 'durationMinutes', title: '睡眠时长', width: 110 },
  { dataIndex: 'quality', key: 'quality', title: '睡眠质量', width: 100 },
  { dataIndex: 'notes', key: 'notes', title: '备注', ellipsis: true },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as SleepRecord[];
  const avgDuration = list.length
    ? (list.reduce((s, i) => s + i.durationMinutes, 0) / list.length / 60).toFixed(1)
    : '0';
  const avgQuality = list.length
    ? (list.reduce((s, i) => s + i.quality, 0) / list.length).toFixed(1)
    : '0';
  return { total: total.value, avgDuration, avgQuality };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function formatDuration(minutes: number) {
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  return mins > 0 ? `${hours}h${mins}m` : `${hours}h`;
}

function openCreate() {
  editingId.value = null;
  formData.value = { bedTime: '', wakeTime: '', quality: 3, notes: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: SleepRecord) {
  editingId.value = record.id;
  formData.value = {
    bedTime: record.bedTime,
    wakeTime: record.wakeTime,
    quality: record.quality,
    notes: record.notes || '',
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  try {
    if (editingId.value) {
      await updateSleepRecordApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createSleepRecordApi(formData.value as CreateSleepRecordInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteSleepRecordApi(id);
    message.success('删除成功');
    await load();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="追踪睡眠质量和规律" title="睡眠追踪">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="记录天数" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均时长" :value="stats.avgDuration" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均质量" :value="stats.avgQuality" suffix="/5" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" suffix="条" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="搜索" @press-enter="search" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button @click="openCreate">记录睡眠</Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>

    <Card>
      <Table
        :columns="columns"
        :data-source="items"
        :loading="loading"
        :pagination="{
          current: query.page,
          pageSize: query.pageSize,
          showSizeChanger: true,
          showTotal: (value: number) => `共 ${value} 条`,
          total,
        }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'durationMinutes'">
            {{ formatDuration(record.durationMinutes) }}
          </template>
          <template v-else-if="column.key === 'quality'">
            <Tag :color="qualityMap[record.quality]?.color">{{ qualityMap[record.quality]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record as SleepRecord)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑睡眠记录' : '记录睡眠'"
      @ok="handleSubmit"
    >
      <Form layout="vertical" :model="formData">
        <Form.Item label="就寝时间" required>
          <Input v-model:value="formData.bedTime" placeholder="如: 23:00" />
        </Form.Item>
        <Form.Item label="起床时间" required>
          <Input v-model:value="formData.wakeTime" placeholder="如: 07:00" />
        </Form.Item>
        <Form.Item label="睡眠质量">
          <InputNumber v-model:value="formData.quality" :min="1" :max="5" class="w-full" />
        </Form.Item>
        <Form.Item label="备注">
          <Input.TextArea v-model:value="formData.notes" placeholder="睡眠感受" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

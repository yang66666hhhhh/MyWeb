<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateMoodRecordInput, MoodRecord, UpdateMoodRecordInput } from '#/api/growth/extended';

import {
  createMoodRecordApi,
  deleteMoodRecordApi,
  getMoodRecordsApi,
  updateMoodRecordApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const submitting = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateMoodRecordInput & UpdateMoodRecordInput>({
  moodLevel: 5,
  moodType: '',
  notes: '',
  recordDate: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  recordDate: [{ required: true, message: '请选择记录日期', type: 'string' as const }],};

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  MoodRecord,
  { keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getMoodRecordsApi,
});

const moodTypeOptions = [
  { label: '开心', value: '开心', color: 'green' },
  { label: '平静', value: '平静', color: 'blue' },
  { label: '兴奋', value: '兴奋', color: 'purple' },
  { label: '疲惫', value: '疲惫', color: 'orange' },
  { label: '焦虑', value: '焦虑', color: 'red' },
  { label: '难过', value: '难过', color: 'gray' },
];

const moodColorMap: Record<string, string> = {
  开心: 'green',
  平静: 'blue',
  兴奋: 'purple',
  疲惫: 'orange',
  焦虑: 'red',
  难过: 'gray',
};

const columns = [
  { dataIndex: 'recordDate', key: 'recordDate', title: '日期', width: 120 },
  { dataIndex: 'moodType', key: 'moodType', title: '心情', width: 100 },
  { dataIndex: 'moodLevel', key: 'moodLevel', title: '情绪指数', width: 100 },
  { dataIndex: 'notes', key: 'notes', title: '备注', ellipsis: true },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 180 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as MoodRecord[];
  const avgLevel = list.length
    ? (list.reduce((s, i) => s + i.moodLevel, 0) / list.length).toFixed(1)
    : '0';
  return { total: total.value, avgLevel };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { moodLevel: 5, moodType: '', notes: '', recordDate: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: MoodRecord) {
  editingId.value = record.id;
  formData.value = {
    moodLevel: record.moodLevel,
    moodType: record.moodType || '',
    notes: record.notes || '',
    recordDate: record.recordDate,
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
    try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateMoodRecordApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createMoodRecordApi(formData.value as CreateMoodRecordInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  } finally {
    submitting.value = false;
  }
}

async function handleDelete(id: string) {
  try {
    await deleteMoodRecordApi(id);
    message.success('删除成功');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="追踪和分析您的情绪变化" title="心情追踪">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="记录天数" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均指数" :value="stats.avgLevel" suffix="/10" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" suffix="条" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总页数" :value="Math.ceil(total / 10)" /></Card>
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
            <Button @click="openCreate">记录心情</Button>
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
          <template v-if="column.key === 'moodType'">
            <Tag v-if="record.moodType" :color="moodColorMap[record.moodType] || 'default'">{{ record.moodType }}</Tag>
            <span v-else>-</span>
          </template>
          <template v-else-if="column.key === 'moodLevel'">
            <span>{{ record.moodLevel }}/10</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record as MoodRecord)">编辑</Button>
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
      :confirm-loading="submitting"
      :title="editingId ? '编辑心情记录' : '记录心情'"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="记录日期" required>
          <DatePicker v-model:value="formData.recordDate" class="w-full" />
        </Form.Item>
        <Form.Item label="心情类型">
          <Select v-model:value="formData.moodType" placeholder="选择心情">
            <Select.Option v-for="opt in moodTypeOptions" :key="opt.value" :value="opt.value">
              <Tag :color="opt.color">{{ opt.label }}</Tag>
            </Select.Option>
          </Select>
        </Form.Item>
        <Form.Item label="情绪指数(1-10)">
          <InputNumber v-model:value="formData.moodLevel" :min="1" :max="10" class="w-full" />
        </Form.Item>
        <Form.Item label="备注">
          <Input.TextArea v-model:value="formData.notes" placeholder="今天的心情如何？" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

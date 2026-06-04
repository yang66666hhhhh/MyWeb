<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

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
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateFitnessRecordInput, FitnessRecord, UpdateFitnessRecordInput } from '#/api/growth/extended';

import {
  createFitnessRecordApi,
  deleteFitnessRecordApi,
  getFitnessRecordsApi,
  updateFitnessRecordApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const accessStore = useAccessStore();
const hasPermission = (code: string) => accessStore.accessCodes.includes(code);

const formOpen = ref(false);
const submitting = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateFitnessRecordInput & UpdateFitnessRecordInput>({
  exerciseType: '',
  durationMinutes: 0,
  caloriesBurned: 0,
  notes: '',
  exerciseDate: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  exerciseType: [{ required: true, message: '请输入运动类型', type: 'string' as const }],
  exerciseDate: [{ required: true, message: '请选择运动日期', type: 'string' as const }],
  durationMinutes: [{ required: true, message: '请输入时长', type: 'number' as const }],};

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  FitnessRecord,
  { keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getFitnessRecordsApi,
});

const typeColors: Record<string, string> = {
  跑步: 'blue',
  力量训练: 'red',
  瑜伽: 'purple',
  游泳: 'cyan',
  骑行: 'green',
  有氧: 'orange',
};

const columns = [
  { dataIndex: 'exerciseDate', key: 'exerciseDate', title: '日期', width: 120 },
  { dataIndex: 'exerciseType', key: 'exerciseType', title: '运动类型', width: 120 },
  { dataIndex: 'durationMinutes', key: 'durationMinutes', title: '时长(分钟)', width: 110 },
  { dataIndex: 'caloriesBurned', key: 'caloriesBurned', title: '消耗热量', width: 100 },
  { dataIndex: 'notes', key: 'notes', title: '备注', ellipsis: true },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 180 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as FitnessRecord[];
  const totalDuration = list.reduce((s, i) => s + i.durationMinutes, 0);
  const totalCalories = list.reduce((s, i) => s + i.caloriesBurned, 0);
  return { total: total.value, totalDuration, totalCalories };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { exerciseType: '', durationMinutes: 0, caloriesBurned: 0, notes: '', exerciseDate: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: FitnessRecord) {
  editingId.value = record.id;
  formData.value = {
    exerciseType: record.exerciseType,
    durationMinutes: record.durationMinutes,
    caloriesBurned: record.caloriesBurned,
    notes: record.notes || '',
    exerciseDate: record.exerciseDate,
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
    try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateFitnessRecordApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createFitnessRecordApi(formData.value as CreateFitnessRecordInput);
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
    await deleteFitnessRecordApi(id);
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
  <Page description="记录和管理您的健身活动" title="健身管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="运动记录" :value="stats.total" suffix="次" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总时长" :value="stats.totalDuration" suffix="分钟" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="消耗热量" :value="stats.totalCalories" suffix="卡" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" suffix="条" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="运动类型" @press-enter="search" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button v-if="hasPermission('GROWTH_FITNESS')" @click="openCreate">记录运动</Button>
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
          <template v-if="column.key === 'exerciseType'">
            <Tag :color="typeColors[record.exerciseType] || 'default'">{{ record.exerciseType }}</Tag>
          </template>
          <template v-else-if="column.key === 'durationMinutes'">
            {{ record.durationMinutes }}分钟
          </template>
          <template v-else-if="column.key === 'caloriesBurned'">
            {{ record.caloriesBurned }}卡
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button v-if="hasPermission('GROWTH_FITNESS')" size="small" type="link" @click="openEdit(record as FitnessRecord)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button v-if="hasPermission('GROWTH_FITNESS')" danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :confirm-loading="submitting"
      :title="editingId ? '编辑运动记录' : '记录运动'"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="运动类型" required>
          <Input v-model:value="formData.exerciseType" placeholder="如: 跑步, 力量训练, 瑜伽" />
        </Form.Item>
        <Form.Item label="运动日期" required>
          <DatePicker v-model:value="formData.exerciseDate" class="w-full" />
        </Form.Item>
        <Form.Item label="时长(分钟)" required>
          <InputNumber v-model:value="formData.durationMinutes" :min="1" class="w-full" />
        </Form.Item>
        <Form.Item label="消耗热量(卡)">
          <InputNumber v-model:value="formData.caloriesBurned" :min="0" class="w-full" />
        </Form.Item>
        <Form.Item label="备注">
          <Input.TextArea v-model:value="formData.notes" placeholder="运动详情" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Descriptions,
  Drawer,
  Form,
  Input,
  InputNumber,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import type { Okr } from '#/api/work/extended';
import {
  createOkrApi,
  deleteOkrApi,
  getOkrsApi,
  updateOkrApi,
} from '#/api/work/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<Okr | null>(null);

const formState = ref({
  description: '',
  keyResults: '',
  objective: '',
  quarter: 1,
  tags: '',
  title: '',
  year: new Date().getFullYear(),
});

const quarterOptions = [
  { label: 'Q1', value: 1 },
  { label: 'Q2', value: 2 },
  { label: 'Q3', value: 3 },
  { label: 'Q4', value: 4 },
];

const columns: any[] = [
  { dataIndex: 'title', key: 'title', title: '标题', minWidth: 140 },
  { dataIndex: 'objective', key: 'objective', title: '目标', minWidth: 200 },
  { dataIndex: 'progress', key: 'progress', title: '进度', width: 150 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 90 },
  { dataIndex: 'year', key: 'year', title: '年份', width: 80 },
  { dataIndex: 'quarter', key: 'quarter', title: '季度', width: 80 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const statusOptions = [
  { label: '进行中', value: 0 },
  { label: '已完成', value: 1 },
  { label: '已取消', value: 2 },
];

const statusColors: Record<number, string> = {
  0: 'processing',
  1: 'success',
  2: 'default',
};

const statusLabels: Record<number, string> = {
  0: '进行中',
  1: '已完成',
  2: '已取消',
};

const { changePage, items, load, loading, query, resetQuery, search, total } =
  usePagedQuery<
    Okr,
    {
      keyword?: string;
      page: number;
      pageSize: number;
      quarter?: number;
      status?: number;
      year?: number;
    }
  >({
    defaultQuery: { page: 1, pageSize: 10 },
    fetcher: getOkrsApi,
  });

function openCreate() {
  editingId.value = null;
  formState.value = {
    description: '',
    keyResults: '',
    objective: '',
    quarter: 1,
    tags: '',
    title: '',
    year: new Date().getFullYear(),
  };
  formOpen.value = true;
}

async function openEdit(record: Record<string, any>) {
  const okr = record as Okr;
  editingId.value = okr.id;
  formState.value = {
    description: okr.description || '',
    keyResults: okr.keyResults || '',
    objective: okr.objective || '',
    quarter: okr.quarter || 1,
    tags: okr.tags || '',
    title: okr.title || '',
    year: okr.year || new Date().getFullYear(),
  };
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as Okr;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  try {
    await deleteOkrApi(id);
    message.success('OKR已删除');
    await load();
  } catch {
    message.error('删除失败');
  }
}

async function handleSubmit() {
  if (!formState.value.title.trim()) {
    message.warning('请填写标题');
    return;
  }
  if (!formState.value.objective.trim()) {
    message.warning('请填写目标');
    return;
  }
  try {
    if (editingId.value) {
      await updateOkrApi(editingId.value, formState.value);
      message.success('OKR已更新');
    } else {
      await createOkrApi(formState.value as any);
      message.success('OKR已创建');
    }
    formOpen.value = false;
    await load();
  } catch {
    message.error('保存失败');
  }
}

function resetFilters() {
  resetQuery();
  void load();
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="设定和追踪OKR目标" title="OKR管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="目标数" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="items.filter((i: Okr) => i.status === 0).length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="items.filter((i: Okr) => i.status === 1).length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic
            title="平均进度"
            :value="items.length ? Math.round(items.reduce((s: number, i: Okr) => s + (i.progress || 0), 0) / items.length) : 0"
            suffix="%"
          />
        </Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <Form :model="query" layout="inline">
          <Form.Item label="年份">
            <InputNumber v-model:value="query.year" :min="2020" :max="2030" class="w-24" />
          </Form.Item>
          <Form.Item label="季度">
            <Select v-model:value="query.quarter" :options="quarterOptions" allow-clear class="w-24" />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-28" />
          </Form.Item>
          <Form.Item label="关键词">
            <Input v-model:value="query.keyword" allow-clear placeholder="标题/目标" style="width: 180px" @press-enter="search" />
          </Form.Item>
          <Form.Item>
            <Space>
              <Button type="primary" @click="search">查询</Button>
              <Button @click="resetFilters">重置</Button>
            </Space>
          </Form.Item>
        </Form>
        <Button type="primary" @click="openCreate">新建OKR</Button>
      </div>
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
        :scroll="{ x: 1000 }"
        row-key="id"
        @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'title'">
            <div class="font-medium">{{ record.title }}</div>
            <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'objective'">
            <div>
              <div class="font-bold mb-1">{{ record.objective }}</div>
              <div v-for="(kr, index) in (record.keyResults || '').split('\n').filter(Boolean)" :key="index" class="text-gray-500 text-sm">
                {{ Number(index) + 1 }}. {{ kr }}
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="w-20 h-2 bg-gray-200 rounded-full">
                <div class="h-full bg-blue-500 rounded-full" :style="{ width: `${record.progress || 0}%` }"></div>
              </div>
              <span>{{ record.progress || 0 }}%</span>
            </div>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ statusLabels[record.status] }}</Tag>
          </template>
          <template v-else-if="column.key === 'quarter'">
            Q{{ record.quarter }}
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="showDetail(record)">详情</Button>
              <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      :open="formOpen"
      :title="editingId ? '编辑OKR' : '新建OKR'"
      width="600px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="标题" required>
          <Input v-model:value="formState.title" placeholder="OKR标题" />
        </Form.Item>
        <Form.Item label="目标" required>
          <Input.TextArea v-model:value="formState.objective" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="目标描述" />
        </Form.Item>
        <Form.Item label="关键结果（每行一个）">
          <Input.TextArea v-model:value="formState.keyResults" :auto-size="{ minRows: 3, maxRows: 6 }" placeholder="关键结果1&#10;关键结果2&#10;关键结果3" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="年份">
            <InputNumber v-model:value="formState.year" :min="2020" :max="2030" class="w-full" />
          </Form.Item>
          <Form.Item label="季度">
            <Select v-model:value="formState.quarter" :options="quarterOptions" class="w-full" />
          </Form.Item>
        </div>
        <Form.Item label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="OKR详情" width="500px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="标题">{{ selectedItem.title }}</Descriptions.Item>
          <Descriptions.Item label="目标">{{ selectedItem.objective }}</Descriptions.Item>
          <Descriptions.Item label="关键结果">
            <div v-for="(kr, index) in (selectedItem.keyResults || '').split('\n').filter(Boolean)" :key="index" class="text-sm">
              {{ Number(index) + 1 }}. {{ kr }}
            </div>
          </Descriptions.Item>
          <Descriptions.Item label="进度">
            <div class="flex items-center gap-2">
              <div class="w-24 h-2 bg-gray-200 rounded-full">
                <div class="h-full bg-blue-500 rounded-full" :style="{ width: `${selectedItem.progress || 0}%` }"></div>
              </div>
              <span>{{ selectedItem.progress || 0 }}%</span>
            </div>
          </Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="statusColors[selectedItem.status]">{{ statusLabels[selectedItem.status] }}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="年份/季度">{{ selectedItem.year }} Q{{ selectedItem.quarter }}</Descriptions.Item>
          <Descriptions.Item label="标签">{{ selectedItem.tags || '-' }}</Descriptions.Item>
          <Descriptions.Item label="创建时间">{{ selectedItem.createdAt }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

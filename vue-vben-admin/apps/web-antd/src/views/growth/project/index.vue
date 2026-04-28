<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Form,
  Input,
  Modal,
  Popconfirm,
  Progress,
  Select,
  Space,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import type { Dayjs } from 'dayjs';

import type { Project, ProjectStatus, ProjectType } from '#/api/growth';

import { deleteProjectApi, getProjectPageApi } from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

import ProjectDetail from './components/ProjectDetail.vue';
import ProjectForm from './components/ProjectForm.vue';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<null | Project>(null);

const typeMap: Record<ProjectType, { color: string; label: string }> = {
  0: { color: 'blue', label: '工作' },
  1: { color: 'purple', label: '学习' },
  2: { color: 'cyan', label: '个人' },
  3: { color: 'default', label: '其他' },
};

const statusMap: Record<ProjectStatus, { color: string; label: string }> = {
  0: { color: 'default', label: '未开始' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'warning', label: '暂停' },
};

const typeOptions = Object.entries(typeMap).map(([value, item]) => ({
  label: item.label,
  value: Number(value),
}));

const statusOptions = Object.entries(statusMap).map(([value, item]) => ({
  label: item.label,
  value: Number(value),
}));

const columns: any[] = [
  { dataIndex: 'name', key: 'name', title: '项目名称', minWidth: 220 },
  { dataIndex: 'type', key: 'type', title: '项目类型', width: 100 },
  { dataIndex: 'status', key: 'status', title: '项目状态', width: 110 },
  { dataIndex: 'progress', key: 'progress', title: '项目进度', width: 170 },
  { dataIndex: 'startDate', key: 'startDate', title: '开始日期', width: 120 },
  { dataIndex: 'endDate', key: 'endDate', title: '结束日期', width: 120 },
  { dataIndex: 'taskCount', key: 'taskCount', title: '关联任务数', width: 110 },
  { key: 'action', title: '操作', width: 210, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  Project,
  { endDate?: string; keyword?: string; page: number; pageSize: number; startDate?: string; status?: ProjectStatus; type?: ProjectType }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getProjectPageApi,
});

function onDateRangeChange(values: [string, string] | [Dayjs, Dayjs] | null) {
  const start = values?.[0];
  const end = values?.[1];
  query.startDate =
    start && typeof start !== 'string' ? start.format('YYYY-MM-DD') : undefined;
  query.endDate =
    end && typeof end !== 'string' ? end.format('YYYY-MM-DD') : undefined;
}

function openCreate() {
  editingId.value = null;
  formOpen.value = true;
}

function openEdit(record: Record<string, any>) {
  editingId.value = (record as Project).id;
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as Project;
  detailOpen.value = true;
}

async function remove(id: string) {
  await deleteProjectApi(id);
  message.success('项目已删除');
  await load();
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function resetFilters() {
  resetQuery();
  void load();
}

function handleFormOpenChange(value: boolean) {
  if (!value) {
    editingId.value = null;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="管理工作、学习、个人和其他项目，当前使用 mock/预留接口。" title="项目管理">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="日期范围">
              <DatePicker.RangePicker style="width: 240px" @change="onDateRangeChange" />
            </Form.Item>
            <Form.Item label="项目类型">
              <Select v-model:value="query.type" :options="typeOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="项目状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="项目名称/描述"
                style="width: 220px"
                @press-enter="search"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增项目</Button>
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
          :scroll="{ x: 1180 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'name'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.name }}</div>
                <div class="text-text-secondary line-clamp-2 text-xs">
                  {{ record.description || '暂无项目描述' }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'type'">
              <Tag :color="typeMap[text as ProjectType].color">
                {{ typeMap[text as ProjectType].label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="statusMap[text as ProjectStatus].color">
                {{ statusMap[text as ProjectStatus].label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'progress'">
              <Progress :percent="text" size="small" />
            </template>
            <template v-else-if="column.key === 'endDate'">
              {{ text || '-' }}
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm title="确认删除这个项目？" @confirm="remove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <ProjectForm
      v-model:open="formOpen"
      :id="editingId"
      @success="load"
      @update:open="handleFormOpenChange"
    />
    <Modal v-model:open="detailOpen" title="项目详情" width="680px" :footer="null">
      <ProjectDetail :item="selectedItem" />
    </Modal>
  </Page>
</template>

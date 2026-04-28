<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Descriptions,
  Drawer,
  Form,
  Input,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import type { WorkProject } from '#/api/growth/work';
import {
  createWorkProjectApi,
  deleteWorkProjectApi,
  getWorkProjectApi,
  getWorkProjectPageApi,
  updateWorkProjectApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';
import {
  WorkProjectStatus,
  WorkProjectStatusColor,
  WorkProjectStatusLabel,
  WorkProjectType,
  WorkProjectTypeLabel,
} from '#/enums/workEnum';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkProject | null>(null);

const formState = ref({
  description: '',
  endDate: '',
  projectCode: '',
  projectName: '',
  projectType: 0,
  startDate: '',
  status: 0,
});

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '进行中', value: 0 },
  { label: '已完成', value: 1 },
  { label: '已暂停', value: 2 },
  { label: '已归档', value: 3 },
];

const typeOptions = [
  { label: '内部项目', value: 0 },
  { label: '外部项目', value: 1 },
  { label: '研发项目', value: 2 },
  { label: '支持项目', value: 3 },
  { label: '其他', value: 4 },
];

const columns = [
  { dataIndex: 'projectName', key: 'projectName', title: '项目名称', minWidth: 180 },
  { dataIndex: 'projectCode', key: 'projectCode', title: '项目编号', width: 120 },
  { dataIndex: 'projectType', key: 'projectType', title: '项目类型', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 90 },
  { dataIndex: 'startDate', key: 'startDate', title: '开始日期', width: 110 },
  { dataIndex: 'endDate', key: 'endDate', title: '结束日期', width: 110 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkProject,
  { keyword?: string; page: number; pageSize: number; projectType?: number; status?: number }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getWorkProjectPageApi,
});

function openCreate() {
  editingId.value = null;
  formState.value = {
    description: '',
    endDate: '',
    projectCode: '',
    projectName: '',
    projectType: 0,
    startDate: dayjs().format('YYYY-MM-DD'),
    status: 0,
  };
  formOpen.value = true;
}

async function openEdit(record: WorkProject) {
  editingId.value = record.id;
  const detail = await getWorkProjectApi(record.id);
  if (detail) {
    formState.value = {
      description: detail.description || '',
      endDate: detail.endDate || '',
      projectCode: detail.projectCode || '',
      projectName: detail.projectName,
      projectType: detail.projectType ?? 0,
      startDate: detail.startDate || '',
      status: detail.status,
    };
  }
  formOpen.value = true;
}

function showDetail(record: WorkProject) {
  selectedItem.value = record;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  await deleteWorkProjectApi(id);
  message.success('项目已删除');
  await load();
}

async function handleSubmit() {
  if (!formState.value.projectName.trim()) {
    message.warning('请填写项目名称');
    return;
  }
  if (editingId.value) {
    await updateWorkProjectApi(editingId.value, formState.value);
    message.success('项目已更新');
  } else {
    await createWorkProjectApi(formState.value);
    message.success('项目已创建');
  }
  formOpen.value = false;
  await load();
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
  <Page description="管理工作项目信息" title="项目管理">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="项目类型">
              <Select v-model:value="query.projectType" :options="typeOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input v-model:value="query.keyword" allow-clear placeholder="项目名称/编号" style="width: 180px" @press-enter="search" />
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
          :scroll="{ x: 1000 }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'projectName'">
              <div class="font-medium">{{ record.projectName }}</div>
              <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
                {{ record.description }}
              </div>
            </template>
            <template v-else-if="column.key === 'projectType'">
              {{ WorkProjectTypeLabel[text as WorkProjectType] }}
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="WorkProjectStatusColor[text as WorkProjectStatus]">
                {{ WorkProjectStatusLabel[text as WorkProjectStatus] }}
              </Tag>
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
    </div>

    <Modal
      :open="formOpen"
      :title="editingId ? '编辑项目' : '新增项目'"
      width="600px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="项目名称" required>
          <Input v-model:value="formState.projectName" placeholder="项目名称" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="项目编号">
            <Input v-model:value="formState.projectCode" placeholder="项目编号" />
          </Form.Item>
          <Form.Item label="项目类型">
            <Select v-model:value="formState.projectType" :options="typeOptions" />
          </Form.Item>
          <Form.Item label="开始日期">
            <DatePicker style="width: 100%" :value="formState.startDate ? dayjs(formState.startDate) : undefined" format="YYYY-MM-DD" @change="(_, d) => formState.startDate = d" />
          </Form.Item>
          <Form.Item label="结束日期">
            <DatePicker style="width: 100%" :value="formState.endDate ? dayjs(formState.endDate) : undefined" format="YYYY-MM-DD" @change="(_, d) => formState.endDate = d" />
          </Form.Item>
        </div>
        <Form.Item label="状态">
          <Select v-model:value="formState.status" :options="statusOptions.slice(1)" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="项目描述" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="项目详情" width="500px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="项目名称">{{ selectedItem.projectName }}</Descriptions.Item>
          <Descriptions.Item label="项目编号">{{ selectedItem.projectCode || '-' }}</Descriptions.Item>
          <Descriptions.Item label="项目类型">{{ WorkProjectTypeLabel[selectedItem.projectType as WorkProjectType] }}</Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="WorkProjectStatusColor[selectedItem.status as WorkProjectStatus]">
              {{ WorkProjectStatusLabel[selectedItem.status as WorkProjectStatus] }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="开始日期">{{ selectedItem.startDate || '-' }}</Descriptions.Item>
          <Descriptions.Item label="结束日期">{{ selectedItem.endDate || '-' }}</Descriptions.Item>
          <Descriptions.Item label="描述">{{ selectedItem.description || '-' }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

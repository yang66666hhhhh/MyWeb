<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
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

import type { WorkDevice } from '#/api/growth/work';
import {
  createWorkDeviceApi,
  deleteWorkDeviceApi,
  getWorkDeviceApi,
  getWorkDevicePageApi,
  updateWorkDeviceApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';
import { WorkDeviceStatus, WorkDeviceStatusColor, WorkDeviceStatusLabel, WorkDeviceType, WorkDeviceTypeLabel } from '#/enums/workEnum';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkDevice | null>(null);

const formState = ref({
  description: '',
  deviceCode: '',
  deviceName: '',
  deviceType: 0,
  projectId: '',
  status: 0,
});

const projectOptions = [
  { label: '生产线升级项目', value: 'project-1' },
  { label: '质量改进项目', value: 'project-2' },
];

const typeOptions = [
  { label: '生产线', value: 0 },
  { label: '设备', value: 1 },
  { label: '测试设备', value: 2 },
  { label: '其他', value: 3 },
];

const statusOptions = [
  { label: '正常', value: 0 },
  { label: '停用', value: 1 },
  { label: '维护中', value: 2 },
];

const columns = [
  { dataIndex: 'deviceName', key: 'deviceName', title: '设备名称', minWidth: 140 },
  { dataIndex: 'deviceCode', key: 'deviceCode', title: '设备编号', width: 120 },
  { dataIndex: 'deviceType', key: 'deviceType', title: '设备类型', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 90 },
  { dataIndex: 'projectId', key: 'projectId', title: '所属项目', width: 140 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkDevice,
  { deviceType?: number; keyword?: string; page: number; pageSize: number; projectId?: string; status?: number }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getWorkDevicePageApi,
});

function openCreate() {
  editingId.value = null;
  formState.value = {
    description: '',
    deviceCode: '',
    deviceName: '',
    deviceType: 0,
    projectId: '',
    status: 0,
  };
  formOpen.value = true;
}

async function openEdit(record: WorkDevice) {
  editingId.value = record.id;
  const detail = await getWorkDeviceApi(record.id);
  if (detail) {
    formState.value = {
      description: detail.description || '',
      deviceCode: detail.deviceCode || '',
      deviceName: detail.deviceName,
      deviceType: detail.deviceType ?? 0,
      projectId: detail.projectId || '',
      status: detail.status,
    };
  }
  formOpen.value = true;
}

function showDetail(record: WorkDevice) {
  selectedItem.value = record;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  await deleteWorkDeviceApi(id);
  message.success('设备已删除');
  await load();
}

async function handleSubmit() {
  if (!formState.value.deviceName.trim()) {
    message.warning('请填写设备名称');
    return;
  }
  if (editingId.value) {
    await updateWorkDeviceApi(editingId.value, formState.value);
    message.success('设备已更新');
  } else {
    await createWorkDeviceApi(formState.value);
    message.success('设备已创建');
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
  <Page description="管理设备/线体信息" title="设备管理">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="设备类型">
              <Select v-model:value="query.deviceType" :options="typeOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input v-model:value="query.keyword" allow-clear placeholder="设备名称/编号" style="width: 180px" @press-enter="search" />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增设备</Button>
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
          :scroll="{ x: 900 }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'deviceName'">
              <div class="font-medium">{{ record.deviceName }}</div>
              <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
                {{ record.description }}
              </div>
            </template>
            <template v-else-if="column.key === 'deviceType'">
              {{ WorkDeviceTypeLabel[text as WorkDeviceType] }}
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="WorkDeviceStatusColor[text as WorkDeviceStatus]">
                {{ WorkDeviceStatusLabel[text as WorkDeviceStatus] }}
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
      :title="editingId ? '编辑设备' : '新增设备'"
      width="560px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="设备名称" required>
            <Input v-model:value="formState.deviceName" placeholder="设备名称" />
          </Form.Item>
          <Form.Item label="设备编号">
            <Input v-model:value="formState.deviceCode" placeholder="设备编号" />
          </Form.Item>
          <Form.Item label="设备类型">
            <Select v-model:value="formState.deviceType" :options="typeOptions" />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="formState.status" :options="statusOptions" />
          </Form.Item>
        </div>
        <Form.Item label="所属项目">
          <Select v-model:value="formState.projectId" :options="projectOptions" allow-clear placeholder="选择项目" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="设备描述" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="设备详情" width="450px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="设备名称">{{ selectedItem.deviceName }}</Descriptions.Item>
          <Descriptions.Item label="设备编号">{{ selectedItem.deviceCode || '-' }}</Descriptions.Item>
          <Descriptions.Item label="设备类型">{{ WorkDeviceTypeLabel[selectedItem.deviceType as WorkDeviceType] }}</Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="WorkDeviceStatusColor[selectedItem.status as WorkDeviceStatus]">
              {{ WorkDeviceStatusLabel[selectedItem.status as WorkDeviceStatus] }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="描述">{{ selectedItem.description || '-' }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

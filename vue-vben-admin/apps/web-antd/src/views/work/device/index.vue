<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

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

import type { WorkDevice } from '#/api/work/device';
import {
  createWorkDeviceApi,
  deleteWorkDeviceApi,
  getWorkDeviceApi,
  getWorkDevicePageApi,
  updateWorkDeviceApi,
} from '#/api/work/device';
import { projectApi } from '#/api/work/project';
import { usePagedQuery } from '#/composables/usePagedQuery';
import { WorkDeviceStatus, WorkDeviceStatusColor, WorkDeviceStatusLabel, WorkDeviceType, WorkDeviceTypeLabel } from '#/enums/workEnum';

const formRef = ref();
const formRules = {
  deviceName: [{ required: true, message: '请输入设备名称', type: 'string' as const, trigger: 'blur' as const }],
};

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkDevice | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateDevice = computed(() => accessStore.accessCodes.includes('WORK_DEVICE'));
const canEditDevice = computed(() => accessStore.accessCodes.includes('WORK_DEVICE'));
const canDeleteDevice = computed(() => accessStore.accessCodes.includes('WORK_DEVICE'));

const formState = ref({
  description: '',
  deviceCode: '',
  deviceName: '',
  deviceType: 0,
  projectId: '',
  status: 0,
});

const projectOptions = ref<Array<{ label: string; value: string }>>([]);

async function loadProjects() {
  try {
    const res = await projectApi.getPage({ page: 1, pageSize: 100 });
    projectOptions.value = res.items.map(p => ({ label: p.projectName, value: p.id }));
  } catch {
    // ignore
  }
}

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
  { key: 'action', title: '操作', width: 180, fixed: 'right' as const },
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

async function openEdit(record: Record<string, any>) {
  const device = record as WorkDevice;
  editingId.value = device.id;
  try {
    const detail = await getWorkDeviceApi(device.id);
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
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载详情失败');
  }
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as WorkDevice;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  try {
    await deleteWorkDeviceApi(id);
    message.success('设备已删除');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

async function handleSubmit() {
  try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateWorkDeviceApi(editingId.value, formState.value);
      message.success('设备已更新');
    } else {
      await createWorkDeviceApi(formState.value);
      message.success('设备已创建');
    }
    formOpen.value = false;
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '保存失败');
  } finally {
    submitting.value = false;
  }
}

function resetFilters() {
  resetQuery();
  void load();
}

onMounted(() => {
  void loadProjects();
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
          <Button v-if="canCreateDevice" type="primary" @click="openCreate">新增设备</Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="items"
          :loading="loading"
          :locale="{ emptyText: '暂无数据' }"
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
                <Button v-if="canEditDevice" size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm v-if="canDeleteDevice" title="确认删除？" @confirm="handleRemove(record.id)">
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
      :confirm-loading="submitting"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
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

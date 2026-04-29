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
  Switch,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import type { WorkTaskType } from '#/api/growth/work';
import {
  createWorkTaskTypeApi,
  deleteWorkTaskTypeApi,
  getWorkTaskTypeApi,
  getWorkTaskTypePageApi,
  updateWorkTaskTypeApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkTaskType | null>(null);

const formState = ref({
  description: '',
  enabled: true,
  sort: 0,
  typeCode: '',
  typeName: '',
});

const columns = [
  { dataIndex: 'typeName', key: 'typeName', title: '类型名称', minWidth: 140 },
  { dataIndex: 'typeCode', key: 'typeCode', title: '类型编码', width: 120 },
  { dataIndex: 'description', key: 'description', title: '描述', minWidth: 160 },
  { dataIndex: 'sort', key: 'sort', title: '排序', width: 80 },
  { dataIndex: 'enabled', key: 'enabled', title: '启用', width: 80 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkTaskType,
  { enabled?: boolean; keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getWorkTaskTypePageApi,
});

function openCreate() {
  editingId.value = null;
  formState.value = {
    description: '',
    enabled: true,
    sort: 0,
    typeCode: '',
    typeName: '',
  };
  formOpen.value = true;
}

async function openEdit(record: WorkTaskType) {
  editingId.value = record.id;
  const detail = await getWorkTaskTypeApi(record.id);
  if (detail) {
    formState.value = {
      description: detail.description || '',
      enabled: detail.enabled,
      sort: detail.sort || 0,
      typeCode: detail.typeCode || '',
      typeName: detail.typeName,
    };
  }
  formOpen.value = true;
}

function showDetail(record: WorkTaskType) {
  selectedItem.value = record;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  await deleteWorkTaskTypeApi(id);
  message.success('任务类型已删除');
  await load();
}

async function handleSubmit() {
  if (!formState.value.typeName.trim()) {
    message.warning('请填写类型名称');
    return;
  }
  if (editingId.value) {
    await updateWorkTaskTypeApi(editingId.value, formState.value);
    message.success('任务类型已更新');
  } else {
    await createWorkTaskTypeApi(formState.value);
    message.success('任务类型已创建');
  }
  formOpen.value = false;
  await load();
}

async function handleToggleEnabled(record: WorkTaskType) {
  await updateWorkTaskTypeApi(record.id, { enabled: !record.enabled });
  message.success(`已${record.enabled ? '停用' : '启用'}`);
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
  <Page description="管理任务类型，支持启用/停用" title="任务类型管理">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="启用状态">
              <Select
                v-model:value="query.enabled"
                :options="[
                  { label: '全部', value: undefined },
                  { label: '已启用', value: true },
                  { label: '已停用', value: false },
                ]"
                allow-clear
                class="w-32"
              />
            </Form.Item>
            <Form.Item label="关键词">
              <Input v-model:value="query.keyword" allow-clear placeholder="类型名称/编码" style="width: 180px" @press-enter="search" />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增类型</Button>
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
          :scroll="{ x: 800 }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'typeName'">
              <div class="font-medium">{{ record.typeName }}</div>
            </template>
            <template v-else-if="column.key === 'enabled'">
              <Switch :checked="text" size="small" @click="handleToggleEnabled(record)" />
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
      :title="editingId ? '编辑类型' : '新增类型'"
      width="480px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="类型名称" required>
            <Input v-model:value="formState.typeName" placeholder="类型名称" />
          </Form.Item>
          <Form.Item label="类型编码">
            <Input v-model:value="formState.typeCode" placeholder="类型编码" />
          </Form.Item>
          <Form.Item label="排序">
            <Input v-model:value="formState.sort" type="number" />
          </Form.Item>
          <Form.Item label="启用">
            <Switch v-model:checked="formState.enabled" />
          </Form.Item>
        </div>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="类型描述" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="类型详情" width="400px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="类型名称">{{ selectedItem.typeName }}</Descriptions.Item>
          <Descriptions.Item label="类型编码">{{ selectedItem.typeCode || '-' }}</Descriptions.Item>
          <Descriptions.Item label="排序">{{ selectedItem.sort || 0 }}</Descriptions.Item>
          <Descriptions.Item label="启用">
            <Tag :color="selectedItem.enabled ? 'success' : 'default'">
              {{ selectedItem.enabled ? '已启用' : '已停用' }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="描述">{{ selectedItem.description || '-' }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

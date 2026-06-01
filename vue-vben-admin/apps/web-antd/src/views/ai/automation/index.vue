<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  Form,
  Input,
  List,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Switch,
  Tag,
} from 'ant-design-vue';

import type {
  AutomationWorkflow,
  CreateAutomationWorkflowInput,
} from '#/api/ai/extended';

import {
  createWorkflowApi,
  deleteWorkflowApi,
  getWorkflowsApi,
  updateWorkflowApi,
} from '#/api/ai/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const accessStore = useAccessStore();
const canCreateWorkflow = computed(() => accessStore.accessCodes.includes('AI_AUTOMATION'));
const canEditWorkflow = computed(() => accessStore.accessCodes.includes('AI_AUTOMATION'));
const canDeleteWorkflow = computed(() => accessStore.accessCodes.includes('AI_AUTOMATION'));

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const submitting = ref(false);
const formData = ref<CreateAutomationWorkflowInput>({
  name: '',
  description: '',
  triggerType: '',
  actions: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入工作流名称', type: 'string' as const }],};

const { items, load, loading, query, search, total, changePage } = usePagedQuery<
  AutomationWorkflow,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getWorkflowsApi,
});

const triggerOptions = [
  { label: '定时触发', value: '定时触发' },
  { label: '事件触发', value: '事件触发' },
  { label: '手动触发', value: '手动触发' },
];

const statusColors: Record<string, string> = {
  true: 'success',
  false: 'warning',
};

function openCreate() {
  editingId.value = null;
  formData.value = { name: '', description: '', triggerType: '', actions: '' };
  formOpen.value = true;
}

function openEdit(record: AutomationWorkflow) {
  editingId.value = record.id;
  formData.value = {
    name: record.name,
    description: record.description || '',
    triggerType: record.triggerType || '',
    actions: record.actions || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateWorkflowApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createWorkflowApi(formData.value);
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
    await deleteWorkflowApi(id);
    message.success('删除成功');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

async function handleToggleStatus(record: AutomationWorkflow) {
  try {
    await updateWorkflowApi(record.id, { isActive: !record.isActive });
    message.success(record.isActive ? '已暂停' : '已启动');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  }
}

function handlePageChange(page: number, pageSize: number) {
  void changePage(page, pageSize);
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="创建和管理自动化工作流" title="自动化工作流">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="工作流总数" :value="total" /></Card>
      </Col>
        <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="运行中" :value="items.filter((i: AutomationWorkflow) => i.isActive).length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已暂停" :value="items.filter((i: AutomationWorkflow) => !i.isActive).length" />
        </Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="工作流名称" @press-enter="search" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button v-if="canCreateWorkflow" @click="openCreate">创建工作流</Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>

    <Card title="工作流列表">
      <List
        :data-source="items"
        :loading="loading"
        :pagination="{
          current: query.page,
          pageSize: query.pageSize,
          total,
          showSizeChanger: true,
          showTotal: (value: number) => `共 ${value} 条`,
          onChange: handlePageChange,
        }"
      >
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name">
              <template #description>
                <div>
                  <div v-if="item.triggerType">触发: {{ item.triggerType }}</div>
                  <div v-if="item.actions">动作: {{ item.actions }}</div>
                  <div v-if="item.lastRunAt">最后执行: {{ item.lastRunAt }}</div>
                </div>
              </template>
            </List.Item.Meta>
            <Space>
              <Tag :color="statusColors[String(item.isActive)]">
                {{ item.isActive ? '运行中' : '已暂停' }}
              </Tag>
              <Switch
                :checked="item.isActive"
                checked-children="启动"
                un-checked-children="暂停"
                @change="handleToggleStatus(item)"
              />
              <Button v-if="canEditWorkflow" type="link" @click="openEdit(item)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(item.id)">
                <Button v-if="canDeleteWorkflow" danger type="link">删除</Button>
              </Popconfirm>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑工作流' : '创建工作流'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="工作流名称" required>
          <Input v-model:value="formData.name" placeholder="请输入工作流名称" />
        </Form.Item>
        <Form.Item label="触发类型">
          <Select v-model:value="formData.triggerType" :options="triggerOptions" placeholder="请选择触发类型" />
        </Form.Item>
        <Form.Item label="动作">
          <Input v-model:value="formData.actions" placeholder="描述工作流动作" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formData.description" placeholder="工作流描述" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

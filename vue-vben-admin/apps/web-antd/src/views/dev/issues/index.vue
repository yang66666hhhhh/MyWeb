<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  SelectOption,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateIssueInput, Issue } from '#/api/persona';

import {
  createIssueApi,
  deleteIssueApi,
  getIssuesApi,
  updateIssueApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<Issue[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateIssueInput>({
  title: '',
  description: '',
  repository: '',
  priority: 1,
  assignee: '',
  labels: '',
});

const priorityOptions = [
  { label: '高', value: 3 },
  { label: '中', value: 2 },
  { label: '低', value: 1 },
];

const statusMap: Record<number, string> = {
  0: '待处理',
  1: '进行中',
  2: '已解决',
  3: '已关闭',
};

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title', ellipsis: true },
  { title: '仓库', dataIndex: 'repository', key: 'repository', width: 120 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 80 },
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 80 },
  { title: '负责人', dataIndex: 'assignee', key: 'assignee', width: 100 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<number, string> = {
  0: 'blue',
  1: 'orange',
  2: 'green',
  3: 'default',
};

const priorityColors: Record<number, string> = {
  3: 'red',
  2: 'yellow',
  1: 'green',
};

const priorityLabels: Record<number, string> = {
  3: '高',
  2: '中',
  1: '低',
};

const openCount = computed(() => dataList.value.filter((i) => i.status === 0).length);
const inProgressCount = computed(() => dataList.value.filter((i) => i.status === 1).length);
const resolvedCount = computed(() => dataList.value.filter((i) => i.status === 2).length);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getIssuesApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch {
    // ignore
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    title: '',
    description: '',
    repository: '',
    priority: 1,
    assignee: '',
    labels: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Issue) => {
  editingId.value = record.id;
  Object.assign(formState, {
    title: record.title,
    description: record.description || '',
    repository: record.repository,
    priority: record.priority,
    assignee: record.assignee || '',
    labels: record.labels || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteIssueApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  if (!formState.title || !formState.repository) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateIssueApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createIssueApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch {
    message.error('操作失败');
  } finally {
    submitting.value = false;
  }
};

const handlePageChange = (page: number, size: number) => {
  currentPage.value = page;
  pageSize.value = size;
  fetchData();
};

onMounted(() => {
  fetchData();
});
</script>

<template>
  <Page description="跟踪和管理项目中的问题和任务" title="问题跟踪">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="待处理" :value="openCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="进行中" :value="inProgressCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="已解决" :value="resolvedCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总数" :value="total" />
        </Card>
      </Col>
    </Row>

    <Card title="问题列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">新建问题</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="dataList"
        :loading="loading"
        :pagination="{
          current: currentPage,
          pageSize,
          total,
          showSizeChanger: true,
          showTotal: (t: number) => `共 ${t} 条`,
          onChange: handlePageChange,
        }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ statusMap[record.status] || '未知' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'priority'">
            <Tag :color="priorityColors[record.priority] || 'default'">
              {{ priorityLabels[record.priority] || '未知' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Issue)">编辑</Button>
              <Popconfirm title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑问题' : '新建问题'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form layout="vertical">
        <FormItem label="标题" required>
          <Input v-model:value="formState.title" placeholder="问题标题" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="问题描述" :rows="3" />
        </FormItem>
        <FormItem label="仓库" required>
          <Input v-model:value="formState.repository" placeholder="所属仓库" />
        </FormItem>
        <FormItem label="优先级">
          <Select v-model:value="formState.priority" placeholder="选择优先级">
            <SelectOption v-for="opt in priorityOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="负责人">
          <Input v-model:value="formState.assignee" placeholder="负责人" />
        </FormItem>
        <FormItem label="标签">
          <Input v-model:value="formState.labels" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

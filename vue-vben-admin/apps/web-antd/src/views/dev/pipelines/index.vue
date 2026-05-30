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
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreatePipelineInput, Pipeline } from '#/api/persona';

import {
  createPipelineApi,
  deletePipelineApi,
  getPipelinesApi,
  updatePipelineApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<Pipeline[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreatePipelineInput>({
  name: '',
  description: '',
  repository: '',
  branch: 'main',
  triggerType: 'push',
  steps: '',
});

const triggerTypeOptions = ['push', 'manual', 'schedule', 'merge_request'];

const statusMap: Record<number, string> = {
  0: '待运行',
  1: '运行中',
  2: '成功',
  3: '失败',
};

const columns = [
  { title: '流水线名称', dataIndex: 'name', key: 'name' },
  { title: '仓库', dataIndex: 'repository', key: 'repository', width: 120 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '分支', dataIndex: 'branch', key: 'branch', width: 100 },
  { title: '触发方式', dataIndex: 'triggerType', key: 'triggerType', width: 100 },
  { title: '最后运行', dataIndex: 'lastRunAt', key: 'lastRunAt', width: 150 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'blue',
  2: 'green',
  3: 'red',
};

const statusIcons: Record<number, string> = {
  0: '○',
  1: '⟳',
  2: '✓',
  3: '✗',
};

const successCount = computed(() => dataList.value.filter((p) => p.status === 2).length);
const runningCount = computed(() => dataList.value.filter((p) => p.status === 1).length);
const failedCount = computed(() => dataList.value.filter((p) => p.status === 3).length);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getPipelinesApi({ page: currentPage.value, pageSize: pageSize.value });
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
    name: '',
    description: '',
    repository: '',
    branch: 'main',
    triggerType: 'push',
    steps: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Pipeline) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    description: record.description || '',
    repository: record.repository,
    branch: record.branch,
    triggerType: record.triggerType || 'push',
    steps: record.steps || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deletePipelineApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  if (!formState.name || !formState.repository || !formState.branch) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updatePipelineApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createPipelineApi(formState);
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
  <Page description="查看和管理 CI/CD 流水线" title="流水线">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="成功" :value="successCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="运行中" :value="runningCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="失败" :value="failedCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总流水线" :value="total" />
        </Card>
      </Col>
    </Row>

    <Card title="流水线列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">新建流水线</Button>
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
            <Space>
              <span>{{ statusIcons[record.status] || '○' }}</span>
              <Tag :color="statusColors[record.status] || 'default'">
                {{ statusMap[record.status] || '未知' }}
              </Tag>
            </Space>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Pipeline)">编辑</Button>
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
      :title="editingId ? '编辑流水线' : '新建流水线'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form layout="vertical">
        <FormItem label="流水线名称" required>
          <Input v-model:value="formState.name" placeholder="流水线名称" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="流水线描述" :rows="2" />
        </FormItem>
        <FormItem label="仓库" required>
          <Input v-model:value="formState.repository" placeholder="所属仓库" />
        </FormItem>
        <FormItem label="分支" required>
          <Input v-model:value="formState.branch" placeholder="分支名称" />
        </FormItem>
        <FormItem label="触发方式">
          <Select v-model:value="formState.triggerType" placeholder="选择触发方式">
            <SelectOption v-for="t in triggerTypeOptions" :key="t" :value="t">{{ t }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="步骤配置">
          <Input.TextArea v-model:value="formState.steps" placeholder="JSON 格式的步骤配置" :rows="3" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

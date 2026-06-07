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
  Space,
  Statistic,
  Table,
  Tag,
  Tooltip,
} from 'ant-design-vue';

import type { CreatePrototypeInput, Prototype } from '#/api/persona';

import {
  createPrototypeApi,
  deletePrototypeApi,
  getPrototypesApi,
  updatePrototypeApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<Prototype[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreatePrototypeInput>({
  title: '',
  description: '',
  project: '',
  previewUrl: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入原型名称', type: 'string' as const }],
  project: [{ required: true, message: '请输入所属项目', type: 'string' as const }],};

const statusMap: Record<number, string> = {
  0: '草稿',
  1: '进行中',
  2: '已完成',
  3: '待审核',
};

const previewVisible = ref(false);
const previewUrl = ref('');

const handlePreview = (url: string) => {
  previewUrl.value = url;
  previewVisible.value = true;
};

const columns = [
  { title: '原型名称', dataIndex: 'title', key: 'title' },
  { title: '项目', dataIndex: 'project', key: 'project', width: 120 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 80 },
  { title: '预览', key: 'preview', width: 80 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'blue',
  2: 'green',
  3: 'orange',
};

const completedCount = computed(() => dataList.value.filter((p) => p.status === 2).length);
const inProgressCount = computed(() => dataList.value.filter((p) => p.status === 1).length);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getPrototypesApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    title: '',
    description: '',
    project: '',
    previewUrl: '',
    tags: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Prototype) => {
  editingId.value = record.id;
  Object.assign(formState, {
    title: record.title,
    description: record.description || '',
    project: record.project,
    previewUrl: record.previewUrl || '',
    tags: record.tags || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deletePrototypeApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.title || !formState.project) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updatePrototypeApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createPrototypeApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
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
  <Page description="管理产品原型和交互设计" title="原型管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="原型总数" :value="total" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="已完成" :value="completedCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="进行中" :value="inProgressCount" />
        </Card>
      </Col>
    </Row>

    <Card title="原型列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">新建原型</Button>
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
          <template v-else-if="column.key === 'preview'">
            <Tooltip v-if="record.previewUrl" title="点击预览">
              <Button type="link" size="small" @click="handlePreview(record.previewUrl)">
                预览
              </Button>
            </Tooltip>
            <span v-else class="text-gray-400">-</span>
          </template>
          <template v-else-if="column.key === 'tags'">
            <Space>
              <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag">{{ tag }}</Tag>
            </Space>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Prototype)">编辑</Button>
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
      :title="editingId ? '编辑原型' : '新建原型'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="原型名称" required>
          <Input v-model:value="formState.title" placeholder="原型名称" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="原型描述" :rows="2" />
        </FormItem>
        <FormItem label="所属项目" required>
          <Input v-model:value="formState.project" placeholder="项目名称" />
        </FormItem>
        <FormItem label="预览地址">
          <Input v-model:value="formState.previewUrl" placeholder="原型预览 URL" />
        </FormItem>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>

    <Modal
      v-model:open="previewVisible"
      title="原型预览"
      :footer="null"
      width="80%"
    >
      <div class="w-full" style="height: 70vh">
        <iframe
          v-if="previewUrl"
          :src="previewUrl"
          class="w-full h-full border-0"
          allow="fullscreen"
        />
      </div>
    </Modal>
  </Page>
</template>

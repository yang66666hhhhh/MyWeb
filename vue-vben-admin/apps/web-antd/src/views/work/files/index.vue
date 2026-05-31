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

import type { WorkFile } from '#/api/work/extended';
import {
  createFileApi,
  deleteFileApi,
  getFilesApi,
  updateFileApi,
} from '#/api/work/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formRef = ref();
const formRules = {
  fileName: [{ required: true, message: '请输入文件名', type: 'string' as const, trigger: 'blur' as const }],
};

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkFile | null>(null);

const formState = ref({
  category: '',
  description: '',
  fileName: '',
  fileSize: 0,
  fileType: '',
  fileUrl: '',
  tags: '',
});

const columns: any[] = [
  { dataIndex: 'fileName', key: 'fileName', title: '文件名', minWidth: 200 },
  { dataIndex: 'fileType', key: 'fileType', title: '类型', width: 90 },
  { dataIndex: 'fileSize', key: 'fileSize', title: '大小', width: 100 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '上传时间', width: 160 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const categoryOptions = [
  { label: '需求', value: '需求' },
  { label: '设计', value: '设计' },
  { label: '技术', value: '技术' },
  { label: '测试', value: '测试' },
  { label: '会议', value: '会议' },
  { label: '其他', value: '其他' },
];

const fileTypeOptions = [
  { label: '文档', value: '文档' },
  { label: '图片', value: '图片' },
  { label: '表格', value: '表格' },
  { label: '代码', value: '代码' },
  { label: '其他', value: '其他' },
];

const typeColors: Record<string, string> = {
  '文档': 'blue',
  '图片': 'green',
  '表格': 'orange',
  '代码': 'purple',
};

const categoryColors: Record<string, string> = {
  '需求': 'blue',
  '设计': 'purple',
  '技术': 'cyan',
  '测试': 'green',
  '会议': 'gold',
};

function formatFileSize(bytes: number) {
  if (!bytes) return '-';
  if (bytes < 1024) return `${bytes}B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)}KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)}MB`;
}

const { changePage, items, load, loading, query, resetQuery, search, total } =
  usePagedQuery<
    WorkFile,
    {
      category?: string;
      keyword?: string;
      page: number;
      pageSize: number;
    }
  >({
    defaultQuery: { page: 1, pageSize: 10 },
    fetcher: getFilesApi,
  });

function openCreate() {
  editingId.value = null;
  formState.value = {
    category: '',
    description: '',
    fileName: '',
    fileSize: 0,
    fileType: '文档',
    fileUrl: '',
    tags: '',
  };
  formOpen.value = true;
}

async function openEdit(record: Record<string, any>) {
  const file = record as WorkFile;
  editingId.value = file.id;
  formState.value = {
    category: file.category || '',
    description: file.description || '',
    fileName: file.fileName || '',
    fileSize: file.fileSize || 0,
    fileType: file.fileType || '',
    fileUrl: file.fileUrl || '',
    tags: file.tags || '',
  };
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as WorkFile;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  try {
    await deleteFileApi(id);
    message.success('文件已删除');
    await load();
  } catch {
    message.error('删除失败');
  }
}

async function handleSubmit() {
  try { await formRef.value?.validate(); } catch { return; }
  try {
    if (editingId.value) {
      await updateFileApi(editingId.value, formState.value);
      message.success('文件信息已更新');
    } else {
      await createFileApi(formState.value as any);
      message.success('文件已创建');
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
  <Page description="管理项目文件和文档" title="文件管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="文件总数" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="文档" :value="items.filter((i: WorkFile) => i.fileType === '文档').length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="图片" :value="items.filter((i: WorkFile) => i.fileType === '图片').length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic
            title="总大小"
            :value="formatFileSize(items.reduce((s: number, i: WorkFile) => s + (i.fileSize || 0), 0))"
          />
        </Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <Form :model="query" layout="inline">
          <Form.Item label="分类">
            <Select v-model:value="query.category" :options="categoryOptions" allow-clear class="w-28" />
          </Form.Item>
          <Form.Item label="关键词">
            <Input v-model:value="query.keyword" allow-clear placeholder="文件名/描述" style="width: 180px" @press-enter="search" />
          </Form.Item>
          <Form.Item>
            <Space>
              <Button type="primary" @click="search">查询</Button>
              <Button @click="resetFilters">重置</Button>
            </Space>
          </Form.Item>
        </Form>
        <Button type="primary" @click="openCreate">上传文件</Button>
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
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'fileName'">
            <div class="font-medium">{{ record.fileName }}</div>
            <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'fileType'">
            <Tag :color="typeColors[record.fileType]">{{ record.fileType }}</Tag>
          </template>
          <template v-else-if="column.key === 'fileSize'">
            {{ formatFileSize(record.fileSize) }}
          </template>
          <template v-else-if="column.key === 'category'">
            <Tag v-if="record.category" :color="categoryColors[record.category]">{{ record.category }}</Tag>
            <span v-else>-</span>
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
      :title="editingId ? '编辑文件' : '上传文件'"
      width="560px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <Form.Item label="文件名" required>
          <Input v-model:value="formState.fileName" placeholder="文件名" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="文件类型">
            <Select v-model:value="formState.fileType" :options="fileTypeOptions" />
          </Form.Item>
          <Form.Item label="分类">
            <Select v-model:value="formState.category" :options="categoryOptions" allow-clear />
          </Form.Item>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="文件URL">
            <Input v-model:value="formState.fileUrl" placeholder="文件地址" />
          </Form.Item>
          <Form.Item label="文件大小 (bytes)">
            <InputNumber v-model:value="formState.fileSize" :min="0" class="w-full" />
          </Form.Item>
        </div>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="文件描述" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="文件详情" width="450px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="文件名">{{ selectedItem.fileName }}</Descriptions.Item>
          <Descriptions.Item label="文件类型">
            <Tag :color="typeColors[selectedItem.fileType]">{{ selectedItem.fileType }}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="文件大小">{{ formatFileSize(selectedItem.fileSize) }}</Descriptions.Item>
          <Descriptions.Item label="分类">
            <Tag v-if="selectedItem.category" :color="categoryColors[selectedItem.category]">{{ selectedItem.category }}</Tag>
            <span v-else>-</span>
          </Descriptions.Item>
          <Descriptions.Item label="文件URL">{{ selectedItem.fileUrl || '-' }}</Descriptions.Item>
          <Descriptions.Item label="描述">{{ selectedItem.description || '-' }}</Descriptions.Item>
          <Descriptions.Item label="标签">{{ selectedItem.tags || '-' }}</Descriptions.Item>
          <Descriptions.Item label="创建时间">{{ selectedItem.createdAt }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

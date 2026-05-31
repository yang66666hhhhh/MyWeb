<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  InputNumber,
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

import type { CreateMediaItemInput, MediaItem } from '#/api/content';

import { createMediaApi, deleteMediaApi, getMediaPageApi, updateMediaApi } from '#/api/content';

const loading = ref(false);
const dataList = ref<MediaItem[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateMedia = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canEditMedia = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canDeleteMedia = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));

const formState = reactive<CreateMediaItemInput>({
  fileName: '',
  fileUrl: '',
  fileType: '',
  fileSize: 0,
  tags: '',
  remark: '',
});

const formRef = ref();
const formRules = {
  fileName: [{ required: true, message: '请输入文件名', type: 'string' as const }],
  fileUrl: [{ required: true, message: '请输入文件URL', type: 'string' as const }],
  fileType: [{ required: true, message: '请选择文件类型', type: 'string' as const }],};

const fileTypeOptions = ['image', 'video', 'audio', 'document', 'other'];

const columns = [
  { title: '文件名', dataIndex: 'fileName', key: 'fileName', ellipsis: true },
  { title: '类型', dataIndex: 'fileType', key: 'fileType', width: 100 },
  { title: '大小', key: 'fileSize', width: 100 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const fileTypeColors: Record<string, string> = {
  image: 'blue',
  video: 'purple',
  audio: 'green',
  document: 'orange',
  other: 'default',
};

const formatFileSize = (size: number) => {
  if (size < 1024) return `${size} B`;
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`;
  if (size < 1024 * 1024 * 1024) return `${(size / (1024 * 1024)).toFixed(1)} MB`;
  return `${(size / (1024 * 1024 * 1024)).toFixed(1)} GB`;
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getMediaPageApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch {
    message.error('加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    fileName: '',
    fileUrl: '',
    fileType: '',
    fileSize: 0,
    tags: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: MediaItem) => {
  editingId.value = record.id;
  Object.assign(formState, {
    fileName: record.fileName,
    fileUrl: record.fileUrl,
    fileType: record.fileType,
    fileSize: record.fileSize,
    tags: record.tags || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteMediaApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.fileName || !formState.fileUrl || !formState.fileType) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateMediaApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createMediaApi(formState);
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
  <Page description="管理媒体文件" title="媒体文件">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="文件总数" :value="total" suffix="个" />
        </Card>
      </Col>
    </Row>

    <Card title="媒体列表">
      <template #extra>
        <Button v-if="canCreateMedia" type="primary" @click="handleAdd">上传文件</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="dataList"
        :loading="loading"
        :locale="{ emptyText: '暂无数据' }"
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
          <template v-if="column.key === 'fileType'">
            <Tag :color="fileTypeColors[record.fileType]">{{ record.fileType }}</Tag>
          </template>
          <template v-else-if="column.key === 'fileSize'">
            {{ formatFileSize(record.fileSize) }}
          </template>
          <template v-else-if="column.key === 'tags'">
            <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag">{{ tag }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditMedia" type="link" size="small" @click="handleEdit(record as MediaItem)">编辑</Button>
              <Popconfirm v-if="canDeleteMedia" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑文件' : '上传文件'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="文件名" required>
          <Input v-model:value="formState.fileName" placeholder="文件名" />
        </FormItem>
        <FormItem label="文件URL" required>
          <Input v-model:value="formState.fileUrl" placeholder="文件URL" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="文件类型" required>
              <Select v-model:value="formState.fileType" placeholder="选择类型">
                <SelectOption v-for="t in fileTypeOptions" :key="t" :value="t">{{ t }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="文件大小 (字节)">
              <InputNumber v-model:value="formState.fileSize" :min="0" style="width: 100%" />
            </FormItem>
          </Col>
        </Row>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" :rows="2" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

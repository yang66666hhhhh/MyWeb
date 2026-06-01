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

import type { Article, CreateArticleInput } from '#/api/content';

import { createArticleApi, deleteArticleApi, getArticlePageApi, updateArticleApi } from '#/api/content';

const loading = ref(false);
const dataList = ref<Article[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateArticle = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canEditArticle = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canDeleteArticle = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));

const formState = reactive<CreateArticleInput>({
  title: '',
  content: '',
  status: 'draft',
  tags: '',
  category: '',
  remark: '',
});

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入标题', type: 'string' as const }],};

const statusOptions = [
  { value: 'draft', label: '草稿' },
  { value: 'published', label: '已发布' },
  { value: 'archived', label: '已归档' },
];

const categoryOptions = ['技术', '生活', '工作', '学习', '其他'];

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title', ellipsis: true },
  { title: '分类', dataIndex: 'category', key: 'category', width: 100 },
  { title: '状态', key: 'status', width: 100 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '发布时间', dataIndex: 'publishedAt', key: 'publishedAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<string, string> = {
  draft: 'default',
  published: 'success',
  archived: 'warning',
};

const statusLabels: Record<string, string> = {
  draft: '草稿',
  published: '已发布',
  archived: '已归档',
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getArticlePageApi({ page: currentPage.value, pageSize: pageSize.value });
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
    content: '',
    status: 'draft',
    tags: '',
    category: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Article) => {
  editingId.value = record.id;
  Object.assign(formState, {
    title: record.title,
    content: record.content || '',
    status: record.status,
    tags: record.tags || '',
    category: record.category || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteArticleApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.title) {
    message.warning('请填写标题');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateArticleApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createArticleApi(formState);
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
  <Page description="管理文章内容" title="文章管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="文章总数" :value="total" suffix="篇" />
        </Card>
      </Col>
    </Row>

    <Card title="文章列表">
      <template #extra>
        <Button v-if="canCreateArticle" type="primary" @click="handleAdd">新建文章</Button>
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
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ statusLabels[record.status] || record.status }}</Tag>
          </template>
          <template v-else-if="column.key === 'tags'">
            <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag">{{ tag }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditArticle" type="link" size="small" @click="handleEdit(record as Article)">编辑</Button>
              <Popconfirm v-if="canDeleteArticle" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑文章' : '新建文章'"
      :confirm-loading="submitting"
      width="700px"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="标题" required>
          <Input v-model:value="formState.title" placeholder="文章标题" />
        </FormItem>
        <FormItem label="内容">
          <Input.TextArea v-model:value="formState.content" placeholder="文章内容" :rows="6" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="分类">
              <Select v-model:value="formState.category" placeholder="选择分类" allow-clear>
                <SelectOption v-for="cat in categoryOptions" :key="cat" :value="cat">{{ cat }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="状态">
              <Select v-model:value="formState.status">
                <SelectOption v-for="opt in statusOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</SelectOption>
              </Select>
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

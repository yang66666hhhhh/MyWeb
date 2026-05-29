<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  DatePicker,
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

import type { CreatePublishingCalendarInput, PublishingCalendar } from '#/api/content';

import { createCalendarApi, deleteCalendarApi, getCalendarPageApi, updateCalendarApi } from '#/api/content';

const loading = ref(false);
const dataList = ref<PublishingCalendar[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateCalendar = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canEditCalendar = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canDeleteCalendar = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));

const formState = reactive<CreatePublishingCalendarInput>({
  plannedDate: '',
  platform: '',
  title: '',
  status: 'pending',
  remark: '',
});

const platformOptions = ['微信公众号', '知乎', '掘金', 'CSDN', '博客', '微博', '其他'];

const statusOptions = [
  { value: 'pending', label: '待发布' },
  { value: 'published', label: '已发布' },
  { value: 'cancelled', label: '已取消' },
];

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title', ellipsis: true },
  { title: '平台', dataIndex: 'platform', key: 'platform', width: 120 },
  { title: '计划日期', dataIndex: 'plannedDate', key: 'plannedDate', width: 120 },
  { title: '状态', key: 'status', width: 100 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<string, string> = {
  pending: 'processing',
  published: 'success',
  cancelled: 'default',
};

const statusLabels: Record<string, string> = {
  pending: '待发布',
  published: '已发布',
  cancelled: '已取消',
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getCalendarPageApi({ page: currentPage.value, pageSize: pageSize.value });
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
    plannedDate: '',
    platform: '',
    title: '',
    status: 'pending',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: PublishingCalendar) => {
  editingId.value = record.id;
  Object.assign(formState, {
    plannedDate: record.plannedDate,
    platform: record.platform,
    title: record.title,
    status: record.status,
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteCalendarApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  if (!formState.title || !formState.platform || !formState.plannedDate) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateCalendarApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createCalendarApi(formState);
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
  <Page description="管理内容发布计划" title="发布日历">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="计划总数" :value="total" suffix="条" />
        </Card>
      </Col>
    </Row>

    <Card title="发布计划">
      <template #extra>
        <Button v-if="canCreateCalendar" type="primary" @click="handleAdd">新建计划</Button>
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
            <Tag :color="statusColors[record.status]">{{ statusLabels[record.status] || record.status }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditCalendar" type="link" size="small" @click="handleEdit(record as PublishingCalendar)">编辑</Button>
              <Popconfirm v-if="canDeleteCalendar" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑计划' : '新建计划'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form layout="vertical">
        <FormItem label="标题" required>
          <Input v-model:value="formState.title" placeholder="内容标题" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="平台" required>
              <Select v-model:value="formState.platform" placeholder="选择平台">
                <SelectOption v-for="p in platformOptions" :key="p" :value="p">{{ p }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="计划日期" required>
              <DatePicker v-model:value="formState.plannedDate" style="width: 100%" />
            </FormItem>
          </Col>
        </Row>
        <FormItem label="状态">
          <Select v-model:value="formState.status">
            <SelectOption v-for="opt in statusOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" :rows="2" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

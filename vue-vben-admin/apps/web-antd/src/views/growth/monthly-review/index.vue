<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateMonthlyReviewInput, MonthlyReview, UpdateMonthlyReviewInput } from '#/api/growth/extended';

import {
  createMonthlyReviewApi,
  deleteMonthlyReviewApi,
  getMonthlyReviewsApi,
  updateMonthlyReviewApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateMonthlyReviewInput & UpdateMonthlyReviewInput>({
  year: new Date().getFullYear(),
  month: new Date().getMonth() + 1,
  title: '',
  achievements: '',
  challenges: '',
  lessonsLearned: '',
  nextMonthGoals: '',
  rating: 3,
  tags: '',
});

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  MonthlyReview,
  { keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getMonthlyReviewsApi,
});

const monthOptions = Array.from({ length: 12 }, (_, i) => ({
  label: `${i + 1}月`,
  value: i + 1,
}));

const columns = [
  { key: 'period', title: '月份', width: 120 },
  { dataIndex: 'title', key: 'title', title: '标题' },
  { dataIndex: 'achievements', key: 'achievements', title: '亮点', ellipsis: true },
  { dataIndex: 'challenges', key: 'challenges', title: '挑战', ellipsis: true },
  { dataIndex: 'rating', key: 'rating', title: '评分', width: 100 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 180 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as MonthlyReview[];
  const avgRating = list.length
    ? (list.reduce((s, i) => s + i.rating, 0) / list.length).toFixed(1)
    : '0';
  return { total: total.value, avgRating };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = {
    year: new Date().getFullYear(),
    month: new Date().getMonth() + 1,
    title: '',
    achievements: '',
    challenges: '',
    lessonsLearned: '',
    nextMonthGoals: '',
    rating: 3,
    tags: '',
  };
  formOpen.value = true;
}

function openEdit(record: MonthlyReview) {
  editingId.value = record.id;
  formData.value = {
    year: record.year,
    month: record.month,
    title: record.title,
    achievements: record.achievements || '',
    challenges: record.challenges || '',
    lessonsLearned: record.lessonsLearned || '',
    nextMonthGoals: record.nextMonthGoals || '',
    rating: record.rating,
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  try {
    if (editingId.value) {
      await updateMonthlyReviewApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createMonthlyReviewApi(formData.value as CreateMonthlyReviewInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteMonthlyReviewApi(id);
    message.success('删除成功');
    await load();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="每月回顾成长和规划" title="月度复盘">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已复盘" :value="stats.total" suffix="个月" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均评分" :value="stats.avgRating" suffix="/5" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" suffix="条" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总页数" :value="Math.ceil(total / 10)" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="复盘标题" @press-enter="search" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button @click="openCreate">新建复盘</Button>
          </Space>
        </Form.Item>
      </Form>
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
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'period'">
            <Tag color="blue">{{ record.year }}-{{ String(record.month).padStart(2, '0') }}</Tag>
          </template>
          <template v-else-if="column.key === 'rating'">
            <span>{{ record.rating }}/5</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record as MonthlyReview)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑复盘' : '新建复盘'"
      width="600px"
      @ok="handleSubmit"
    >
      <Form layout="vertical" :model="formData">
        <Form.Item v-if="!editingId" label="年份" required>
          <InputNumber v-model:value="formData.year" :min="2020" :max="2030" class="w-full" />
        </Form.Item>
        <Form.Item v-if="!editingId" label="月份" required>
          <Select v-model:value="formData.month" :options="monthOptions" class="w-full" />
        </Form.Item>
        <Form.Item label="标题" required>
          <Input v-model:value="formData.title" placeholder="复盘标题" />
        </Form.Item>
        <Form.Item label="评分">
          <InputNumber v-model:value="formData.rating" :min="1" :max="5" class="w-full" />
        </Form.Item>
        <Form.Item label="本月亮点">
          <Input.TextArea v-model:value="formData.achievements" placeholder="本月成就和亮点" />
        </Form.Item>
        <Form.Item label="遇到的挑战">
          <Input.TextArea v-model:value="formData.challenges" placeholder="本月遇到的困难" />
        </Form.Item>
        <Form.Item label="经验教训">
          <Input.TextArea v-model:value="formData.lessonsLearned" placeholder="从中学到了什么" />
        </Form.Item>
        <Form.Item label="下月目标">
          <Input.TextArea v-model:value="formData.nextMonthGoals" placeholder="下个月的计划" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Input,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Tag,
  Table,
} from 'ant-design-vue';

import type { Habit } from '#/api/growth';

import {
  checkInHabitApi,
  deleteHabitApi,
  getHabitPageApi,
  updateHabitStatusApi,
} from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

import HabitDetail from './components/HabitDetail.vue';
import HabitForm from './components/HabitForm.vue';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(undefined);
const selectedItem = ref<null | Habit>(null);

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  Habit,
  { habitType?: string; keyword?: string; page: number; pageSize: number; status?: 0 | 1 }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getHabitPageApi,
});

const columns = [
  { dataIndex: 'name', key: 'name', title: '习惯' },
  { dataIndex: 'habitType', key: 'habitType', title: '类型', width: 100 },
  { dataIndex: 'targetFrequency', key: 'targetFrequency', title: '频率' },
  { dataIndex: 'currentStreak', key: 'currentStreak', title: '当前连续', width: 110 },
  { dataIndex: 'longestStreak', key: 'longestStreak', title: '最长连续', width: 110 },
  { dataIndex: 'checkInCount', key: 'checkInCount', title: '打卡次数' },
  { dataIndex: 'todayCompleted', key: 'todayCompleted', title: '今日完成', width: 110 },
  { dataIndex: 'lastCheckInDate', key: 'lastCheckInDate', title: '最近打卡' },
  { dataIndex: 'status', key: 'status', title: '状态' },
  { key: 'action', title: '操作', width: 260 },
];

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '停用', value: 0 },
  { label: '启用', value: 1 },
];

const habitTypeOptions = [
  { label: '全部类型', value: undefined },
  { label: '学习', value: '学习' },
  { label: '工作', value: '工作' },
  { label: '生活', value: '生活' },
  { label: '健康', value: '健康' },
];

function openCreate() {
  editingId.value = undefined;
  formOpen.value = true;
}

function openEdit(id: string) {
  editingId.value = id;
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as Habit;
  detailOpen.value = true;
}

async function checkIn(record: Record<string, any>) {
  const habit = record as Habit;
  if (habit.todayCompleted) {
    message.info(`"${habit.name}" 今日已完成`);
    return;
  }
  await checkInHabitApi(habit.id);
  message.success(`"${habit.name}" 已打卡`);
  await load();
}

async function toggleStatus(record: Record<string, any>) {
  const habit = record as Habit;
  await updateHabitStatusApi(habit.id, habit.status === 1 ? 0 : 1);
  message.success(`"${habit.name}" 状态已更新`);
  await load();
}

async function remove(id: string) {
  await deleteHabitApi(id);
  message.success('习惯已删除');
  await load();
}

function handleFormOpenChange(value: boolean) {
  if (!value) {
    editingId.value = undefined;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="维护学习、工作、健康和生活习惯，支持每日打卡和启停管理" title="习惯打卡">
    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input
            v-model:value="query.keyword"
            allow-clear
            placeholder="习惯名称"
            @press-enter="search"
          />
        </Form.Item>
        <Form.Item label="状态">
          <Select v-model:value="query.status" :options="statusOptions" class="w-36" />
        </Form.Item>
        <Form.Item label="类型">
          <Select v-model:value="query.habitType" :options="habitTypeOptions" class="w-36" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search"> 查询 </Button>
            <Button @click="openCreate"> 新增 </Button>
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
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'status'">
            <Tag :color="text === 1 ? 'success' : 'default'">
              {{ text === 1 ? '启用' : '停用' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'habitType'">
            <Tag color="blue">{{ text }}</Tag>
          </template>
          <template v-else-if="column.key === 'todayCompleted'">
            <Tag :color="text ? 'success' : 'default'">
              {{ text ? '已完成' : '未完成' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'currentStreak'"> {{ text }} 天 </template>
          <template v-else-if="column.key === 'longestStreak'"> {{ text }} 天 </template>
          <template v-else-if="column.key === 'lastCheckInDate'">
            {{ text || '-' }}
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="showDetail(record)"> 详情 </Button>
              <Button size="small" type="link" @click="checkIn(record)"> 打卡 </Button>
              <Button size="small" type="link" @click="openEdit(record.id)"> 编辑 </Button>
              <Button size="small" type="link" @click="toggleStatus(record)">
                {{ record.status === 1 ? '停用' : '启用' }}
              </Button>
              <Popconfirm title="确认删除这个习惯？" @confirm="remove(record.id)">
                <Button danger size="small" type="link"> 删除 </Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <HabitForm
      v-model:open="formOpen"
      :id="editingId"
      @success="load"
      @update:open="handleFormOpenChange"
    />
    <Modal v-model:open="detailOpen" title="习惯详情" width="560px" :footer="null">
      <HabitDetail :item="selectedItem" />
    </Modal>
  </Page>
</template>
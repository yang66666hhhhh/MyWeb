<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Form,
  Input,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import type { Dayjs } from 'dayjs';

import type { WorkLog } from '#/api/growth';

import { deleteWorkLogApi, getWorkLogPageApi } from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

import WorkLogDetail from './components/WorkLogDetail.vue';
import WorkLogForm from './components/WorkLogForm.vue';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<null | WorkLog>(null);

const projectOptions = [
  { label: '个人管理系统', value: 'project-1' },
  { label: '工作项目', value: 'project-work' },
  { label: '在职备考', value: 'project-study' },
];

const categoryColorMap: Record<string, string> = {
  会议: 'gold',
  复盘: 'purple',
  学习: 'cyan',
  开发: 'blue',
  排障: 'red',
};

const columns: any[] = [
  { dataIndex: 'logDate', key: 'logDate', title: '日期', width: 120 },
  { dataIndex: 'title', key: 'title', title: '工作标题', minWidth: 220 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 110 },
  { dataIndex: 'projectName', key: 'projectName', title: '关联项目', width: 140 },
  { dataIndex: 'durationMinutes', key: 'durationMinutes', title: '耗时', width: 110 },
  { dataIndex: 'summary', key: 'summary', title: '今日总结', minWidth: 240 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
];

const {
  changePage,
  items,
  load,
  loading,
  query,
  resetQuery,
  search,
  total,
} = usePagedQuery<
  WorkLog,
  { endDate?: string; keyword?: string; page: number; pageSize: number; projectId?: string; startDate?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getWorkLogPageApi,
});

function onDateRangeChange(values: [string, string] | [Dayjs, Dayjs] | null) {
  const start = values?.[0];
  const end = values?.[1];
  query.startDate =
    start && typeof start !== 'string' ? start.format('YYYY-MM-DD') : undefined;
  query.endDate =
    end && typeof end !== 'string' ? end.format('YYYY-MM-DD') : undefined;
}

function openCreate() {
  editingId.value = null;
  formOpen.value = true;
}

function openEdit(record: Record<string, any>) {
  editingId.value = (record as WorkLog).id;
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as WorkLog;
  detailOpen.value = true;
}

async function remove(id: string) {
  await deleteWorkLogApi(id);
  message.success('工作日志已删除');
  await load();
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function resetFilters() {
  resetQuery();
  void load();
}

function handleFormOpenChange(value: boolean) {
  if (!value) {
    editingId.value = null;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="沉淀每日工作内容、问题、总结和明日计划，当前使用 mock/预留接口。" title="工作日志">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="日期范围">
              <DatePicker.RangePicker @change="onDateRangeChange" />
            </Form.Item>
            <Form.Item label="项目">
              <Select
                v-model:value="query.projectId"
                :options="projectOptions"
                allow-clear
                class="w-40"
              />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="标题、内容、总结"
                style="width: 220px"
                @press-enter="search"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增日志</Button>
        </div>
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
          :scroll="{ x: 1120 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.title }}</div>
                <div class="text-text-secondary line-clamp-2 text-xs">
                  {{ record.content }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'category'">
              <Tag :color="categoryColorMap[text] || 'blue'">{{ text }}</Tag>
            </template>
            <template v-else-if="column.key === 'durationMinutes'">{{ text }} 分钟</template>
            <template v-else-if="column.key === 'summary'">
              <span class="text-text-secondary">{{ text || '-' }}</span>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm title="确认删除这条工作日志？" @confirm="remove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <WorkLogForm
      v-model:open="formOpen"
      :id="editingId"
      @success="load"
      @update:open="handleFormOpenChange"
    />
    <Modal v-model:open="detailOpen" title="工作日志详情" width="760px" :footer="null">
      <WorkLogDetail :item="selectedItem" />
    </Modal>
  </Page>
</template>

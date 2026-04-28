<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
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

import type { KnowledgeArticle } from '#/api/growth';

import { deleteKnowledgeArticleApi, getKnowledgeArticlePageApi } from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

import KnowledgeDetail from './components/KnowledgeDetail.vue';
import KnowledgeForm from './components/KnowledgeForm.vue';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<KnowledgeArticle | null>(null);

const categoryOptions = [
  { label: '学习/数据结构', value: '学习/数据结构' },
  { label: '学习/数学', value: '学习/数学' },
  { label: '学习/英语', value: '学习/英语' },
  { label: '学习/政治', value: '学习/政治' },
  { label: '工作/工程实践', value: '工作/工程实践' },
  { label: '个人/方法论', value: '个人/方法论' },
];

const tagOptions = [
  { label: '学习', value: '学习' },
  { label: '工作', value: '工作' },
  { label: '错题', value: '错题' },
  { label: '接口', value: '接口' },
  { label: '复盘', value: '复盘' },
];

const columns: any[] = [
  { dataIndex: 'title', key: 'title', title: '标题', minWidth: 220 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 180 },
  { dataIndex: 'tags', key: 'tags', title: '标签', width: 220 },
  { dataIndex: 'summary', key: 'summary', title: '内容摘要', minWidth: 260 },
  { dataIndex: 'favorite', key: 'favorite', title: '收藏', width: 90 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 170 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  KnowledgeArticle,
  { category?: string; keyword?: string; page: number; pageSize: number; tag?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getKnowledgeArticlePageApi,
});

function openCreate() {
  editingId.value = null;
  formOpen.value = true;
}

function openEdit(record: Record<string, any>) {
  editingId.value = (record as KnowledgeArticle).id;
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as KnowledgeArticle;
  detailOpen.value = true;
}

async function remove(id: string) {
  await deleteKnowledgeArticleApi(id);
  message.success('笔记已删除');
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
  <Page description="管理知识笔记、标签和 Markdown 内容，当前使用 mock/预留接口。" title="知识库">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="分类">
              <Select
                v-model:value="query.category"
                :options="categoryOptions"
                allow-clear
                class="w-48"
              />
            </Form.Item>
            <Form.Item label="标签">
              <Select
                v-model:value="query.tag"
                :options="tagOptions"
                allow-clear
                class="w-36"
              />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="标题、摘要、内容"
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
          <Button type="primary" @click="openCreate">新增笔记</Button>
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
          :scroll="{ x: 1280 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.title }}</div>
                <div class="text-text-secondary line-clamp-2 text-xs">
                  {{ record.sourceUrl || '暂无来源链接' }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'category'">
              <Tag color="purple">{{ text }}</Tag>
            </template>
            <template v-else-if="column.key === 'tags'">
              <Space v-if="text?.length" wrap>
                <Tag v-for="tag in text" :key="tag">{{ tag }}</Tag>
              </Space>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'summary'">
              <span class="text-text-secondary">{{ text || '-' }}</span>
            </template>
            <template v-else-if="column.key === 'favorite'">
              <Tag :color="text ? 'gold' : 'default'">
                {{ text ? '已收藏' : '未收藏' }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'createdAt'">
              {{ text?.replace('T', ' ').slice(0, 16) || '-' }}
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm title="确认删除这篇笔记？" @confirm="remove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <KnowledgeForm
      v-model:open="formOpen"
      :id="editingId"
      @success="load"
      @update:open="handleFormOpenChange"
    />
    <Modal v-model:open="detailOpen" title="笔记详情" width="820px" :footer="null">
      <KnowledgeDetail :item="selectedItem" />
    </Modal>
  </Page>
</template>

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

import type { RiskItem } from '#/api/work/extended';
import {
  createRiskApi,
  deleteRiskApi,
  getRisksApi,
  updateRiskApi,
} from '#/api/work/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入风险标题', type: 'string' as const, trigger: 'blur' as const }],
};

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<RiskItem | null>(null);

const formState = ref({
  category: '',
  description: '',
  identifiedDate: '',
  impact: 3,
  mitigationPlan: '',
  probability: 3,
  tags: '',
  title: '',
});

const columns: any[] = [
  { dataIndex: 'title', key: 'title', title: '风险', minWidth: 160 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'impact', key: 'impact', title: '影响', width: 80 },
  { dataIndex: 'probability', key: 'probability', title: '概率', width: 80 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 90 },
  { dataIndex: 'mitigationPlan', key: 'mitigationPlan', title: '应对措施', minWidth: 160 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const categoryOptions = [
  { label: '技术', value: '技术' },
  { label: '人员', value: '人员' },
  { label: '需求', value: '需求' },
  { label: '进度', value: '进度' },
  { label: '质量', value: '质量' },
  { label: '其他', value: '其他' },
];

const statusOptions = [
  { label: '已识别', value: 0 },
  { label: '监控中', value: 1 },
  { label: '已解决', value: 2 },
  { label: '已关闭', value: 3 },
];

const statusColors: Record<number, string> = {
  0: 'orange',
  1: 'processing',
  2: 'success',
  3: 'default',
};

const statusLabels: Record<number, string> = {
  0: '已识别',
  1: '监控中',
  2: '已解决',
  3: '已关闭',
};

const levelColors: Record<number, string> = {
  5: 'red',
  4: 'red',
  3: 'orange',
  2: 'green',
  1: 'green',
};

function levelLabel(value: number) {
  if (value >= 4) return '高';
  if (value >= 3) return '中';
  return '低';
}

const { changePage, items, load, loading, query, resetQuery, search, total } =
  usePagedQuery<
    RiskItem,
    {
      category?: string;
      keyword?: string;
      page: number;
      pageSize: number;
      status?: number;
    }
  >({
    defaultQuery: { page: 1, pageSize: 10 },
    fetcher: getRisksApi,
  });

function openCreate() {
  editingId.value = null;
  formState.value = {
    category: '技术',
    description: '',
    identifiedDate: '',
    impact: 3,
    mitigationPlan: '',
    probability: 3,
    tags: '',
    title: '',
  };
  formOpen.value = true;
}

async function openEdit(record: Record<string, any>) {
  const risk = record as RiskItem;
  editingId.value = risk.id;
  formState.value = {
    category: risk.category || '',
    description: risk.description || '',
    identifiedDate: risk.identifiedDate || '',
    impact: risk.impact ?? 3,
    mitigationPlan: risk.mitigationPlan || '',
    probability: risk.probability ?? 3,
    tags: risk.tags || '',
    title: risk.title || '',
  };
  formOpen.value = true;
}

function showDetail(record: Record<string, any>) {
  selectedItem.value = record as RiskItem;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  try {
    await deleteRiskApi(id);
    message.success('风险已删除');
    await load();
  } catch {
    message.error('删除失败');
  }
}

async function handleSubmit() {
  try { await formRef.value?.validate(); } catch { return; }
  try {
    if (editingId.value) {
      await updateRiskApi(editingId.value, formState.value);
      message.success('风险已更新');
    } else {
      await createRiskApi(formState.value as any);
      message.success('风险已创建');
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
  <Page description="识别和管理项目风险" title="风险管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="风险总数" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="高风险" :value="items.filter((i: RiskItem) => i.impact >= 4).length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="监控中" :value="items.filter((i: RiskItem) => i.status === 1).length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已解决" :value="items.filter((i: RiskItem) => i.status === 2).length" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <Form :model="query" layout="inline">
          <Form.Item label="分类">
            <Select v-model:value="query.category" :options="categoryOptions" allow-clear class="w-28" />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-28" />
          </Form.Item>
          <Form.Item label="关键词">
            <Input v-model:value="query.keyword" allow-clear placeholder="风险标题/描述" style="width: 180px" @press-enter="search" />
          </Form.Item>
          <Form.Item>
            <Space>
              <Button type="primary" @click="search">查询</Button>
              <Button @click="resetFilters">重置</Button>
            </Space>
          </Form.Item>
        </Form>
        <Button type="primary" @click="openCreate">识别风险</Button>
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
        :scroll="{ x: 1000 }"
        row-key="id"
        @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'title'">
            <div class="font-medium">{{ record.title }}</div>
            <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'impact'">
            <Tag :color="levelColors[record.impact]">{{ levelLabel(record.impact) }}</Tag>
          </template>
          <template v-else-if="column.key === 'probability'">
            <Tag :color="levelColors[record.probability]">{{ levelLabel(record.probability) }}</Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ statusLabels[record.status] }}</Tag>
          </template>
          <template v-else-if="column.key === 'mitigationPlan'">
            <span class="line-clamp-2">{{ record.mitigationPlan || '-' }}</span>
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
      :title="editingId ? '编辑风险' : '识别风险'"
      width="600px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <Form.Item label="风险标题" required>
          <Input v-model:value="formState.title" placeholder="风险标题" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="风险描述" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="分类">
            <Select v-model:value="formState.category" :options="categoryOptions" />
          </Form.Item>
          <Form.Item label="识别日期">
            <Input v-model:value="formState.identifiedDate" type="date" />
          </Form.Item>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="影响程度 (1-5)">
            <InputNumber v-model:value="formState.impact" :min="1" :max="5" class="w-full" />
          </Form.Item>
          <Form.Item label="发生概率 (1-5)">
            <InputNumber v-model:value="formState.probability" :min="1" :max="5" class="w-full" />
          </Form.Item>
        </div>
        <Form.Item label="应对措施">
          <Input.TextArea v-model:value="formState.mitigationPlan" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="应对措施" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="风险详情" width="500px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="风险标题">{{ selectedItem.title }}</Descriptions.Item>
          <Descriptions.Item label="描述">{{ selectedItem.description || '-' }}</Descriptions.Item>
          <Descriptions.Item label="分类">{{ selectedItem.category }}</Descriptions.Item>
          <Descriptions.Item label="影响程度">
            <Tag :color="levelColors[selectedItem.impact]">{{ levelLabel(selectedItem.impact) }} ({{ selectedItem.impact }})</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="发生概率">
            <Tag :color="levelColors[selectedItem.probability]">{{ levelLabel(selectedItem.probability) }} ({{ selectedItem.probability }})</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="statusColors[selectedItem.status]">{{ statusLabels[selectedItem.status] }}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="应对措施">{{ selectedItem.mitigationPlan || '-' }}</Descriptions.Item>
          <Descriptions.Item label="识别日期">{{ selectedItem.identifiedDate || '-' }}</Descriptions.Item>
          <Descriptions.Item label="解决日期">{{ selectedItem.resolvedDate || '-' }}</Descriptions.Item>
          <Descriptions.Item label="标签">{{ selectedItem.tags || '-' }}</Descriptions.Item>
          <Descriptions.Item label="创建时间">{{ selectedItem.createdAt }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>

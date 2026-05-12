<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
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

import { softwareAssetApi, type SoftwareAsset, type SoftwareAssetQuery, type CreateSoftwareAssetInput } from '#/api/work/softwareAsset';

const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const items = ref<SoftwareAsset[]>([]);
const total = ref(0);

const query = reactive({
  page: 1,
  pageSize: 20,
  keyword: '',
  type: undefined as number | undefined,
  licenseType: undefined as number | undefined,
  status: undefined as number | undefined,
});

const formState = reactive({
  name: '',
  version: '',
  type: 0,
  licenseType: 0,
  status: 0,
  vendor: '',
  purchaseDate: null as string | null,
  expireDate: null as string | null,
  cost: undefined as number | undefined,
  description: '',
  assignedTo: '',
});

const typeMap: Record<number, { color: string; label: string }> = {
  0: { color: 'blue', label: 'IDE' },
  1: { color: 'purple', label: '设计工具' },
  2: { color: 'cyan', label: 'API工具' },
  3: { color: 'orange', label: '项目管理' },
  4: { color: 'green', label: '通讯工具' },
  5: { color: 'magenta', label: 'DevOps' },
  6: { color: 'red', label: '数据库' },
  99: { color: 'default', label: '其他' },
};

const licenseTypeMap: Record<number, { color: string; label: string }> = {
  0: { color: 'green', label: '免费' },
  1: { color: 'orange', label: '付费' },
  2: { color: 'blue', label: '开源' },
  3: { color: 'gold', label: '试用' },
};

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'success', label: '可用' },
  1: { color: 'processing', label: '使用中' },
  2: { color: 'error', label: '已过期' },
  3: { color: 'default', label: '已退役' },
};

const columns = [
  { title: '软件名称', dataIndex: 'name', key: 'name', minWidth: 150 },
  { title: '版本', dataIndex: 'version', key: 'version', width: 100 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 100 },
  { title: '许可', dataIndex: 'licenseType', key: 'licenseType', width: 80 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 80 },
  { title: '使用人', dataIndex: 'assignedTo', key: 'assignedTo', width: 100 },
  { title: '过期日期', dataIndex: 'expireDate', key: 'expireDate', width: 120 },
  { title: '成本', dataIndex: 'cost', key: 'cost', width: 100 },
  { key: 'action', title: '操作', width: 150, fixed: 'right' },
];

const typeOptions = Object.entries(typeMap).map(([v, m]) => ({ label: m.label, value: Number(v) }));
const licenseTypeOptions = Object.entries(licenseTypeMap).map(([v, m]) => ({ label: m.label, value: Number(v) }));
const statusOptions = Object.entries(statusMap).map(([v, m]) => ({ label: m.label, value: Number(v) }));

const filterOptions = [
  { label: '全部类型', value: undefined },
  ...typeOptions,
];

const licenseFilterOptions = [
  { label: '全部许可', value: undefined },
  ...licenseTypeOptions,
];

const statusFilterOptions = [
  { label: '全部状态', value: undefined },
  ...statusOptions,
];

const stats = computed(() => {
  const totalCount = items.value.length;
  const inUseCount = items.value.filter((i) => i.status === 1).length;
  const paidCount = items.value.filter((i) => i.licenseType === 1).length;
  const expiredCount = items.value.filter((i) => i.status === 2).length;
  return { totalCount, inUseCount, paidCount, expiredCount };
});

async function fetchPage() {
  loading.value = true;
  try {
    const result = await softwareAssetApi.getPage(query);
    items.value = result.items;
    total.value = result.total;
  } catch {
    message.error('加载失败');
  } finally {
    loading.value = false;
  }
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  query.page = pagination.current ?? 1;
  query.pageSize = pagination.pageSize ?? 20;
  fetchPage();
}

function search() {
  query.page = 1;
  fetchPage();
}

function resetFilters() {
  query.keyword = '';
  query.type = undefined;
  query.licenseType = undefined;
  query.status = undefined;
  query.page = 1;
  fetchPage();
}

function openCreate() {
  editingId.value = null;
  Object.assign(formState, {
    name: '',
    version: '',
    type: 0,
    licenseType: 0,
    status: 0,
    vendor: '',
    purchaseDate: null,
    expireDate: null,
    cost: undefined,
    description: '',
    assignedTo: '',
  });
  formOpen.value = true;
}

function openEdit(record: SoftwareAsset) {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    version: record.version || '',
    type: record.type,
    licenseType: record.licenseType,
    status: record.status,
    vendor: record.vendor || '',
    purchaseDate: record.purchaseDate || null,
    expireDate: record.expireDate || null,
    cost: record.cost,
    description: record.description || '',
    assignedTo: record.assignedTo || '',
  });
  formOpen.value = true;
}

async function handleSave() {
  if (!formState.name) {
    message.error('请填写软件名称');
    return;
  }

  try {
    if (editingId.value) {
      await softwareAssetApi.update(editingId.value, formState);
      message.success('更新成功');
    } else {
      await softwareAssetApi.create(formState as CreateSoftwareAssetInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await fetchPage();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await softwareAssetApi.delete(id);
    message.success('删除成功');
    await fetchPage();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  fetchPage();
});
</script>

<template>
  <Page description="管理团队软件资产和许可证" title="软件资产">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="软件总数" :value="stats.totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="使用中" :value="stats.inUseCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="付费许可" :value="stats.paidCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已过期" :value="stats.expiredCount" /></Card>
      </Col>
    </Row>

    <Card>
      <div class="mb-4 flex items-center justify-between">
        <Space>
          <Input
            v-model:value="query.keyword"
            placeholder="搜索软件名称..."
            style="width: 200px"
            @pressEnter="search"
          />
          <Select
            v-model:value="query.type"
            :options="filterOptions"
            allow-clear
            placeholder="类型"
            style="width: 120px"
            @change="search"
          />
          <Select
            v-model:value="query.licenseType"
            :options="licenseFilterOptions"
            allow-clear
            placeholder="许可"
            style="width: 100px"
            @change="search"
          />
          <Select
            v-model:value="query.status"
            :options="statusFilterOptions"
            allow-clear
            placeholder="状态"
            style="width: 100px"
            @change="search"
          />
          <Button type="primary" @click="search">查询</Button>
          <Button @click="resetFilters">重置</Button>
        </Space>
        <Button type="primary" @click="openCreate">添加软件</Button>
      </div>

      <Table
        :columns="columns"
        :data-source="items"
        :loading="loading"
        :pagination="{ current: query.page, pageSize: query.pageSize, showSizeChanger: true, showTotal: (v: number) => `共 ${v} 条`, total }"
        :scroll="{ x: 1200 }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, text, record }">
          <template v-if="column.key === 'type'">
            <Tag :color="typeMap[text]?.color">{{ typeMap[text]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'licenseType'">
            <Tag :color="licenseTypeMap[text]?.color">{{ licenseTypeMap[text]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusMap[text]?.color">{{ statusMap[text]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'cost'">
            <span v-if="text">¥{{ text }}</span>
            <span v-else class="text-gray-400">-</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
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
      :title="editingId ? '编辑软件' : '添加软件'"
      width="600px"
      @ok="handleSave"
    >
      <Form :model="formState" layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="软件名称" required>
              <Input v-model:value="formState.name" placeholder="如: Visual Studio Code" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="版本">
              <Input v-model:value="formState.version" placeholder="如: 1.85" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="8">
            <Form.Item label="类型">
              <Select v-model:value="formState.type" :options="typeOptions" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="许可类型">
              <Select v-model:value="formState.licenseType" :options="licenseTypeOptions" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="状态">
              <Select v-model:value="formState.status" :options="statusOptions" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="厂商">
              <Input v-model:value="formState.vendor" placeholder="如: Microsoft" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="使用人">
              <Input v-model:value="formState.assignedTo" placeholder="如: Jack" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="购买日期">
              <DatePicker v-model:value="formState.purchaseDate" style="width: 100%" format="YYYY-MM-DD" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="过期日期">
              <DatePicker v-model:value="formState.expireDate" style="width: 100%" format="YYYY-MM-DD" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="成本">
              <InputNumber v-model:value="formState.cost" :min="0" style="width: 100%" placeholder="0" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :rows="2" placeholder="备注信息" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

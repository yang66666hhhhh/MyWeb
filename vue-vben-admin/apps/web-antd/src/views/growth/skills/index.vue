<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

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

import type { CreateSkillInput, Skill, UpdateSkillInput } from '#/api/growth/extended';

import {
  createSkillApi,
  deleteSkillApi,
  getSkillsApi,
  updateSkillApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const accessStore = useAccessStore();
const hasPermission = (code: string) => accessStore.accessCodes.includes(code);

const formOpen = ref(false);
const submitting = ref(false);
const editingId = ref<null | string>(null);
const formRef = ref();
const formData = ref<CreateSkillInput & UpdateSkillInput>({
  name: '',
  category: '',
  level: 0,
  targetLevel: 0,
  description: '',
  tags: '',
});

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  Skill,
  { category?: string; keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getSkillsApi,
});

const categoryOptions = [
  { label: '全部分类', value: undefined },
  { label: '前端', value: '前端' },
  { label: '后端', value: '后端' },
  { label: '数据库', value: '数据库' },
  { label: 'DevOps', value: 'DevOps' },
];

const categoryColors: Record<string, string> = {
  前端: 'blue',
  后端: 'green',
  数据库: 'orange',
  DevOps: 'purple',
};

const formRules = {
  name: [{ required: true, message: '请输入技能名称', type: 'string' as const, trigger: 'blur' as const }],
  category: [{ required: true, message: '请选择分类', type: 'string' as const, trigger: 'change' as const }],
};

const columns = [
  { dataIndex: 'name', key: 'name', title: '技能名称' },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'level', key: 'level', title: '当前等级', width: 100 },
  { dataIndex: 'targetLevel', key: 'targetLevel', title: '目标等级', width: 100 },
  { dataIndex: 'experiencePoints', key: 'experiencePoints', title: '经验值', width: 100 },
  { key: 'progress', title: '进度', width: 160 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 180 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as Skill[];
  return {
    total: total.value,
    avgLevel: list.length ? Math.round(list.reduce((s, i) => s + i.level, 0) / list.length) : 0,
    avgProgress: list.length
      ? Math.round(list.reduce((s, i) => s + (i.targetLevel ? (i.level / i.targetLevel) * 100 : 0), 0) / list.length)
      : 0,
  };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { name: '', category: '', level: 0, targetLevel: 0, description: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: Skill) {
  editingId.value = record.id;
  formData.value = {
    name: record.name,
    category: record.category,
    level: record.level,
    targetLevel: record.targetLevel,
    description: record.description || '',
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  try {
    await formRef.value?.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateSkillApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createSkillApi(formData.value as CreateSkillInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  } finally {
    submitting.value = false;
  }
}

async function handleDelete(id: string) {
  try {
    await deleteSkillApi(id);
    message.success('删除成功');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="管理和追踪您的技能成长" title="技能管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="技能总数" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均等级" :value="stats.avgLevel" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="stats.avgProgress" suffix="%" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" suffix="条" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="技能名称" @press-enter="search" />
        </Form.Item>
        <Form.Item label="分类">
          <Select v-model:value="query.category" :options="categoryOptions" class="w-36" allow-clear />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button v-if="hasPermission('GROWTH_SKILL')" @click="openCreate">新增技能</Button>
          </Space>
        </Form.Item>
      </Form>
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
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category]">{{ record.category }}</Tag>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="w-32">
              <div class="mb-1 text-right text-sm">
                {{ record.targetLevel ? Math.round((record.level / record.targetLevel) * 100) : 0 }}%
              </div>
              <div class="h-2 rounded-full bg-gray-200">
                <div
                  class="h-full rounded-full bg-blue-500"
                  :style="{ width: `${record.targetLevel ? Math.round((record.level / record.targetLevel) * 100) : 0}%` }"
                ></div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button v-if="hasPermission('GROWTH_SKILL')" size="small" type="link" @click="openEdit(record as Skill)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button v-if="hasPermission('GROWTH_SKILL')" danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :confirm-loading="submitting"
      :title="editingId ? '编辑技能' : '新增技能'"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="技能名称" required>
          <Input v-model:value="formData.name" placeholder="请输入技能名称" />
        </Form.Item>
        <Form.Item label="分类" required>
          <Select v-model:value="formData.category" :options="categoryOptions.filter(o => o.value)" placeholder="请选择分类" />
        </Form.Item>
        <Form.Item label="当前等级">
          <InputNumber v-model:value="formData.level" :min="0" class="w-full" />
        </Form.Item>
        <Form.Item label="目标等级">
          <InputNumber v-model:value="formData.targetLevel" :min="0" class="w-full" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formData.description" placeholder="技能描述" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

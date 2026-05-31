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
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import {
  createMaterialApi,
  deleteMaterialApi,
  getMaterialPageApi,
  type ExamMaterial,
  type SaveExamMaterialInput,
  updateMaterialApi,
} from '#/api/student';

interface MaterialFormState {
  content: string;
  subject: string;
  tags: string;
  title: string;
  type: number;
}

const loading = ref(false);
const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const currentMaterial = ref<ExamMaterial | null>(null);
const keyword = ref('');
const subject = ref<string | undefined>();
const type = ref<number | undefined>();
const materials = ref<ExamMaterial[]>([]);

const accessStore = useAccessStore();
const canCreateMaterial = computed(() => accessStore.accessCodes.includes('STUDENT_MATERIALS'));
const canEditMaterial = computed(() => accessStore.accessCodes.includes('STUDENT_MATERIALS'));
const canDeleteMaterial = computed(() => accessStore.accessCodes.includes('STUDENT_MATERIALS'));

const defaultSubjects = ['数据结构', '操作系统', '计算机网络', '计算机组成原理', '数学', '英语', '政治'];

const typeOptions = [
  { label: '笔记', value: 0 },
  { label: '总结', value: 1 },
  { label: '公式', value: 2 },
  { label: '模板', value: 3 },
];

const typeLabels: Record<number, string> = {
  0: '笔记',
  1: '总结',
  2: '公式',
  3: '模板',
};

const typeColors: Record<number, string> = {
  0: 'blue',
  1: 'cyan',
  2: 'purple',
  3: 'green',
};

const subjectOptions = computed(() => {
  const values = new Set(defaultSubjects);
  for (const item of materials.value) {
    if (item.subject?.trim()) {
      values.add(item.subject.trim());
    }
  }
  return [...values].map((item) => ({ label: item, value: item }));
});

const columns: TableColumnsType<ExamMaterial> = [
  { title: '资料标题', dataIndex: 'title', key: 'title', minWidth: 240 },
  { title: '科目', dataIndex: 'subject', key: 'subject', width: 120 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 100 },
  { title: '标签', dataIndex: 'tags', key: 'tags', minWidth: 160 },
  { title: '更新时间', dataIndex: 'updatedAt', key: 'updatedAt', width: 170 },
  { key: 'action', title: '操作', width: 190, fixed: 'right' },
];

const formState = ref<MaterialFormState>({
  content: '',
  subject: '',
  tags: '',
  title: '',
  type: 0,
});

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入资料标题', type: 'string' as const }],
  subject: [{ required: true, message: '请选择科目', type: 'string' as const }],};

const totalCount = computed(() => materials.value.length);
const noteCount = computed(() => materials.value.filter((item) => item.type === 0).length);
const formulaCount = computed(() => materials.value.filter((item) => item.type === 2).length);
const subjectCount = computed(() => new Set(materials.value.map((item) => item.subject).filter(Boolean)).size);

async function fetchMaterials() {
  loading.value = true;
  try {
    const allMaterials: ExamMaterial[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await getMaterialPageApi({
        keyword: keyword.value || undefined,
        page,
        pageSize,
        subject: subject.value,
        type: type.value,
      });

      allMaterials.push(...result.items);
      if (allMaterials.length >= result.total || result.items.length < pageSize) {
        break;
      }
      page += 1;
    }

    materials.value = allMaterials;
  } catch {
    message.error('加载学习资料失败');
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  keyword.value = '';
  subject.value = undefined;
  type.value = undefined;
  void fetchMaterials();
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    content: '',
    subject: '',
    tags: '',
    title: '',
    type: 0,
  };
  formOpen.value = true;
}

function openEdit(material: ExamMaterial) {
  editingId.value = material.id;
  formState.value = {
    content: material.content || '',
    subject: material.subject,
    tags: material.tags || '',
    title: material.title,
    type: material.type,
  };
  formOpen.value = true;
}

function openDetail(material: ExamMaterial) {
  currentMaterial.value = material;
  detailOpen.value = true;
}

function toMaterial(record: Record<string, any>) {
  return record as ExamMaterial;
}

function splitTags(tags?: null | string) {
  return (tags || '')
    .split(/[,\s，]+/)
    .map((item) => item.trim())
    .filter(Boolean);
}

async function handleSave() {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.value.title.trim() || !formState.value.subject.trim()) {
    message.warning('请填写资料标题和科目');
    return;
  }

  const payload: SaveExamMaterialInput = {
    content: formState.value.content || undefined,
    subject: formState.value.subject,
    tags: formState.value.tags || undefined,
    title: formState.value.title,
    type: formState.value.type,
  };

  try {
    if (editingId.value) {
      await updateMaterialApi(editingId.value, payload);
      message.success('学习资料已更新');
    } else {
      await createMaterialApi(payload);
      message.success('学习资料已创建');
    }
    formOpen.value = false;
    await fetchMaterials();
  } catch {
    message.error('保存学习资料失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteMaterialApi(id);
    message.success('学习资料已删除');
    await fetchMaterials();
  } catch {
    message.error('删除学习资料失败');
  }
}

onMounted(() => {
  void fetchMaterials();
});
</script>

<template>
  <Page description="沉淀笔记、公式、模板和阶段总结" title="学习资料">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="资料总数" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="笔记" :value="noteCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="公式卡片" :value="formulaCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="覆盖科目" :value="subjectCount" /></Card>
      </Col>
    </Row>

    <Card title="资料库">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="搜索标题/内容"
            style="width: 180px"
            @press-enter="fetchMaterials"
          />
          <Select
            v-model:value="subject"
            :options="subjectOptions"
            allow-clear
            placeholder="科目"
            style="width: 140px"
          />
          <Select
            v-model:value="type"
            :options="typeOptions"
            allow-clear
            placeholder="类型"
            style="width: 120px"
          />
          <Button type="primary" @click="fetchMaterials">查询</Button>
          <Button @click="resetFilters">重置</Button>
          <Button v-if="canCreateMaterial" type="primary" @click="openCreate">新增资料</Button>
        </Space>
      </template>

      <Table
        :columns="columns"
        :data-source="materials"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (value: number) => `共 ${value} 条` }"
        :scroll="{ x: 1050 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'title'">
            <Button type="link" class="!h-auto !p-0 text-left" @click="openDetail(toMaterial(record))">
              {{ record.title }}
            </Button>
            <div v-if="record.content" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.content }}
            </div>
          </template>
          <template v-else-if="column.key === 'subject'">
            <Tag color="blue">{{ text }}</Tag>
          </template>
          <template v-else-if="column.key === 'type'">
            <Tag :color="typeColors[Number(text)] || 'default'">
              {{ typeLabels[Number(text)] || '笔记' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'tags'">
            <Space v-if="splitTags(String(text || '')).length > 0" :size="4" wrap>
              <Tag v-for="tag in splitTags(String(text || ''))" :key="tag">{{ tag }}</Tag>
            </Space>
            <span v-else class="text-text-secondary">-</span>
          </template>
          <template v-else-if="column.key === 'updatedAt'">
            <span>{{ text || record.createdAt }}</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openDetail(toMaterial(record))">查看</Button>
              <Button v-if="canEditMaterial" size="small" type="link" @click="openEdit(toMaterial(record))">编辑</Button>
              <Popconfirm v-if="canDeleteMaterial" title="确认删除该资料？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑学习资料' : '新增学习资料'"
      width="760px"
      @ok="handleSave"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <Form.Item label="资料标题" required>
          <Input v-model:value="formState.title" placeholder="例如：线性代数特征值总结" />
        </Form.Item>
        <Row :gutter="16">
          <Col :span="8">
            <Form.Item label="科目" required>
              <Select
                v-model:value="formState.subject"
                :options="subjectOptions"
                allow-clear
                placeholder="选择科目"
                show-search
              />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="类型">
              <Select v-model:value="formState.type" :options="typeOptions" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="标签">
              <Input v-model:value="formState.tags" placeholder="例如：公式 真题 高频" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="内容">
          <Input.TextArea
            v-model:value="formState.content"
            :rows="8"
            placeholder="记录核心内容、例题、链接或复习提醒"
          />
        </Form.Item>
      </Form>
    </Modal>

    <Modal v-model:open="detailOpen" title="资料详情" width="760px" :footer="null">
      <div v-if="currentMaterial" class="space-y-4">
        <div>
          <div class="mb-2 text-base font-medium">{{ currentMaterial.title }}</div>
          <Space wrap>
            <Tag color="blue">{{ currentMaterial.subject }}</Tag>
            <Tag :color="typeColors[currentMaterial.type] || 'default'">
              {{ typeLabels[currentMaterial.type] || '笔记' }}
            </Tag>
            <Tag v-for="tag in splitTags(currentMaterial.tags)" :key="tag">{{ tag }}</Tag>
          </Space>
        </div>
        <div class="whitespace-pre-wrap rounded border border-gray-200 p-3 text-sm">
          {{ currentMaterial.content || '暂无内容' }}
        </div>
      </div>
    </Modal>
  </Page>
</template>

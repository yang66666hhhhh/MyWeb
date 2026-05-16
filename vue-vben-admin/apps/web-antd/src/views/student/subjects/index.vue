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
  Modal,
  Popconfirm,
  Row,
  Space,
  Statistic,
  Switch,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import {
  createStudentSubjectApi,
  deleteStudentSubjectApi,
  getStudentSubjectPageApi,
  type SaveStudentSubjectInput,
  type StudentSubject,
  updateStudentSubjectApi,
} from '#/api/student';

interface SubjectFormState {
  color: string;
  description: string;
  isActive: boolean;
  name: string;
  sort: number;
  targetHours: number;
}

const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const keyword = ref('');
const subjects = ref<StudentSubject[]>([]);

const colorOptions = [
  { label: '蓝色', value: 'blue' },
  { label: '青色', value: 'cyan' },
  { label: '绿色', value: 'green' },
  { label: '紫色', value: 'purple' },
  { label: '橙色', value: 'orange' },
  { label: '红色', value: 'red' },
];

const columns: TableColumnsType<StudentSubject> = [
  { title: '科目', dataIndex: 'name', key: 'name', minWidth: 180 },
  { title: '目标时长', dataIndex: 'targetHours', key: 'targetHours', width: 120 },
  { title: '排序', dataIndex: 'sort', key: 'sort', width: 90 },
  { title: '状态', dataIndex: 'isActive', key: 'isActive', width: 90 },
  { key: 'action', title: '操作', width: 160, fixed: 'right' },
];

const formState = ref<SubjectFormState>({
  color: 'blue',
  description: '',
  isActive: true,
  name: '',
  sort: 0,
  targetHours: 120,
});

const totalCount = computed(() => subjects.value.length);
const activeCount = computed(() => subjects.value.filter((item) => item.isActive).length);
const targetHoursTotal = computed(() => subjects.value.reduce((sum, item) => sum + item.targetHours, 0));
const averageTarget = computed(() =>
  subjects.value.length === 0 ? 0 : Math.round(targetHoursTotal.value / subjects.value.length),
);

async function fetchSubjects() {
  loading.value = true;
  try {
    const result = await getStudentSubjectPageApi({
      keyword: keyword.value || undefined,
      page: 1,
      pageSize: 100,
    });
    subjects.value = result.items;
  } catch {
    message.error('加载科目目标失败');
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    color: 'blue',
    description: '',
    isActive: true,
    name: '',
    sort: subjects.value.length,
    targetHours: 120,
  };
  formOpen.value = true;
}

function openEdit(subject: StudentSubject) {
  editingId.value = subject.id;
  formState.value = {
    color: subject.color,
    description: subject.description || '',
    isActive: subject.isActive,
    name: subject.name,
    sort: subject.sort,
    targetHours: subject.targetHours,
  };
  formOpen.value = true;
}

function toSubject(record: Record<string, any>) {
  return record as StudentSubject;
}

async function handleSave() {
  if (!formState.value.name.trim()) {
    message.warning('请填写科目名称');
    return;
  }

  const payload: SaveStudentSubjectInput = {
    color: formState.value.color,
    description: formState.value.description || undefined,
    isActive: formState.value.isActive,
    name: formState.value.name,
    sort: formState.value.sort,
    targetHours: formState.value.targetHours,
  };

  try {
    if (editingId.value) {
      await updateStudentSubjectApi(editingId.value, payload);
      message.success('科目目标已更新');
    } else {
      await createStudentSubjectApi(payload);
      message.success('科目目标已创建');
    }
    formOpen.value = false;
    await fetchSubjects();
  } catch {
    message.error('保存科目目标失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteStudentSubjectApi(id);
    message.success('科目目标已删除');
    await fetchSubjects();
  } catch {
    message.error('删除科目目标失败');
  }
}

onMounted(() => {
  void fetchSubjects();
});
</script>

<template>
  <Page description="配置科目目标和阶段学习时长" title="科目目标">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="科目数量" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="启用科目" :value="activeCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="目标总时长" :value="targetHoursTotal" suffix="h" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均目标" :value="averageTarget" suffix="h" /></Card>
      </Col>
    </Row>

    <Card title="科目列表">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="搜索科目"
            style="width: 180px"
            @press-enter="fetchSubjects"
          />
          <Button type="primary" @click="fetchSubjects">查询</Button>
          <Button type="primary" @click="openCreate">新增科目</Button>
        </Space>
      </template>

      <Table
        :columns="columns"
        :data-source="subjects"
        :loading="loading"
        :pagination="{ pageSize: 10, showTotal: (value: number) => `共 ${value} 条` }"
        :scroll="{ x: 760 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'name'">
            <Tag :color="record.color || 'blue'">{{ record.name }}</Tag>
            <div v-if="record.description" class="text-text-secondary mt-1 line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'targetHours'">
            <span>{{ text }}h</span>
          </template>
          <template v-else-if="column.key === 'isActive'">
            <Tag :color="text ? 'green' : 'default'">{{ text ? '启用' : '停用' }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(toSubject(record))">编辑</Button>
              <Popconfirm title="确认删除该科目？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑科目目标' : '新增科目目标'"
      width="620px"
      @ok="handleSave"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="科目名称" required>
          <Input v-model:value="formState.name" placeholder="例如：数学" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :rows="3" placeholder="记录阶段目标或复习范围" />
        </Form.Item>
        <Row :gutter="16">
          <Col :span="8">
            <Form.Item label="目标时长">
              <InputNumber v-model:value="formState.targetHours" :min="0" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="排序">
              <InputNumber v-model:value="formState.sort" :min="0" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="启用">
              <Switch v-model:checked="formState.isActive" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="颜色">
          <Space wrap>
            <Button
              v-for="item in colorOptions"
              :key="item.value"
              :type="formState.color === item.value ? 'primary' : 'default'"
              size="small"
              @click="formState.color = item.value"
            >
              {{ item.label }}
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

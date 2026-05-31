<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  InputNumber,
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

import type { CreateTeacherCourseInput, TeacherCourse } from '#/api/persona';

import {
  createTeacherCourseApi,
  deleteTeacherCourseApi,
  getTeacherCoursesApi,
  updateTeacherCourseApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<TeacherCourse[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateTeacherCourseInput>({
  name: '',
  description: '',
  code: '',
  semester: 1,
  year: new Date().getFullYear(),
  tags: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入课程名称', type: 'string' as const }],
  code: [{ required: true, message: '请输入课程代码', type: 'string' as const }],};

const semesterOptions = [
  { label: '第一学期', value: 1 },
  { label: '第二学期', value: 2 },
];

const statusMap: Record<number, string> = {
  0: '待开始',
  1: '进行中',
  2: '已完成',
};

const columns = [
  { title: '课程名称', dataIndex: 'name', key: 'name' },
  { title: '课程代码', dataIndex: 'code', key: 'code', width: 100 },
  { title: '学年', key: 'year', width: 120 },
  { title: '学期', key: 'semester', width: 100 },
  { title: '学生数', dataIndex: 'studentCount', key: 'studentCount', width: 80 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 80 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'blue',
  2: 'green',
};

const inProgressCount = computed(() => dataList.value.filter((c) => c.status === 1).length);
const totalStudents = computed(() => dataList.value.reduce((sum, c) => sum + (c.studentCount || 0), 0));
const completedCount = computed(() => dataList.value.filter((c) => c.status === 2).length);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getTeacherCoursesApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch {
    message.error('加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    name: '',
    description: '',
    code: '',
    semester: 1,
    year: new Date().getFullYear(),
    tags: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: TeacherCourse) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    description: record.description || '',
    code: record.code,
    semester: record.semester,
    year: record.year,
    tags: record.tags || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteTeacherCourseApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateTeacherCourseApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createTeacherCourseApi(formState);
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
  <Page description="管理课程内容、教学计划和学生信息" title="课程管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="课程总数" :value="total" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="进行中" :value="inProgressCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="学生总数" :value="totalStudents" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="已完成课程" :value="completedCount" />
        </Card>
      </Col>
    </Row>

    <Card title="课程列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">新建课程</Button>
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
          <template v-if="column.key === 'year'">
            {{ record.year }}-{{ record.year + 1 }}
          </template>
          <template v-else-if="column.key === 'semester'">
            <Tag color="blue">{{ record.semester === 1 ? '第一学期' : '第二学期' }}</Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ statusMap[record.status] || '未知' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as TeacherCourse)">编辑</Button>
              <Popconfirm title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑课程' : '新建课程'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="课程名称" required>
          <Input v-model:value="formState.name" placeholder="课程名称" />
        </FormItem>
        <FormItem label="课程代码" required>
          <Input v-model:value="formState.code" placeholder="如 CS101" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="课程描述" :rows="2" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="学年">
              <InputNumber v-model:value="formState.year" :min="2020" :max="2030" style="width: 100%" />
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="学期">
              <Select v-model:value="formState.semester" placeholder="选择学期">
                <SelectOption v-for="opt in semesterOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
        </Row>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

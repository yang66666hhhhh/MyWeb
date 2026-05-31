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
  message,
  Modal,
  Popconfirm,
  Row,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateTeacherStudentInput, TeacherStudent } from '#/api/persona';

import {
  createTeacherStudentApi,
  deleteTeacherStudentApi,
  getTeacherStudentsApi,
  updateTeacherStudentApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<TeacherStudent[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateTeacherStudentInput>({
  name: '',
  studentId: '',
  email: '',
  phone: '',
  course: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入学生姓名', type: 'string' as const }],};

const columns = [
  { title: '学生姓名', dataIndex: 'name', key: 'name' },
  { title: '学号', dataIndex: 'studentId', key: 'studentId', width: 120 },
  { title: '邮箱', dataIndex: 'email', key: 'email', width: 180 },
  { title: '课程', dataIndex: 'course', key: 'course', width: 150 },
  { title: '成绩', dataIndex: 'grade', key: 'grade', width: 80 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const gradeColors = (grade: number) => {
  if (grade >= 90) return 'green';
  if (grade >= 80) return 'blue';
  if (grade >= 60) return 'orange';
  return 'red';
};

const averageGrade = computed(() => {
  if (dataList.value.length === 0) return 0;
  const sum = dataList.value.reduce((acc, s) => acc + (s.grade || 0), 0);
  return Math.round(sum / dataList.value.length);
});

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getTeacherStudentsApi({ page: currentPage.value, pageSize: pageSize.value });
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
    studentId: '',
    email: '',
    phone: '',
    course: '',
    tags: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: TeacherStudent) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    studentId: record.studentId || '',
    email: record.email || '',
    phone: record.phone || '',
    course: record.course || '',
    tags: record.tags || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteTeacherStudentApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.name) {
    message.warning('请填写学生姓名');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateTeacherStudentApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createTeacherStudentApi(formState);
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
  <Page description="管理学生信息、学习进度和成绩" title="学生管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="学生总数" :value="total" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="平均成绩" :value="averageGrade" suffix="分" />
        </Card>
      </Col>
    </Row>

    <Card title="学生列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">添加学生</Button>
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
          <template v-if="column.key === 'grade'">
            <Tag :color="gradeColors(record.grade)">
              {{ record.grade }}分
            </Tag>
          </template>
          <template v-else-if="column.key === 'tags'">
            <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag">{{ tag }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as TeacherStudent)">编辑</Button>
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
      :title="editingId ? '编辑学生' : '添加学生'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="学生姓名" required>
          <Input v-model:value="formState.name" placeholder="学生姓名" />
        </FormItem>
        <FormItem label="学号">
          <Input v-model:value="formState.studentId" placeholder="学号" />
        </FormItem>
        <FormItem label="邮箱">
          <Input v-model:value="formState.email" placeholder="邮箱地址" />
        </FormItem>
        <FormItem label="电话">
          <Input v-model:value="formState.phone" placeholder="联系电话" />
        </FormItem>
        <FormItem label="课程">
          <Input v-model:value="formState.course" placeholder="所属课程" />
        </FormItem>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  InputNumber,
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
import dayjs from 'dayjs';

import {
  createStudyRecordApi,
  deleteStudyRecordApi,
  getStudyRecordPageApi,
  getStudentSubjectPageApi,
  type SaveStudentStudyRecordInput,
  type StudentStudyRecord,
  updateStudyRecordApi,
} from '#/api/student';

import type { Dayjs } from 'dayjs';

interface RecordFormState {
  durationMinutes: number;
  recordDate: string;
  remark: string;
  subject: string;
  summary: string;
  taskTitle: string;
}

const loading = ref(false);
const accessStore = useAccessStore();
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const keyword = ref('');
const subject = ref<string | undefined>();
const records = ref<StudentStudyRecord[]>([]);
const subjectOptions = ref<Array<{ label: string; value: string }>>([]);

const canCreateRecord = computed(() => accessStore.accessCodes.includes('STUDENT_RECORDS'));
const canEditRecord = computed(() => accessStore.accessCodes.includes('STUDENT_RECORDS'));
const canDeleteRecord = computed(() => accessStore.accessCodes.includes('STUDENT_RECORDS'));

const columns: TableColumnsType<StudentStudyRecord> = [
  { title: '日期', dataIndex: 'recordDate', key: 'recordDate', width: 120 },
  { title: '科目', dataIndex: 'subject', key: 'subject', width: 120 },
  { title: '摘要', dataIndex: 'summary', key: 'summary', minWidth: 240 },
  { title: '时长', dataIndex: 'durationMinutes', key: 'durationMinutes', width: 100 },
  { title: '关联任务', dataIndex: 'taskTitle', key: 'taskTitle', minWidth: 160 },
  { key: 'action', title: '操作', width: 160, fixed: 'right' },
];

const formState = ref<RecordFormState>({
  durationMinutes: 60,
  recordDate: dayjs().format('YYYY-MM-DD'),
  remark: '',
  subject: '',
  summary: '',
  taskTitle: '',
});

const formRef = ref();
const formRules = {
  subject: [{ required: true, message: '请选择科目', type: 'string' as const }],
  summary: [{ required: true, message: '请输入学习摘要', type: 'string' as const }],};

const totalCount = computed(() => records.value.length);
const totalMinutes = computed(() => records.value.reduce((sum, item) => sum + item.durationMinutes, 0));
const totalHours = computed(() => Math.round((totalMinutes.value / 60) * 10) / 10);
const subjectCount = computed(() => new Set(records.value.map((item) => item.subject).filter(Boolean)).size);
const averageMinutes = computed(() => records.value.length === 0 ? 0 : Math.round(totalMinutes.value / records.value.length));
const recordDateValue = computed(() => formState.value.recordDate ? dayjs(formState.value.recordDate) : undefined);
const canViewSubjects = computed(() => accessStore.accessCodes.includes('STUDENT_SUBJECTS'));

async function fetchSubjects() {
  if (!canViewSubjects.value) {
    subjectOptions.value = [];
    return;
  }

  const result = await getStudentSubjectPageApi({ isActive: true, page: 1, pageSize: 100 });
  subjectOptions.value = result.items.map((item) => ({ label: item.name, value: item.name }));
}

async function fetchRecords() {
  loading.value = true;
  try {
    const result = await getStudyRecordPageApi({
      keyword: keyword.value || undefined,
      page: 1,
      pageSize: 100,
      subject: subject.value,
    });
    records.value = result.items;
  } catch {
    message.error('加载学习记录失败');
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    durationMinutes: 60,
    recordDate: dayjs().format('YYYY-MM-DD'),
    remark: '',
    subject: subjectOptions.value[0]?.value || '',
    summary: '',
    taskTitle: '',
  };
  formOpen.value = true;
}

function openEdit(record: StudentStudyRecord) {
  editingId.value = record.id;
  formState.value = {
    durationMinutes: record.durationMinutes,
    recordDate: record.recordDate,
    remark: record.remark || '',
    subject: record.subject,
    summary: record.summary,
    taskTitle: record.taskTitle || '',
  };
  formOpen.value = true;
}

function handleDateChange(_: Dayjs | string, dateString: string) {
  formState.value.recordDate = dateString || dayjs().format('YYYY-MM-DD');
}

function toRecord(record: Record<string, any>) {
  return record as StudentStudyRecord;
}

async function handleSave() {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.value.subject.trim() || !formState.value.summary.trim()) {
    message.warning('请填写科目和学习摘要');
    return;
  }

  const payload: SaveStudentStudyRecordInput = {
    durationMinutes: formState.value.durationMinutes,
    recordDate: formState.value.recordDate,
    remark: formState.value.remark || undefined,
    subject: formState.value.subject,
    summary: formState.value.summary,
    taskTitle: formState.value.taskTitle || undefined,
  };

  try {
    if (editingId.value) {
      await updateStudyRecordApi(editingId.value, payload);
      message.success('学习记录已更新');
    } else {
      await createStudyRecordApi(payload);
      message.success('学习记录已创建');
    }
    formOpen.value = false;
    await fetchRecords();
  } catch {
    message.error('保存学习记录失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteStudyRecordApi(id);
    message.success('学习记录已删除');
    await fetchRecords();
  } catch {
    message.error('删除学习记录失败');
  }
}

onMounted(async () => {
  await fetchSubjects();
  await fetchRecords();
});
</script>

<template>
  <Page description="记录每天学了什么、花了多久、沉淀了什么" title="学习记录">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="记录数" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="累计时长" :value="totalHours" suffix="h" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="覆盖科目" :value="subjectCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均单次" :value="averageMinutes" suffix="分钟" /></Card>
      </Col>
    </Row>

    <Card title="记录列表">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="搜索摘要/备注"
            style="width: 180px"
            @press-enter="fetchRecords"
          />
          <Select
            v-if="canViewSubjects"
            v-model:value="subject"
            :options="subjectOptions"
            allow-clear
            placeholder="科目"
            style="width: 140px"
          />
          <Button type="primary" @click="fetchRecords">查询</Button>
          <Button v-if="canCreateRecord" type="primary" @click="openCreate">新增记录</Button>
        </Space>
      </template>

      <Table
        :columns="columns"
        :data-source="records"
        :loading="loading"
        :locale="{ emptyText: '暂无数据' }"
        :pagination="{ pageSize: 10, showTotal: (value: number) => `共 ${value} 条` }"
        :scroll="{ x: 960 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'subject'">
            <Tag color="blue">{{ text }}</Tag>
          </template>
          <template v-else-if="column.key === 'summary'">
            <div class="font-medium">{{ record.summary }}</div>
            <div v-if="record.remark" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.remark }}
            </div>
          </template>
          <template v-else-if="column.key === 'durationMinutes'">
            <span>{{ text }} 分钟</span>
          </template>
          <template v-else-if="column.key === 'taskTitle'">
            <span>{{ text || '-' }}</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button v-if="canEditRecord" size="small" type="link" @click="openEdit(toRecord(record))">编辑</Button>
              <Popconfirm v-if="canDeleteRecord" title="确认删除该学习记录？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑学习记录' : '新增学习记录'"
      width="680px"
      @ok="handleSave"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <Row :gutter="16">
          <Col :span="8">
            <Form.Item label="日期">
              <DatePicker
                :value="recordDateValue"
                format="YYYY-MM-DD"
                style="width: 100%"
                @change="handleDateChange"
              />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="科目" required>
              <Select
                v-if="canViewSubjects"
                v-model:value="formState.subject"
                :options="subjectOptions"
                allow-clear
                placeholder="选择科目"
                show-search
              />
              <Input
                v-else
                v-model:value="formState.subject"
                placeholder="输入科目"
              />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="时长">
              <InputNumber v-model:value="formState.durationMinutes" :min="1" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="学习摘要" required>
          <Input v-model:value="formState.summary" placeholder="例如：完成线代特征值专题" />
        </Form.Item>
        <Form.Item label="关联任务">
          <Input v-model:value="formState.taskTitle" placeholder="可填写相关学习计划标题" />
        </Form.Item>
        <Form.Item label="备注">
          <Input.TextArea v-model:value="formState.remark" :rows="3" placeholder="记录卡点、结论或下次要继续的内容" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>

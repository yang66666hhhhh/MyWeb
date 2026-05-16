<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
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
import dayjs from 'dayjs';

import {
  createMistakeApi,
  deleteMistakeApi,
  getMistakePageApi,
  type ExamMistake,
  type SaveExamMistakeInput,
  updateMistakeApi,
  updateMistakeReviewStatusApi,
} from '#/api/student';

import type { Dayjs } from 'dayjs';

interface MistakeFormState {
  answer: string;
  explanation: string;
  lastReviewDate: string;
  nextReviewDate: string;
  question: string;
  reviewCount: number;
  status: number;
  subject: string;
  tags: string;
}

const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const keyword = ref('');
const subject = ref<string | undefined>();
const mistakes = ref<ExamMistake[]>([]);

const statusLabels: Record<number, string> = {
  0: '待复习',
  1: '复习中',
  2: '已掌握',
};

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'processing',
  2: 'success',
};

const defaultSubjects = ['数据结构', '操作系统', '计算机网络', '计算机组成原理', '数学', '英语', '政治'];

const subjectOptions = computed(() => {
  const values = new Set(defaultSubjects);
  for (const item of mistakes.value) {
    if (item.subject.trim()) {
      values.add(item.subject.trim());
    }
  }
  return [...values].map((item) => ({ label: item, value: item }));
});

const columns: TableColumnsType<ExamMistake> = [
  { title: '科目', dataIndex: 'subject', key: 'subject', width: 120 },
  { title: '题目', dataIndex: 'question', key: 'question', minWidth: 260 },
  { title: '你的答案', dataIndex: 'answer', key: 'answer', minWidth: 180 },
  { title: '解析', dataIndex: 'explanation', key: 'explanation', minWidth: 220 },
  { title: '复习次数', dataIndex: 'reviewCount', key: 'reviewCount', width: 90 },
  { title: '下次复习', dataIndex: 'nextReviewDate', key: 'nextReviewDate', width: 120 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { key: 'action', title: '操作', width: 220, fixed: 'right' },
];

const formState = ref<MistakeFormState>({
  answer: '',
  explanation: '',
  lastReviewDate: '',
  nextReviewDate: '',
  question: '',
  reviewCount: 0,
  status: 0,
  subject: '',
  tags: '',
});

const totalCount = computed(() => mistakes.value.length);
const pendingCount = computed(() => mistakes.value.filter((item) => item.status === 0).length);
const reviewingCount = computed(() => mistakes.value.filter((item) => item.status === 1).length);
const masteredCount = computed(() => mistakes.value.filter((item) => item.status === 2).length);

const nextReviewDateValue = computed(() =>
  formState.value.nextReviewDate ? dayjs(formState.value.nextReviewDate) : undefined,
);
const lastReviewDateValue = computed(() =>
  formState.value.lastReviewDate ? dayjs(formState.value.lastReviewDate) : undefined,
);

async function fetchMistakes() {
  loading.value = true;
  try {
    const allMistakes: ExamMistake[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await getMistakePageApi({
        keyword: keyword.value || undefined,
        page,
        pageSize,
        subject: subject.value,
      });

      allMistakes.push(...result.items);
      if (allMistakes.length >= result.total || result.items.length < pageSize) {
        break;
      }

      page += 1;
    }

    mistakes.value = allMistakes;
  } catch {
    message.error('加载错题失败');
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  keyword.value = '';
  subject.value = undefined;
  void fetchMistakes();
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    answer: '',
    explanation: '',
    lastReviewDate: '',
    nextReviewDate: '',
    question: '',
    reviewCount: 0,
    status: 0,
    subject: '',
    tags: '',
  };
  formOpen.value = true;
}

function openEdit(mistake: ExamMistake) {
  editingId.value = mistake.id;
  formState.value = {
    answer: mistake.answer || '',
    explanation: mistake.explanation || '',
    lastReviewDate: mistake.lastReviewDate || '',
    nextReviewDate: mistake.nextReviewDate || '',
    question: mistake.question,
    reviewCount: mistake.reviewCount,
    status: mistake.status,
    subject: mistake.subject,
    tags: mistake.tags || '',
  };
  formOpen.value = true;
}

function toMistake(record: Record<string, any>) {
  return record as ExamMistake;
}

function handleLastReviewDateChange(_: Dayjs | string, dateString: string) {
  formState.value.lastReviewDate = dateString;
}

function handleNextReviewDateChange(_: Dayjs | string, dateString: string) {
  formState.value.nextReviewDate = dateString;
}

async function handleSave() {
  if (!formState.value.question.trim() || !formState.value.subject.trim()) {
    message.warning('请填写科目和题目');
    return;
  }

  const payload: SaveExamMistakeInput = {
    answer: formState.value.answer || undefined,
    explanation: formState.value.explanation || undefined,
    lastReviewDate: formState.value.lastReviewDate || undefined,
    nextReviewDate: formState.value.nextReviewDate || undefined,
    question: formState.value.question,
    reviewCount: formState.value.reviewCount,
    status: formState.value.status,
    subject: formState.value.subject,
    tags: formState.value.tags || undefined,
  };

  try {
    if (editingId.value) {
      await updateMistakeApi(editingId.value, payload);
      message.success('错题已更新');
    } else {
      await createMistakeApi(payload);
      message.success('错题已添加');
    }
    formOpen.value = false;
    await fetchMistakes();
  } catch {
    message.error('保存错题失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteMistakeApi(id);
    message.success('错题已删除');
    await fetchMistakes();
  } catch {
    message.error('删除错题失败');
  }
}

async function updateReviewStatus(mistake: ExamMistake, nextStatus: 'mastered' | 'reviewed') {
  try {
    await updateMistakeReviewStatusApi(mistake.id, nextStatus);
    message.success(nextStatus === 'mastered' ? '已标记为掌握' : '已标记为复习中');
    await fetchMistakes();
  } catch {
    message.error('更新复习状态失败');
  }
}

onMounted(() => {
  void fetchMistakes();
});
</script>

<template>
  <Page description="记录错题、分析薄弱环节、针对性复习" title="错题本">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="错题总数" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待复习" :value="pendingCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="复习中" :value="reviewingCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已掌握" :value="masteredCount" /></Card>
      </Col>
    </Row>

    <Card title="错题列表">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="搜索题目/解析"
            style="width: 180px"
            @press-enter="fetchMistakes"
          />
          <Select
            v-model:value="subject"
            :options="subjectOptions"
            allow-clear
            placeholder="科目"
            style="width: 140px"
          />
          <Button type="primary" @click="fetchMistakes">查询</Button>
          <Button @click="resetFilters">重置</Button>
          <Button type="primary" @click="openCreate">添加错题</Button>
        </Space>
      </template>

      <Table
        :columns="columns"
        :data-source="mistakes"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (value: number) => `共 ${value} 条` }"
        :scroll="{ x: 1250 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'subject'">
            <Tag color="blue">{{ text }}</Tag>
          </template>
          <template v-else-if="column.key === 'answer'">
            <span>{{ text || '-' }}</span>
          </template>
          <template v-else-if="column.key === 'explanation'">
            <span>{{ text || '-' }}</span>
          </template>
          <template v-else-if="column.key === 'nextReviewDate'">
            <span>{{ text || '-' }}</span>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[Number(text)] || 'default'">
              {{ statusLabels[Number(text)] || '待复习' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button v-if="record.status === 0" size="small" type="link" @click="updateReviewStatus(toMistake(record), 'reviewed')">
                标记复习中
              </Button>
              <Button v-if="record.status !== 2" size="small" type="link" @click="updateReviewStatus(toMistake(record), 'mastered')">
                标记掌握
              </Button>
              <Button size="small" type="link" @click="openEdit(toMistake(record))">编辑</Button>
              <Popconfirm title="确认删除这条错题？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑错题' : '添加错题'"
      width="720px"
      @ok="handleSave"
    >
      <Form :model="formState" layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="科目" required>
              <Select
                v-model:value="formState.subject"
                :options="subjectOptions"
                allow-clear
                placeholder="选择或输入科目"
                show-search
              />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="标签">
              <Input v-model:value="formState.tags" placeholder="例如：树、遍历、真题" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="题目" required>
          <Input.TextArea v-model:value="formState.question" :rows="3" placeholder="输入错题题目" />
        </Form.Item>
        <Form.Item label="你的答案">
          <Input.TextArea v-model:value="formState.answer" :rows="2" placeholder="记录当时的错误答案" />
        </Form.Item>
        <Form.Item label="解析">
          <Input.TextArea v-model:value="formState.explanation" :rows="3" placeholder="补充正确思路、易错点和复习笔记" />
        </Form.Item>
        <Row :gutter="16">
          <Col :span="6">
            <Form.Item label="状态">
              <Select
                v-model:value="formState.status"
                :options="[
                  { label: '待复习', value: 0 },
                  { label: '复习中', value: 1 },
                  { label: '已掌握', value: 2 },
                ]"
              />
            </Form.Item>
          </Col>
          <Col :span="6">
            <Form.Item label="复习次数">
              <Input v-model:value="formState.reviewCount" type="number" />
            </Form.Item>
          </Col>
          <Col :span="6">
            <Form.Item label="上次复习">
              <DatePicker
                :value="lastReviewDateValue"
                format="YYYY-MM-DD"
                style="width: 100%"
                @change="handleLastReviewDateChange"
              />
            </Form.Item>
          </Col>
          <Col :span="6">
            <Form.Item label="下次复习">
              <DatePicker
                :value="nextReviewDateValue"
                format="YYYY-MM-DD"
                style="width: 100%"
                @change="handleNextReviewDateChange"
              />
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Modal>
  </Page>
</template>

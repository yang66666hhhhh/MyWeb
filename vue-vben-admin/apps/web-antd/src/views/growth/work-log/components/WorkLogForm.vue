<script lang="ts" setup>
import { reactive, ref, watch } from 'vue';

import { DatePicker, Form, Input, InputNumber, message, Modal, Select } from 'ant-design-vue';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

import type { SaveWorkLogInput, WorkLog } from '#/api/growth';

import { createWorkLogApi, getWorkLogApi, updateWorkLogApi } from '#/api/growth';

const props = defineProps<{ id?: null | string; open: boolean }>();
const emit = defineEmits<{ success: []; 'update:open': [value: boolean] }>();

const loading = ref(false);
const Textarea = Input.TextArea;

const formState = reactive<SaveWorkLogInput>({
  category: '开发',
  content: '',
  durationMinutes: 60,
  issue: '',
  logDate: new Date().toISOString().slice(0, 10),
  nextPlan: '',
  projectId: undefined,
  projectName: '',
  summary: '',
  tags: [],
  title: '',
});

const categoryOptions = [
  { label: '开发', value: '开发' },
  { label: '会议', value: '会议' },
  { label: '排障', value: '排障' },
  { label: '学习', value: '学习' },
  { label: '复盘', value: '复盘' },
];

const projectOptions = [
  { label: '个人管理系统', value: 'project-1' },
  { label: '工作项目', value: 'project-work' },
  { label: '在职备考', value: 'project-study' },
];

function fillForm(item?: WorkLog) {
  Object.assign(formState, {
    category: item?.category ?? '开发',
    content: item?.content ?? '',
    durationMinutes: item?.durationMinutes ?? 60,
    issue: item?.issue ?? '',
    logDate: item?.logDate ?? new Date().toISOString().slice(0, 10),
    nextPlan: item?.nextPlan ?? '',
    projectId: item?.projectId,
    projectName: item?.projectName ?? '',
    summary: item?.summary ?? '',
    tags: item?.tags ?? [],
    title: item?.title ?? '',
  });
}

function onLogDateChange(value: null | string | Dayjs) {
  formState.logDate =
    value && typeof value !== 'string' ? value.format('YYYY-MM-DD') : '';
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }
  loading.value = true;
  try {
    fillForm((await getWorkLogApi(props.id)) as WorkLog);
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.title.trim() || !formState.logDate) {
    message.warning('请填写工作日期和标题');
    return;
  }

  const selectedProject = projectOptions.find((item) => item.value === formState.projectId);
  formState.projectName = selectedProject?.label ?? '';

  loading.value = true;
  try {
    if (props.id) {
      await updateWorkLogApi(props.id, { ...formState });
      message.success('工作日志已更新');
    } else {
      await createWorkLogApi({ ...formState });
      message.success('工作日志已创建');
    }
    emit('update:open', false);
    emit('success');
  } finally {
    loading.value = false;
  }
}

watch(
  () => props.open,
  (open) => {
    if (open) {
      void loadDetail();
    }
  },
);
</script>

<template>
  <Modal
    :confirm-loading="loading"
    :open="open"
    :title="id ? '编辑工作日志' : '新增工作日志'"
    width="760px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <Form :model="formState" layout="vertical">
      <div class="grid grid-cols-1 gap-4 md:grid-cols-3">
        <Form.Item label="工作日期" required>
          <DatePicker
            style="width: 100%"
            :value="formState.logDate ? dayjs(formState.logDate) : undefined"
            format="YYYY-MM-DD"
            @change="onLogDateChange"
          />
        </Form.Item>
        <Form.Item label="工作分类">
          <Select v-model:value="formState.category" :options="categoryOptions" />
        </Form.Item>
        <Form.Item label="关联项目">
          <Select
            v-model:value="formState.projectId"
            :options="projectOptions"
            allow-clear
            placeholder="可选"
          />
        </Form.Item>
      </div>
      <Form.Item label="工作标题" required>
        <Input v-model:value="formState.title" placeholder="今天主要完成了什么" />
      </Form.Item>
      <Form.Item label="工作内容">
        <Textarea
          v-model:value="formState.content"
          :auto-size="{ minRows: 4, maxRows: 8 }"
          placeholder="记录过程、关键动作和产出"
        />
      </Form.Item>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
        <Form.Item label="消耗时间">
          <InputNumber
            v-model:value="formState.durationMinutes"
            :min="0"
            addon-after="分钟"
            class="w-full"
          />
        </Form.Item>
        <Form.Item label="标签">
          <Select
            v-model:value="formState.tags"
            mode="tags"
            :max-tag-count="4"
            placeholder="例如：前端、排障、复盘"
          />
        </Form.Item>
      </div>
      <Form.Item label="今日总结">
        <Textarea
          v-model:value="formState.summary"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="一句话总结今天的结果"
        />
      </Form.Item>
      <Form.Item label="遇到的问题">
        <Textarea
          v-model:value="formState.issue"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="记录阻塞点、风险或疑问"
        />
      </Form.Item>
      <Form.Item label="明日计划">
        <Textarea
          v-model:value="formState.nextPlan"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="写下下一步动作，方便第二天直接接上"
        />
      </Form.Item>
    </Form>
  </Modal>
</template>

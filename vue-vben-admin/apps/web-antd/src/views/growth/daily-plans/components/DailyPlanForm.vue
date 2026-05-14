<script lang="ts" setup>
import { computed, reactive, ref, watch } from 'vue';

import {
  DatePicker,
  Form,
  Input,
  message,
  Modal,
  Select,
  TimePicker,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import { taskApi } from '#/api/growth';

const props = defineProps<{
  id?: null | string;
  open: boolean;
}>();

const emit = defineEmits<{
  success: [];
  'update:open': [value: boolean];
}>();

const loading = ref(false);
const Textarea = Input.TextArea;

interface TaskFormState {
  description: string;
  endTime: string;
  planDate: string;
  priority: number;
  remark: string;
  startTime: string;
  status: number;
  title: string;
  taskType: string;
  source: string;
}

const formState = reactive<TaskFormState>({
  description: '',
  endTime: '',
  planDate: '',
  priority: 2,
  remark: '',
  startTime: '',
  status: 0,
  title: '',
  taskType: 'Personal',
  source: 'Growth',
});

const modalTitle = computed(() => (props.id ? '编辑任务' : '新增任务'));

const statusOptions = [
  { label: '未开始', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const priorityOptions = [
  { label: '低', value: 1 },
  { label: '中', value: 2 },
  { label: '高', value: 3 },
  { label: '紧急', value: 4 },
];

const priorityStringToNumber: Record<string, number> = {
  Low: 1,
  Medium: 2,
  High: 3,
  Urgent: 4,
};

const statusStringToNumber: Record<string, number> = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
};

function fillForm(plan?: any) {
  Object.assign(formState, {
    description: plan?.description ?? '',
    endTime: plan?.endTime ?? '',
    planDate: plan?.planDate ?? new Date().toISOString().slice(0, 10),
    priority: typeof plan?.priority === 'string'
      ? (priorityStringToNumber[plan.priority] ?? 2)
      : (plan?.priority ?? 2),
    remark: plan?.remark ?? '',
    startTime: plan?.startTime ?? '',
    status: typeof plan?.status === 'string'
      ? (statusStringToNumber[plan.status] ?? 0)
      : (plan?.status ?? 0),
    title: plan?.title ?? '',
    taskType: plan?.type ?? 'Personal',
    source: plan?.source ?? 'Growth',
  });
}

function onPlanDateChange(val: dayjs.Dayjs | null | string) {
  formState.planDate = val && typeof val !== 'string' ? val.format('YYYY-MM-DD') : '';
}

function onStartTimeChange(val: dayjs.Dayjs | null | string) {
  formState.startTime = val && typeof val !== 'string' ? val.format('HH:mm') : '';
}

function onEndTimeChange(val: dayjs.Dayjs | null | string) {
  formState.endTime = val && typeof val !== 'string' ? val.format('HH:mm') : '';
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }

  loading.value = true;
  try {
    const result = await taskApi.getById(props.id);
    fillForm(result);
  } catch {
    message.error('加载详情失败');
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.title.trim() || !formState.planDate) {
    message.warning('请填写计划日期和标题');
    return;
  }

  loading.value = true;
  try {
    const payload = {
      description: formState.description || undefined,
      endTime: formState.endTime || undefined,
      planDate: formState.planDate,
      priority: Number(formState.priority) || 2,
      remark: formState.remark || undefined,
      startTime: formState.startTime || undefined,
      title: formState.title.trim(),
      taskType: formState.taskType,
      source: formState.source,
    };

    if (props.id) {
      await taskApi.update(props.id, payload);
      message.success('任务已更新');
    } else {
      await taskApi.create(payload);
      message.success('任务已创建');
    }

    emit('update:open', false);
    emit('success');
  } catch {
    message.error('保存失败');
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
    :title="modalTitle"
    width="680px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <div class="space-y-4">
      <Form :model="formState" layout="vertical">
        <Form.Item label="任务标题" required>
          <Input v-model:value="formState.title" placeholder="今天最重要的任务" />
        </Form.Item>
        <Form.Item label="任务内容">
          <Textarea
            v-model:value="formState.description"
            :auto-size="{ minRows: 3, maxRows: 6 }"
            placeholder="补充任务内容、关键步骤或验收标准"
          />
        </Form.Item>
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Form.Item label="计划日期" required>
            <DatePicker
              style="width: 100%"
              :value="formState.planDate ? dayjs(formState.planDate) : undefined"
              format="YYYY-MM-DD"
              @change="onPlanDateChange"
            />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="formState.status" :options="statusOptions" />
          </Form.Item>
          <Form.Item label="开始时间">
            <TimePicker
              style="width: 100%"
              format="HH:mm"
              :value="formState.startTime ? dayjs(formState.startTime, 'HH:mm') : undefined"
              @change="onStartTimeChange"
            />
          </Form.Item>
          <Form.Item label="结束时间">
            <TimePicker
              style="width: 100%"
              format="HH:mm"
              :value="formState.endTime ? dayjs(formState.endTime, 'HH:mm') : undefined"
              @change="onEndTimeChange"
            />
          </Form.Item>
          <Form.Item label="优先级">
            <Select v-model:value="formState.priority" :options="priorityOptions" />
          </Form.Item>
        </div>
        <Form.Item label="备注">
          <Textarea
            v-model:value="formState.remark"
            :auto-size="{ minRows: 2, maxRows: 4 }"
            placeholder="复盘记录、阻塞点或临时想法"
          />
        </Form.Item>
      </Form>
    </div>
  </Modal>
</template>

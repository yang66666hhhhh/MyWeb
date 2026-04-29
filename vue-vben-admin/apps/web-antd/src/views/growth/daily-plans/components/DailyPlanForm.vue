<script lang="ts" setup>
import { computed, reactive, ref, watch } from 'vue';

import {
  Alert,
  DatePicker,
  Form,
  Input,
  message,
  Modal,
  Select,
  TimePicker,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import type { DailyPlan, DailyPlanPriority, DailyPlanStatus } from '#/api/growth';

import { createDailyPlanApi, getDailyPlanApi, updateDailyPlanApi } from '#/api/growth';

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
const completedAtDisplay = ref<string>('');

interface DailyPlanFormState {
  description: string;
  endTime: string;
  planDate: string;
  priority: DailyPlanPriority;
  remark: string;
  startTime: string;
  status: DailyPlanStatus;
  title: string;
}

const formState = reactive<DailyPlanFormState>({
  description: '',
  endTime: '',
  planDate: '',
  priority: 3,
  remark: '',
  startTime: '',
  status: 0,
  title: '',
});

const modalTitle = computed(() => (props.id ? '编辑每日计划' : '新增每日计划'));

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
  { label: '紧急+', value: 5 },
];

function fillForm(plan?: DailyPlan) {
  Object.assign(formState, {
    description: plan?.description ?? '',
    endTime: plan?.endTime ?? '',
    planDate: plan?.planDate ?? new Date().toISOString().slice(0, 10),
    priority: plan?.priority ?? 3,
    remark: plan?.remark ?? '',
    startTime: plan?.startTime ?? '',
    status: plan?.status ?? 0,
    title: plan?.title ?? '',
  });
  completedAtDisplay.value = plan?.completedAt
    ? dayjs(plan.completedAt).format('YYYY-MM-DD HH:mm')
    : '';
}

function onPlanDateChange(val: null | string | dayjs.Dayjs) {
  formState.planDate = val && typeof val !== 'string' ? val.format('YYYY-MM-DD') : '';
}

function onStartTimeChange(val: null | string | dayjs.Dayjs) {
  formState.startTime = val && typeof val !== 'string' ? val.format('HH:mm') : '';
}

function onEndTimeChange(val: null | string | dayjs.Dayjs) {
  formState.endTime = val && typeof val !== 'string' ? val.format('HH:mm') : '';
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }

  loading.value = true;
  try {
    fillForm(await getDailyPlanApi(props.id));
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.title.trim() || !formState.planDate) {
    message.warning('请填写计划日期和标题');
    return;
  }

  if (
    formState.startTime &&
    formState.endTime &&
    formState.startTime > formState.endTime
  ) {
    message.warning('结束时间不能早于开始时间');
    return;
  }

  loading.value = true;
  try {
    const payload = {
      description: formState.description || null,
      endTime: formState.endTime || null,
      planDate: formState.planDate,
      priority: formState.priority,
      remark: formState.remark || null,
      startTime: formState.startTime || null,
      status: formState.status,
      title: formState.title.trim(),
    };

    if (props.id) {
      await updateDailyPlanApi(props.id, payload);
      message.success('每日计划已更新');
    } else {
      await createDailyPlanApi(payload);
      message.success('每日计划已创建');
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
    :title="modalTitle"
    width="680px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <div class="space-y-4">
      <Alert
        message="开始时间和结束时间当前先由前端本地预留保存，后端 DailyPlan 真实接口暂未提供对应字段。"
        type="info"
        show-icon
      />
      <Form :model="formState" layout="vertical">
        <Form.Item label="计划标题" required>
          <Input v-model:value="formState.title" placeholder="今天最重要的任务" />
        </Form.Item>
        <Form.Item label="计划内容">
          <Textarea
            v-model:value="formState.description"
            :auto-size="{ minRows: 3, maxRows: 6 }"
            placeholder="补充计划内容、关键步骤或验收标准"
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
        <Form.Item v-if="completedAtDisplay" label="完成时间">
          <Input :value="completedAtDisplay" disabled />
        </Form.Item>
      </Form>
    </div>
  </Modal>
</template>

<script lang="ts" setup>
import { computed, reactive, ref, watch } from 'vue';

import { Form, Input, message, Modal, Select } from 'ant-design-vue';

import type { Habit, HabitStatus } from '#/api/growth';

import { createHabitApi, getHabitApi, updateHabitApi } from '#/api/growth';

const props = defineProps<{
  id?: null | string;
  open: boolean;
}>();

const emit = defineEmits<{
  success: [];
  'update:open': [value: boolean];
}>();

const loading = ref(false);
const title = computed(() => (props.id ? '编辑习惯' : '新增习惯'));
const Textarea = Input.TextArea;

interface HabitFormState {
  description: string;
  habitType: string;
  name: string;
  status: HabitStatus;
  targetFrequency: string;
}

const formState = reactive<HabitFormState>({
  description: '',
  habitType: '学习',
  name: '',
  status: 1,
  targetFrequency: '每天',
});

const statusOptions = [
  { label: '停用', value: 0 },
  { label: '启用', value: 1 },
];

const frequencyOptions = [
  { label: '每天', value: '每天' },
  { label: '工作日', value: '工作日' },
  { label: '每周 3 次', value: '每周 3 次' },
  { label: '每周 4 次', value: '每周 4 次' },
];

const habitTypeOptions = [
  { label: '学习', value: '学习' },
  { label: '工作', value: '工作' },
  { label: '生活', value: '生活' },
  { label: '健康', value: '健康' },
];

function fillForm(habit?: Habit) {
  Object.assign(formState, {
    description: habit?.description ?? '',
    habitType: habit?.habitType ?? '学习',
    name: habit?.name ?? '',
    status: habit?.status ?? 1,
    targetFrequency: habit?.targetFrequency ?? '每天',
  });
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }

  loading.value = true;
  try {
    fillForm(await getHabitApi(props.id));
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.name.trim()) {
    message.warning('请填写习惯名称');
    return;
  }

  loading.value = true;
  try {
    if (props.id) {
      await updateHabitApi(props.id, { ...formState });
      message.success('习惯已更新');
    } else {
      await createHabitApi({ ...formState });
      message.success('习惯已创建');
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
    :title="title"
    width="560px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <Form :model="formState" layout="vertical">
      <Form.Item label="习惯名称" required>
        <Input v-model:value="formState.name" placeholder="例如：英语阅读" />
      </Form.Item>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-3">
        <Form.Item label="习惯类型">
          <Select
            v-model:value="formState.habitType"
            :options="habitTypeOptions"
          />
        </Form.Item>
        <Form.Item label="打卡频率">
          <Select
            v-model:value="formState.targetFrequency"
            :options="frequencyOptions"
          />
        </Form.Item>
        <Form.Item label="状态">
          <Select v-model:value="formState.status" :options="statusOptions" />
        </Form.Item>
      </div>
      <Form.Item label="描述">
        <Textarea
          v-model:value="formState.description"
          :auto-size="{ minRows: 3, maxRows: 5 }"
          placeholder="记录触发条件、完成标准或奖励"
        />
      </Form.Item>
    </Form>
  </Modal>
</template>

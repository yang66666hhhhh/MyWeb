<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  DatePicker,
  Divider,
  Empty,
  Form,
  Input,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
  Typography,
} from 'ant-design-vue';

import type { AiPlan } from '#/api/ai';

import { aiApi } from '#/api/ai';

const accessStore = useAccessStore();
const loading = ref(false);
const generating = ref(false);
const plans = ref<AiPlan[]>([]);
const generatedPlan = ref<string | null>(null);
const viewModalVisible = ref(false);
const currentPlan = ref<AiPlan | null>(null);

const formRef = ref();
const formState = reactive({
  type: 'Daily',
  targetDate: null as any,
  description: '',
});

const formRules = {
  type: [{ required: true, message: '请选择计划类型', type: 'string' as const, trigger: 'change' as const }],
};

const canUsePlanner = accessStore.accessCodes.includes('AI_PLANNER');

const planTypeOptions = [
  { label: '每日计划', value: 'Daily' },
  { label: '每周计划', value: 'Weekly' },
  { label: '每月计划', value: 'Monthly' },
  { label: '项目计划', value: 'Project' },
  { label: '自定义', value: 'Custom' },
];

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '类型', dataIndex: 'type', key: 'type' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt' },
  { title: '操作', key: 'action' },
];

const statusColors: Record<string, string> = {
  Pending: 'default',
  Generating: 'processing',
  Completed: 'success',
  Failed: 'error',
};

const statusLabels: Record<string, string> = {
  Pending: '待处理',
  Generating: '生成中',
  Completed: '已完成',
  Failed: '失败',
};

async function generatePlan() {
  try {
    await formRef.value?.validate();
  } catch {
    return;
  }

  generating.value = true;
  try {
    const plan = await aiApi.generatePlan({
      type: formState.type,
      description: formState.description,
      targetDate: formState.targetDate?.format?.('YYYY-MM-DD'),
    });
    generatedPlan.value = plan.generatedContent || '计划已生成';
    message.success('计划生成成功');
    await fetchPlans();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '生成失败');
  } finally {
    generating.value = false;
  }
}

async function fetchPlans() {
  loading.value = true;
  try {
    const result = await aiApi.getPlans({ page: 1, pageSize: 100 });
    plans.value = result.items || [];
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载计划列表失败');
  } finally {
    loading.value = false;
  }
}

function viewPlan(plan: Record<string, any>) {
  currentPlan.value = plan as AiPlan;
  viewModalVisible.value = true;
}

async function deletePlan(id: string) {
  try {
    await aiApi.deletePlan(id);
    message.success('删除成功');
    await fetchPlans();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

onMounted(() => {
  fetchPlans();
});
</script>

<template>
  <Page description="AI 驱动的智能计划生成" title="智能计划">
    <div class="space-y-4">
      <Card title="AI 智能计划生成">
        <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
          <Form.Item label="计划类型" name="type">
            <Select v-model:value="formState.type" :options="planTypeOptions" placeholder="选择计划类型" />
          </Form.Item>

          <Form.Item label="目标日期">
            <DatePicker v-model:value="formState.targetDate" format="YYYY-MM-DD" style="width: 100%" />
          </Form.Item>

          <Form.Item label="需求描述">
            <Input.TextArea
              v-model:value="formState.description"
              placeholder="描述您的计划需求..."
              :rows="4"
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" :loading="generating" :disabled="!canUsePlanner" @click="generatePlan">
              生成计划
            </Button>
          </Form.Item>
        </Form>

        <Divider />

        <div v-if="generatedPlan">
          <Typography.Title :level="4">生成的计划</Typography.Title>
          <div class="whitespace-pre-wrap rounded bg-gray-50 p-4">
            {{ generatedPlan }}
          </div>
        </div>

        <Empty v-else-if="!generating" description="点击上方按钮生成AI计划" />
      </Card>

      <Card title="历史计划">
        <Table
          :columns="columns"
          :data-source="plans"
          :loading="loading"
          :pagination="{ pageSize: 10 }"
          row-key="id"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'status'">
              <Tag :color="statusColors[record.status]">{{ statusLabels[record.status] || record.status }}</Tag>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button type="link" size="small" @click="viewPlan(record)">查看</Button>
                <Popconfirm title="确定删除?" @confirm="deletePlan(record.id)">
                  <Button type="link" danger size="small">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>

      <Modal v-model:open="viewModalVisible" title="计划详情" width="800px" :footer="null">
        <div class="whitespace-pre-wrap">{{ currentPlan?.generatedContent }}</div>
      </Modal>
    </div>
  </Page>
</template>

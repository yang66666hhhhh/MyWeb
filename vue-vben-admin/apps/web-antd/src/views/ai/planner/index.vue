<template>
  <div class="p-4">
    <Card title="AI 智能计划生成">
      <Form :model="formState" layout="vertical">
        <FormItem label="计划类型">
          <Select v-model:value="formState.type" placeholder="选择计划类型">
            <Select.Option value="Daily">每日计划</Select.Option>
            <Select.Option value="Weekly">每周计划</Select.Option>
            <Select.Option value="Monthly">每月计划</Select.Option>
            <Select.Option value="Project">项目计划</Select.Option>
            <Select.Option value="Custom">自定义</Select.Option>
          </Select>
        </FormItem>

        <FormItem label="目标日期">
          <DatePicker v-model:value="formState.targetDate" format="YYYY-MM-DD" />
        </FormItem>

        <FormItem label="需求描述">
          <Textarea
            v-model:value="formState.description"
            placeholder="描述您的计划需求..."
            :rows="4"
          />
        </FormItem>

        <FormItem>
          <Button type="primary" :loading="generating" @click="generatePlan">
            生成计划
          </Button>
        </FormItem>
      </Form>

      <Divider />

      <div v-if="generatedPlan" class="generated-plan">
        <Typography.Title :level="4">生成的计划</Typography.Title>
        <Typography.Paragraph>
          <pre style="white-space: pre-wrap; background: #f5f5f5; padding: 16px; border-radius: 4px;">
            {{ generatedPlan }}
          </pre>
        </Typography.Paragraph>
      </div>

      <Empty v-else-if="!generating" description="点击上方按钮生成AI计划" />
    </Card>

    <Card title="历史计划" class="mt-4">
      <Table
        :columns="columns"
        :data-source="plans"
        :loading="loading"
        :pagination="{ pageSize: 10 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="getStatusColor(record.status)">{{ record.status }}</Tag>
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
      <pre style="white-space: pre-wrap;">{{ currentPlan?.generatedContent }}</pre>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import {
  Button,
  Card,
  DatePicker,
  Divider,
  Empty,
  Form,
  FormItem,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
  Textarea,
  Typography,
  message,
} from 'ant-design-vue';
import { aiApi, type AiPlan } from '#/api/ai';

const loading = ref(false);
const generating = ref(false);
const plans = ref<AiPlan[]>([]);
const generatedPlan = ref<string | null>(null);
const viewModalVisible = ref(false);
const currentPlan = ref<AiPlan | null>(null);

const formState = reactive({
  type: 'Daily',
  targetDate: null as any,
  description: '',
});

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '类型', dataIndex: 'type', key: 'type' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt' },
  { title: '操作', key: 'action' },
];

const generatePlan = async () => {
  if (!formState.type) {
    message.warning('请选择计划类型');
    return;
  }

  generating.value = true;
  try {
    const response = await aiApi.generatePlan({
      type: formState.type,
      description: formState.description,
      targetDate: formState.targetDate?.format('YYYY-MM-DD'),
    });

    if (response.code === 0 && response.data) {
      generatedPlan.value = response.data.generatedContent || '计划已生成';
      message.success('计划生成成功');
      await fetchPlans();
    } else {
      message.error(response.message || '生成失败');
    }
  } catch (error: any) {
    message.error(error.message || '生成失败');
  } finally {
    generating.value = false;
  }
};

const fetchPlans = async () => {
  loading.value = true;
  try {
    const response = await aiApi.getPlans({ page: 1, pageSize: 100 });
    if (response.code === 0 && response.data) {
      plans.value = response.data.items || [];
    }
  } catch {
    message.error('加载计划列表失败');
  } finally {
    loading.value = false;
  }
};

const viewPlan = (plan: AiPlan) => {
  currentPlan.value = plan;
  viewModalVisible.value = true;
};

const deletePlan = async (id: string) => {
  try {
    const response = await aiApi.deletePlan(id);
    if (response.code === 0) {
      message.success('删除成功');
      await fetchPlans();
    } else {
      message.error(response.message || '删除失败');
    }
  } catch (error) {
    message.error('删除失败');
  }
};

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    Pending: 'default',
    Generating: 'processing',
    Completed: 'success',
    Failed: 'error',
  };
  return colors[status] || 'default';
};

onMounted(() => {
  fetchPlans();
});
</script>

<style scoped>
.mt-4 {
  margin-top: 16px;
}
</style>

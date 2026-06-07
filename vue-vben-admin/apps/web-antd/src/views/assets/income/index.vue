<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { EchartsUI, useEcharts } from '@vben/plugins/echarts';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  DatePicker,
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

import type { CreateIncomeInput, Income, MonthlyTrend } from '#/api/assets';

import { assetApi, createIncomeApi, deleteIncomeApi, getIncomePageApi, updateIncomeApi } from '#/api/assets';
import ExportButton from '#/components/ExportButton.vue';
import ImportButton from '#/components/ImportButton.vue';

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入标题', type: 'string' as const, trigger: 'blur' as const }],
  incomeDate: [{ required: true, message: '请选择日期', type: 'string' as const, trigger: 'change' as const }],
  amount: [{ required: true, message: '请输入金额', type: 'number' as const, trigger: 'blur' as const }],
};

const loading = ref(false);
const dataList = ref<Income[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateIncome = computed(() => accessStore.accessCodes.includes('ASSET_INCOME'));
const canEditIncome = computed(() => accessStore.accessCodes.includes('ASSET_INCOME'));
const canDeleteIncome = computed(() => accessStore.accessCodes.includes('ASSET_INCOME'));

const formState = reactive<CreateIncomeInput>({
  incomeDate: '',
  category: '',
  title: '',
  amount: 0,
  description: '',
  remark: '',
});

const categoryOptions = ['固定收入', '兼职收入', '投资收益', '奖金', '其他'];

const monthlyIncome = computed(() => {
  const now = new Date();
  return dataList.value
    .filter((item) => {
      const date = new Date(item.incomeDate);
      return date.getFullYear() === now.getFullYear() && date.getMonth() === now.getMonth();
    })
    .reduce((sum, item) => sum + item.amount, 0);
});

const trendChartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(trendChartRef);

const fetchIncomeTrend = async () => {
  try {
    const data: MonthlyTrend[] = await assetApi.getIncomeTrend(6);
    renderEcharts({
      tooltip: { trigger: 'axis' as const },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category' as const,
        data: data.map((item) => item.month),
        axisTick: { show: false },
      },
      yAxis: { type: 'value' as const, axisTick: { show: false } },
      series: [
        {
          type: 'bar' as const,
          itemStyle: { color: '#52c41a', borderRadius: [4, 4, 0, 0] },
          data: data.map((item) => item.amount),
        },
      ],
    });
  } catch {
    // ignore
  }
};

const columns = [
  { title: '日期', dataIndex: 'incomeDate', key: 'incomeDate', width: 120 },
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '金额', key: 'amount', width: 120 },
  { title: '分类', dataIndex: 'category', key: 'category', width: 100 },
  { title: '备注', dataIndex: 'remark', key: 'remark' },
  { title: '操作', key: 'action', width: 150 },
];

const categoryColors: Record<string, string> = {
  '固定收入': 'blue',
  '兼职收入': 'green',
  '投资收益': 'purple',
  '奖金': 'orange',
  '其他': 'default',
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getIncomePageApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    incomeDate: '',
    category: '',
    title: '',
    amount: 0,
    description: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Income) => {
  editingId.value = record.id;
  Object.assign(formState, {
    incomeDate: record.incomeDate,
    category: record.category,
    title: record.title,
    amount: record.amount,
    description: record.description || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteIncomeApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
  try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateIncomeApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createIncomeApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
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
  fetchIncomeTrend();
});
</script>

<template>
  <Page description="记录和管理收入" title="收入管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月收入" prefix="¥" :value="monthlyIncome" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="记录总数" :value="total" suffix="条" />
        </Card>
      </Col>
    </Row>

    <Card title="收入趋势（近6个月）" class="mb-4">
      <div class="h-72">
        <EchartsUI ref="trendChartRef" />
      </div>
    </Card>

    <Card title="收入记录">
      <template #extra>
        <Space>
          <ExportButton module="income" filename="收入记录" />
          <ImportButton module="income" @imported="fetchData" />
          <Button v-if="canCreateIncome" type="primary" @click="handleAdd">记录收入</Button>
        </Space>
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
          <template v-if="column.key === 'amount'">
            <span class="text-green-500">+¥{{ record.amount.toLocaleString() }}</span>
          </template>
          <template v-else-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category] || 'default'">{{ record.category }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditIncome" type="link" size="small" @click="handleEdit(record as Income)">编辑</Button>
              <Popconfirm v-if="canDeleteIncome" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑收入' : '记录收入'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <FormItem label="日期" required>
          <DatePicker v-model:value="formState.incomeDate" style="width: 100%" />
        </FormItem>
        <FormItem label="标题" required>
          <Input v-model:value="formState.title" placeholder="收入标题" />
        </FormItem>
        <FormItem label="金额" required>
          <InputNumber v-model:value="formState.amount" :min="0" :precision="2" style="width: 100%" prefix="¥" />
        </FormItem>
        <FormItem label="分类">
          <Select v-model:value="formState.category" placeholder="选择分类">
            <SelectOption v-for="cat in categoryOptions" :key="cat" :value="cat">{{ cat }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="描述">
          <Input v-model:value="formState.description" placeholder="描述" />
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

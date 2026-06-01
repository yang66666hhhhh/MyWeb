<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

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

import type { CreateInvestmentInput, Investment } from '#/api/assets';

import { createInvestmentApi, deleteInvestmentApi, getInvestmentPageApi, updateInvestmentApi } from '#/api/assets';

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入名称', type: 'string' as const, trigger: 'blur' as const }],
  investmentDate: [{ required: true, message: '请选择日期', type: 'string' as const, trigger: 'change' as const }],
  type: [{ required: true, message: '请选择类型', type: 'string' as const, trigger: 'change' as const }],
  amount: [{ required: true, message: '请输入金额', type: 'number' as const, trigger: 'blur' as const }],
};

const loading = ref(false);
const dataList = ref<Investment[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateInvestmentInput>({
  investmentDate: '',
  type: '',
  name: '',
  amount: 0,
  currentValue: 0,
  returnRate: 0,
  description: '',
  remark: '',
});

const typeOptions = ['基金', '股票', '债券', '货币基金', '存款', '其他'];

const totalAmount = computed(() => dataList.value.reduce((sum, item) => sum + item.amount, 0));
const totalCurrentValue = computed(() => dataList.value.reduce((sum, item) => sum + (item.currentValue || 0), 0));
const totalReturn = computed(() => totalCurrentValue.value - totalAmount.value);
const totalReturnRate = computed(() => totalAmount.value > 0 ? ((totalReturn.value / totalAmount.value) * 100).toFixed(2) : '0');

const columns = [
  { title: '日期', dataIndex: 'investmentDate', key: 'investmentDate', width: 120 },
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '类型', dataIndex: 'type', key: 'type', width: 100 },
  { title: '投入金额', key: 'amount', width: 120 },
  { title: '当前价值', key: 'currentValue', width: 120 },
  { title: '收益率', key: 'returnRate', width: 100 },
  { title: '操作', key: 'action', width: 150 },
];

const typeColors: Record<string, string> = {
  '基金': 'blue',
  '股票': 'red',
  '债券': 'green',
  '货币基金': 'cyan',
  '存款': 'orange',
  '其他': 'default',
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getInvestmentPageApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch (e: any) {
    message.error(e?.message || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    investmentDate: '',
    type: '',
    name: '',
    amount: 0,
    currentValue: 0,
    returnRate: 0,
    description: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Investment) => {
  editingId.value = record.id;
  Object.assign(formState, {
    investmentDate: record.investmentDate,
    type: record.type,
    name: record.name,
    amount: record.amount,
    currentValue: record.currentValue || 0,
    returnRate: record.returnRate || 0,
    description: record.description || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteInvestmentApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: any) {
    message.error(e?.message || '删除失败');
  }
};

const handleSubmit = async () => {
  try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateInvestmentApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createInvestmentApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch (e: any) {
    message.error(e?.message || '操作失败');
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
  <Page description="管理投资组合" title="投资管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="投资总额" prefix="¥" :value="totalAmount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="当前价值" prefix="¥" :value="totalCurrentValue" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总收益" prefix="¥" :value="totalReturn" :value-style="{ color: totalReturn >= 0 ? '#3f8600' : '#cf1322' }" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="收益率" :value="Number(totalReturnRate)" suffix="%" :value-style="{ color: Number(totalReturnRate) >= 0 ? '#3f8600' : '#cf1322' }" />
        </Card>
      </Col>
    </Row>

    <Card title="投资组合">
      <template #extra>
        <Button type="primary" @click="handleAdd">添加投资</Button>
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
            ¥{{ record.amount.toLocaleString() }}
          </template>
          <template v-else-if="column.key === 'currentValue'">
            ¥{{ (record.currentValue || 0).toLocaleString() }}
          </template>
          <template v-else-if="column.key === 'returnRate'">
            <span :class="(record.returnRate || 0) >= 0 ? 'text-green-500' : 'text-red-500'">
              {{ (record.returnRate || 0) >= 0 ? '+' : '' }}{{ (record.returnRate || 0).toFixed(2) }}%
            </span>
          </template>
          <template v-else-if="column.key === 'type'">
            <Tag :color="typeColors[record.type] || 'default'">{{ record.type }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Investment)">编辑</Button>
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
      :title="editingId ? '编辑投资' : '添加投资'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <FormItem label="日期" required>
          <DatePicker v-model:value="formState.investmentDate" style="width: 100%" />
        </FormItem>
        <FormItem label="名称" required>
          <Input v-model:value="formState.name" placeholder="投资名称" />
        </FormItem>
        <FormItem label="类型" required>
          <Select v-model:value="formState.type" placeholder="选择类型">
            <SelectOption v-for="t in typeOptions" :key="t" :value="t">{{ t }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="投入金额" required>
          <InputNumber v-model:value="formState.amount" :min="0" :precision="2" style="width: 100%" prefix="¥" />
        </FormItem>
        <FormItem label="当前价值">
          <InputNumber v-model:value="formState.currentValue" :min="0" :precision="2" style="width: 100%" prefix="¥" />
        </FormItem>
        <FormItem label="收益率 (%)">
          <InputNumber v-model:value="formState.returnRate" :precision="2" style="width: 100%" suffix="%" />
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

<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
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

import type { CreateExpenseInput, Expense } from '#/api/assets';

import { createExpenseApi, deleteExpenseApi, getExpensePageApi, updateExpenseApi } from '#/api/assets';

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入标题', type: 'string' as const, trigger: 'blur' as const }],
  expenseDate: [{ required: true, message: '请选择日期', type: 'string' as const, trigger: 'change' as const }],
  amount: [{ required: true, message: '请输入金额', type: 'number' as const, trigger: 'blur' as const }],
};

const loading = ref(false);
const dataList = ref<Expense[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateExpense = computed(() => accessStore.accessCodes.includes('ASSET_EXPENSE'));
const canEditExpense = computed(() => accessStore.accessCodes.includes('ASSET_EXPENSE'));
const canDeleteExpense = computed(() => accessStore.accessCodes.includes('ASSET_EXPENSE'));

const formState = reactive<CreateExpenseInput>({
  expenseDate: '',
  category: '',
  title: '',
  amount: 0,
  description: '',
  remark: '',
});

const categoryOptions = ['住房', '餐饮', '交通', '购物', '娱乐', '医疗', '教育', '其他'];

const monthlyExpense = computed(() => {
  const now = new Date();
  return dataList.value
    .filter((item) => {
      const date = new Date(item.expenseDate);
      return date.getFullYear() === now.getFullYear() && date.getMonth() === now.getMonth();
    })
    .reduce((sum, item) => sum + item.amount, 0);
});

const columns = [
  { title: '日期', dataIndex: 'expenseDate', key: 'expenseDate', width: 120 },
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '金额', key: 'amount', width: 120 },
  { title: '分类', dataIndex: 'category', key: 'category', width: 100 },
  { title: '备注', dataIndex: 'remark', key: 'remark' },
  { title: '操作', key: 'action', width: 150 },
];

const categoryColors: Record<string, string> = {
  '住房': 'red',
  '餐饮': 'orange',
  '交通': 'blue',
  '购物': 'purple',
  '娱乐': 'cyan',
  '医疗': 'magenta',
  '教育': 'green',
  '其他': 'default',
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getExpensePageApi({ page: currentPage.value, pageSize: pageSize.value });
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
    expenseDate: '',
    category: '',
    title: '',
    amount: 0,
    description: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Expense) => {
  editingId.value = record.id;
  Object.assign(formState, {
    expenseDate: record.expenseDate,
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
    await deleteExpenseApi(id);
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
      await updateExpenseApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createExpenseApi(formState);
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
});
</script>

<template>
  <Page description="记录和管理支出" title="支出管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月支出" prefix="¥" :value="monthlyExpense" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="记录总数" :value="total" suffix="条" />
        </Card>
      </Col>
    </Row>

    <Card title="支出记录">
      <template #extra>
        <Button v-if="canCreateExpense" type="primary" @click="handleAdd">记录支出</Button>
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
            <span class="text-red-500">-¥{{ record.amount.toLocaleString() }}</span>
          </template>
          <template v-else-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category] || 'default'">{{ record.category }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditExpense" type="link" size="small" @click="handleEdit(record as Expense)">编辑</Button>
              <Popconfirm v-if="canDeleteExpense" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑支出' : '记录支出'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <FormItem label="日期" required>
          <DatePicker v-model:value="formState.expenseDate" style="width: 100%" />
        </FormItem>
        <FormItem label="标题" required>
          <Input v-model:value="formState.title" placeholder="支出标题" />
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

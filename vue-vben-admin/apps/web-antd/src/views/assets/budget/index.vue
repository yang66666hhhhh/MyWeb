<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Progress,
  Row,
  Select,
  SelectOption,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { Budget, CreateBudgetInput } from '#/api/assets';

import { createBudgetApi, deleteBudgetApi, getBudgetPageApi, updateBudgetApi } from '#/api/assets';

const loading = ref(false);
const dataList = ref<Budget[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateBudgetInput>({
  year: new Date().getFullYear(),
  month: new Date().getMonth() + 1,
  category: '',
  plannedAmount: 0,
  actualAmount: 0,
  remark: '',
});

const categoryOptions = ['住房', '餐饮', '交通', '购物', '娱乐', '医疗', '教育', '其他'];

const totalPlanned = computed(() => dataList.value.reduce((sum, item) => sum + item.plannedAmount, 0));
const totalActual = computed(() => dataList.value.reduce((sum, item) => sum + item.actualAmount, 0));

const columns = [
  { title: '年月', key: 'period', width: 100 },
  { title: '分类', dataIndex: 'category', key: 'category', width: 100 },
  { title: '预算', key: 'plannedAmount', width: 120 },
  { title: '已花费', key: 'actualAmount', width: 120 },
  { title: '进度', key: 'progress', width: 180 },
  { title: '状态', key: 'status', width: 100 },
  { title: '操作', key: 'action', width: 150 },
];

const getStatus = (record: Budget) => {
  const percent = record.plannedAmount > 0 ? (record.actualAmount / record.plannedAmount) * 100 : 0;
  if (percent >= 100) return { text: '已用完', color: 'error' };
  if (percent >= 80) return { text: '接近', color: 'warning' };
  return { text: '正常', color: 'success' };
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getBudgetPageApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch {
    // ignore
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    year: new Date().getFullYear(),
    month: new Date().getMonth() + 1,
    category: '',
    plannedAmount: 0,
    actualAmount: 0,
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Budget) => {
  editingId.value = record.id;
  Object.assign(formState, {
    year: record.year,
    month: record.month,
    category: record.category,
    plannedAmount: record.plannedAmount,
    actualAmount: record.actualAmount,
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteBudgetApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  if (!formState.category || !formState.plannedAmount) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateBudgetApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createBudgetApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch {
    message.error('操作失败');
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
  <Page description="设定和追踪预算" title="预算管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总预算" prefix="¥" :value="totalPlanned" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="已花费" prefix="¥" :value="totalActual" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="剩余" prefix="¥" :value="totalPlanned - totalActual" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="使用率" :value="totalPlanned > 0 ? Math.round((totalActual / totalPlanned) * 100) : 0" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Card title="预算列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">设置预算</Button>
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
          <template v-if="column.key === 'period'">
            {{ record.year }}-{{ String(record.month).padStart(2, '0') }}
          </template>
          <template v-else-if="column.key === 'plannedAmount'">
            ¥{{ record.plannedAmount.toLocaleString() }}
          </template>
          <template v-else-if="column.key === 'actualAmount'">
            ¥{{ record.actualAmount.toLocaleString() }}
          </template>
          <template v-else-if="column.key === 'progress'">
            <Progress
              :percent="record.plannedAmount > 0 ? Math.round((record.actualAmount / record.plannedAmount) * 100) : 0"
              :status="record.actualAmount > record.plannedAmount ? 'exception' : 'active'"
            />
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="getStatus(record as Budget).color">{{ getStatus(record as Budget).text }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Budget)">编辑</Button>
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
      :title="editingId ? '编辑预算' : '设置预算'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form layout="vertical">
        <FormItem label="年份" required>
          <InputNumber v-model:value="formState.year" :min="2020" :max="2030" style="width: 100%" />
        </FormItem>
        <FormItem label="月份" required>
          <Select v-model:value="formState.month" placeholder="选择月份">
            <SelectOption v-for="m in 12" :key="m" :value="m">{{ m }}月</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="分类" required>
          <Select v-model:value="formState.category" placeholder="选择分类">
            <SelectOption v-for="cat in categoryOptions" :key="cat" :value="cat">{{ cat }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="预算金额" required>
          <InputNumber v-model:value="formState.plannedAmount" :min="0" :precision="2" style="width: 100%" prefix="¥" />
        </FormItem>
        <FormItem label="实际花费">
          <InputNumber v-model:value="formState.actualAmount" :min="0" :precision="2" style="width: 100%" prefix="¥" />
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

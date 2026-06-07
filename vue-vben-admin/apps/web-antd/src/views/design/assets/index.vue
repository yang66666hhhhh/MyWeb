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
  Row,
  Select,
  SelectOption,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateDesignAssetInput, DesignAsset } from '#/api/persona';

import {
  createDesignAssetApi,
  deleteDesignAssetApi,
  getDesignAssetsApi,
  updateDesignAssetApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<DesignAsset[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const formState = reactive<CreateDesignAssetInput>({
  name: '',
  description: '',
  category: '',
  fileUrl: '',
  fileType: '',
  fileSize: 0,
  tags: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入资源名称', type: 'string' as const }],
  category: [{ required: true, message: '请选择分类', type: 'string' as const }],
  fileUrl: [{ required: true, message: '请输入文件地址', type: 'string' as const }],
  fileType: [{ required: true, message: '请选择文件格式', type: 'string' as const }],};

const categoryOptions = ['图片', '图标', '设计系统', '插画', '字体', '其他'];
const fileTypeOptions = ['SVG', 'PNG', 'JPG', 'Figma', 'Sketch', 'PSD', 'AI', 'PDF'];

const columns = [
  { title: '资源名称', dataIndex: 'name', key: 'name' },
  { title: '分类', dataIndex: 'category', key: 'category', width: 100 },
  { title: '格式', dataIndex: 'fileType', key: 'fileType', width: 80 },
  { title: '大小', key: 'fileSize', width: 100 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const categoryColors: Record<string, string> = {
  '图片': 'blue',
  '设计系统': 'purple',
  '图标': 'green',
  '插画': 'orange',
  '字体': 'cyan',
  '其他': 'default',
};

const formatFileSize = (size: number) => {
  if (size < 1024) return `${size} B`;
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`;
  if (size < 1024 * 1024 * 1024) return `${(size / (1024 * 1024)).toFixed(1)} MB`;
  return `${(size / (1024 * 1024 * 1024)).toFixed(1)} GB`;
};

const categoryStats = computed(() => {
  const stats: Record<string, number> = {};
  dataList.value.forEach((a) => {
    if (a.category) {
      stats[a.category] = (stats[a.category] || 0) + 1;
    }
  });
  return Object.entries(stats).map(([name, count]) => ({ name, count }));
});

const colorAssets = computed(() =>
  dataList.value.filter((a) => a.category === '设计系统' || a.name.toLowerCase().includes('color')),
);

const fontAssets = computed(() =>
  dataList.value.filter((a) => a.category === '字体' || a.name.toLowerCase().includes('font')),
);

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getDesignAssetsApi({ page: currentPage.value, pageSize: pageSize.value });
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
    name: '',
    description: '',
    category: '',
    fileUrl: '',
    fileType: '',
    fileSize: 0,
    tags: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: DesignAsset) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    description: record.description || '',
    category: record.category,
    fileUrl: record.fileUrl,
    fileType: record.fileType,
    fileSize: record.fileSize,
    tags: record.tags || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteDesignAssetApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.name || !formState.category || !formState.fileUrl || !formState.fileType) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateDesignAssetApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createDesignAssetApi(formState);
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
  <Page description="管理设计资源、组件库和品牌资产" title="设计资产">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="资源总数" :value="total" />
        </Card>
      </Col>
      <Col v-for="stat in categoryStats" :key="stat.name" :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic :title="stat.name" :value="stat.count" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :md="24" :xs="24">
        <Card title="颜色库" :loading="loading">
          <div v-if="colorAssets.length > 0" class="flex flex-wrap gap-2">
            <div
              v-for="asset in colorAssets"
              :key="asset.id"
              class="w-20 h-20 rounded-lg border flex flex-col items-center justify-center cursor-pointer hover:shadow-md"
              @click="handleEdit(asset)"
            >
              <div class="w-12 h-12 rounded mb-1" :style="{ backgroundColor: '#5ab1ef' }" />
              <span class="text-xs truncate w-full text-center">{{ asset.name }}</span>
            </div>
          </div>
          <div v-else class="text-center text-gray-400 py-4">暂无颜色资源</div>
        </Card>
      </Col>
      <Col :lg="12" :md="24" :xs="24">
        <Card title="字体库" :loading="loading">
          <div v-if="fontAssets.length > 0" class="flex flex-wrap gap-2">
            <div
              v-for="asset in fontAssets"
              :key="asset.id"
              class="w-20 h-20 rounded-lg border flex flex-col items-center justify-center cursor-pointer hover:shadow-md"
              @click="handleEdit(asset)"
            >
              <span class="text-lg font-bold">Aa</span>
              <span class="text-xs truncate w-full text-center">{{ asset.name }}</span>
            </div>
          </div>
          <div v-else class="text-center text-gray-400 py-4">暂无字体资源</div>
        </Card>
      </Col>
    </Row>

    <Card title="设计资产列表">
      <template #extra>
        <Button type="primary" @click="handleAdd">上传资源</Button>
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
          <template v-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category] || 'default'">
              {{ record.category }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'fileSize'">
            {{ formatFileSize(record.fileSize) }}
          </template>
          <template v-else-if="column.key === 'tags'">
            <Space>
              <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag">{{ tag }}</Tag>
            </Space>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as DesignAsset)">编辑</Button>
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
      :title="editingId ? '编辑资源' : '上传资源'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="资源名称" required>
          <Input v-model:value="formState.name" placeholder="资源名称" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="资源描述" :rows="2" />
        </FormItem>
        <FormItem label="分类" required>
          <Select v-model:value="formState.category" placeholder="选择分类">
            <SelectOption v-for="cat in categoryOptions" :key="cat" :value="cat">{{ cat }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="文件地址" required>
          <Input v-model:value="formState.fileUrl" placeholder="文件 URL" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="文件格式" required>
              <Select v-model:value="formState.fileType" placeholder="选择格式">
                <SelectOption v-for="ft in fileTypeOptions" :key="ft" :value="ft">{{ ft }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="文件大小 (字节)">
              <InputNumber v-model:value="formState.fileSize" :min="0" style="width: 100%" />
            </FormItem>
          </Col>
        </Row>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

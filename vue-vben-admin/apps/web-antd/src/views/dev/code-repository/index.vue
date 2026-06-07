<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { computed, onMounted, reactive, ref, watch } from 'vue';

import { Page } from '@vben/common-ui';
import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  List,
  ListItem,
  ListItemMeta,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  SelectOption,
  Space,
  Statistic,
  Switch,
  Table,
  Tag,
  Tooltip,
} from 'ant-design-vue';

import type { CodeRepository, CreateCodeRepositoryInput } from '#/api/persona';

import {
  createRepositoryApi,
  deleteRepositoryApi,
  getRepositoriesApi,
  updateRepositoryApi,
} from '#/api/persona';

const loading = ref(false);
const dataList = ref<CodeRepository[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const formState = reactive<CreateCodeRepositoryInput>({
  name: '',
  description: '',
  url: '',
  language: '',
  isPublic: true,
  tags: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入仓库名称', type: 'string' as const }],
  url: [{ required: true, message: '请输入仓库地址', type: 'string' as const }],
  language: [{ required: true, message: '请选择编程语言', type: 'string' as const }],};

const languageOptions = ['TypeScript', 'JavaScript', 'Python', 'C#', 'Java', 'Go', 'Rust'];

const columns = [
  { title: '仓库名称', dataIndex: 'name', key: 'name' },
  { title: '描述', dataIndex: 'description', key: 'description', ellipsis: true },
  { title: '语言', dataIndex: 'language', key: 'language', width: 100 },
  { title: 'Stars', dataIndex: 'stars', key: 'stars', width: 80 },
  { title: '可见性', key: 'isPublic', width: 80 },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const languageColors: Record<string, string> = {
  TypeScript: 'blue',
  'C#': 'purple',
  Python: 'green',
  JavaScript: 'yellow',
  Java: 'orange',
  Go: 'cyan',
  Rust: 'red',
};

const totalStars = computed(() => dataList.value.reduce((sum, r) => sum + (r.stars || 0), 0));

const languageStats = computed(() => {
  const stats: Record<string, number> = {};
  dataList.value.forEach((r) => {
    if (r.language) {
      stats[r.language] = (stats[r.language] || 0) + 1;
    }
  });
  return Object.entries(stats).map(([name, value]) => ({ name, value }));
});

const recentRepos = computed(() =>
  [...dataList.value]
    .toSorted((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5),
);

watch(languageStats, (stats) => {
  if (stats.length > 0) {
    renderEcharts({
      tooltip: { trigger: 'item' as const },
      legend: { bottom: '0%', left: 'center' },
      series: [
        {
          data: stats,
          emphasis: {
            itemStyle: { shadowBlur: 10, shadowOffsetX: 0, shadowColor: 'rgba(0, 0, 0, 0.5)' },
          },
          itemStyle: { borderRadius: 6, borderWidth: 2 },
          label: { formatter: '{b}: {c} ({d}%)', show: true },
          name: '语言分布',
          radius: ['40%', '70%'],
          type: 'pie' as const,
        },
      ],
    });
  }
});

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getRepositoriesApi({ page: currentPage.value, pageSize: pageSize.value });
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
    url: '',
    language: '',
    isPublic: true,
    tags: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: CodeRepository) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    description: record.description || '',
    url: record.url,
    language: record.language,
    isPublic: record.isPublic,
    tags: record.tags || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteRepositoryApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.name || !formState.url || !formState.language) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateRepositoryApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createRepositoryApi(formState);
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
  <Page description="管理您的代码仓库，查看项目状态和统计信息" title="代码仓库">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="仓库总数" :value="total" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总 Stars" :value="totalStars" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :md="24" :xs="24">
        <Card title="语言分布" :loading="loading">
          <div class="h-72">
            <EchartsUI v-if="!loading && languageStats.length > 0" ref="chartRef" />
            <div v-else class="flex items-center justify-center h-full text-gray-400">
              暂无数据
            </div>
          </div>
        </Card>
      </Col>
      <Col :lg="12" :md="24" :xs="24">
        <Card title="最近更新" :loading="loading">
          <List :data-source="recentRepos" size="small">
            <template #renderItem="{ item }">
              <ListItem>
                <ListItemMeta>
                  <template #title>
                    <a :href="item.url" target="_blank" rel="noopener noreferrer">{{ item.name }}</a>
                  </template>
                  <template #description>
                    <Space>
                      <Tag :color="languageColors[item.language] || 'default'">{{ item.language }}</Tag>
                      <Tooltip title="Stars">
                        <span>⭐ {{ item.stars || 0 }}</span>
                      </Tooltip>
                    </Space>
                  </template>
                </ListItemMeta>
              </ListItem>
            </template>
          </List>
        </Card>
      </Col>
    </Row>

    <Card title="我的仓库">
      <template #extra>
        <Button type="primary" @click="handleAdd">新建仓库</Button>
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
          <template v-if="column.key === 'language'">
            <Tag :color="languageColors[record.language] || 'default'">
              {{ record.language }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'isPublic'">
            <Tag :color="record.isPublic ? 'green' : 'orange'">
              {{ record.isPublic ? '公开' : '私有' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as CodeRepository)">编辑</Button>
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
      :title="editingId ? '编辑仓库' : '新建仓库'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="仓库名称" required>
          <Input v-model:value="formState.name" placeholder="仓库名称" />
        </FormItem>
        <FormItem label="描述">
          <Input.TextArea v-model:value="formState.description" placeholder="仓库描述" :rows="2" />
        </FormItem>
        <FormItem label="仓库地址" required>
          <Input v-model:value="formState.url" placeholder="https://github.com/..." />
        </FormItem>
        <FormItem label="编程语言" required>
          <Select v-model:value="formState.language" placeholder="选择语言">
            <SelectOption v-for="lang in languageOptions" :key="lang" :value="lang">{{ lang }}</SelectOption>
          </Select>
        </FormItem>
        <FormItem label="公开仓库">
          <Switch v-model:checked="formState.isPublic" />
        </FormItem>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>

<template>
  <div class="p-4">
    <Card title="身份类型管理">
      <template #extra>
        <Button type="primary" @click="openCreateModal">
          <template #icon><PlusOutlined /></template>
          新增身份
        </Button>
      </template>

      <Table :columns="columns" :data-source="personas" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'icon'">
            <span style="font-size: 20px">{{ record.icon }}</span>
          </template>
          <template v-else-if="column.key === 'isActive'">
            <Tag :color="record.isActive ? 'green' : 'red'">
              {{ record.isActive ? '启用' : '禁用' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button type="link" size="small" @click="openEditModal(record)">编辑</Button>
              <Popconfirm title="确定删除?" @confirm="handleDelete(record.id)">
                <Button type="link" danger size="small">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingPersona ? '编辑身份' : '新增身份'"
      @ok="handleSubmit"
      :confirm-loading="submitting"
    >
      <Form :model="formState" :label-col="{ span: 6 }">
        <FormItem label="编码" name="code">
          <Input v-model:value="formState.code" placeholder="如: Developer" :disabled="!!editingPersona" />
        </FormItem>
        <FormItem label="名称" name="name">
          <Input v-model:value="formState.name" placeholder="如: 开发者" />
        </FormItem>
        <FormItem label="图标" name="icon">
          <Input v-model:value="formState.icon" placeholder="如: 💻" />
        </FormItem>
        <FormItem label="默认首页" name="defaultHomeRoute">
          <Input v-model:value="formState.defaultHomeRoute" placeholder="如: /dashboard/workspace" />
        </FormItem>
        <FormItem label="描述" name="description">
          <Textarea v-model:value="formState.description" :rows="2" />
        </FormItem>
        <FormItem label="排序" name="sort">
          <InputNumber v-model:value="formState.sort" :min="0" />
        </FormItem>
        <FormItem label="启用" name="isActive">
          <Switch v-model:checked="formState.isActive" />
        </FormItem>
      </Form>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import {
  Button,
  Card,
  Form,
  FormItem,
  Input,
  InputNumber,
  Modal,
  Popconfirm,
  Space,
  Switch,
  Table,
  Tag,
  Textarea,
  message,
} from 'ant-design-vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { personaApi, type PersonaType } from '#/api/system/persona';

const loading = ref(false);
const personas = ref<PersonaType[]>([]);
const modalVisible = ref(false);
const editingPersona = ref<PersonaType | null>(null);
const submitting = ref(false);

const formState = reactive({
  code: '',
  name: '',
  icon: '',
  description: '',
  defaultHomeRoute: '',
  sort: 0,
  isActive: true,
});

const columns = [
  { title: '图标', key: 'icon', width: 80 },
  { title: '编码', dataIndex: 'code', width: 120 },
  { title: '名称', dataIndex: 'name' },
  { title: '描述', dataIndex: 'description', ellipsis: true },
  { title: '默认首页', dataIndex: 'defaultHomeRoute' },
  { title: '排序', dataIndex: 'sort', width: 80 },
  { title: '状态', key: 'isActive', width: 80 },
  { title: '操作', key: 'action', width: 120 },
];

const fetchPersonas = async () => {
  loading.value = true;
  try {
    const res = await personaApi.getAll();
    if (Array.isArray(res)) {
      personas.value = res;
    } else if (res && (res as any).data) {
      personas.value = (res as any).data;
    }
  } catch (e) {
    console.error(e);
  } finally {
    loading.value = false;
  }
};

const openCreateModal = () => {
  editingPersona.value = null;
  Object.assign(formState, {
    code: '',
    name: '',
    icon: '',
    description: '',
    defaultHomeRoute: '',
    sort: personas.value.length,
    isActive: true,
  });
  modalVisible.value = true;
};

const openEditModal = (persona: Record<string, any>) => {
  const item = persona as PersonaType;
  editingPersona.value = item;
  Object.assign(formState, {
    code: item.code,
    name: item.name,
    icon: item.icon,
    description: item.description || '',
    defaultHomeRoute: item.defaultHomeRoute || '',
    sort: item.sort,
    isActive: item.isActive,
  });
  modalVisible.value = true;
};

const handleSubmit = async () => {
  submitting.value = true;
  try {
    if (editingPersona.value) {
      await personaApi.update(editingPersona.value.id, formState);
      message.success('更新成功');
      modalVisible.value = false;
      await fetchPersonas();
    } else {
      await personaApi.create(formState);
      message.success('创建成功');
      modalVisible.value = false;
      await fetchPersonas();
    }
  } catch (e) {
    message.error('操作失败');
  } finally {
    submitting.value = false;
  }
};

const handleDelete = async (id: string) => {
  try {
    await personaApi.delete(id);
    message.success('删除成功');
    await fetchPersonas();
  } catch (e) {
    message.error('删除失败');
  }
};

onMounted(() => {
  fetchPersonas();
});
</script>

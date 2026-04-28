<script lang="ts" setup>
import { reactive, ref, watch } from 'vue';

import { Form, Input, message, Modal, Select, Switch } from 'ant-design-vue';

import type { KnowledgeArticle, SaveKnowledgeArticleInput } from '#/api/growth';

import {
  createKnowledgeArticleApi,
  getKnowledgeArticleApi,
  updateKnowledgeArticleApi,
} from '#/api/growth';

const props = defineProps<{ id?: null | string; open: boolean }>();
const emit = defineEmits<{ success: []; 'update:open': [value: boolean] }>();

const loading = ref(false);
const Textarea = Input.TextArea;

const categoryOptions = [
  { label: '学习/数据结构', value: '学习/数据结构' },
  { label: '学习/数学', value: '学习/数学' },
  { label: '学习/英语', value: '学习/英语' },
  { label: '学习/政治', value: '学习/政治' },
  { label: '工作/工程实践', value: '工作/工程实践' },
  { label: '个人/方法论', value: '个人/方法论' },
];

const formState = reactive<SaveKnowledgeArticleInput>({
  category: '学习/数据结构',
  favorite: false,
  markdownContent: '',
  sourceUrl: '',
  summary: '',
  tags: [],
  title: '',
});

function fillForm(item?: KnowledgeArticle) {
  Object.assign(formState, {
    category: item?.category ?? '学习/数据结构',
    favorite: item?.favorite ?? false,
    markdownContent: item?.markdownContent ?? '',
    sourceUrl: item?.sourceUrl ?? '',
    summary: item?.summary ?? '',
    tags: item?.tags ?? [],
    title: item?.title ?? '',
  });
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }
  loading.value = true;
  try {
    fillForm((await getKnowledgeArticleApi(props.id)) as KnowledgeArticle);
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.title.trim()) {
    message.warning('请填写标题');
    return;
  }
  loading.value = true;
  try {
    if (props.id) {
      await updateKnowledgeArticleApi(props.id, { ...formState });
      message.success('笔记已更新');
    } else {
      await createKnowledgeArticleApi({ ...formState });
      message.success('笔记已创建');
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
    :title="id ? '编辑知识笔记' : '新增知识笔记'"
    width="820px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <Form :model="formState" layout="vertical">
      <Form.Item label="标题" required>
        <Input v-model:value="formState.title" placeholder="写下这条笔记的主题" />
      </Form.Item>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
        <Form.Item label="分类">
          <Select v-model:value="formState.category" :options="categoryOptions" />
        </Form.Item>
        <Form.Item label="标签">
          <Select
            v-model:value="formState.tags"
            mode="tags"
            :max-tag-count="5"
            placeholder="输入后回车，可自由扩展"
          />
        </Form.Item>
      </div>
      <Form.Item label="内容摘要">
        <Textarea
          v-model:value="formState.summary"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="快速说明这条笔记讲了什么"
        />
      </Form.Item>
      <Form.Item label="Markdown 内容">
        <Textarea
          v-model:value="formState.markdownContent"
          :auto-size="{ minRows: 8, maxRows: 16 }"
          placeholder="支持 Markdown 原文内容"
        />
      </Form.Item>
      <Form.Item label="来源链接">
        <Input v-model:value="formState.sourceUrl" placeholder="https://..." />
      </Form.Item>
      <Form.Item label="是否收藏">
        <Switch v-model:checked="formState.favorite" />
      </Form.Item>
    </Form>
  </Modal>
</template>

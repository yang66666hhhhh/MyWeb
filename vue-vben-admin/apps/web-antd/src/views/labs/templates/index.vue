<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Empty,
  Input,
  Row,
  Spin,
  Tag,
  message,
} from 'ant-design-vue';

import { getTemplatePageApi } from '#/api/work/template';

interface Template {
  id: string;
  name: string;
  description?: string;
  industry: string;
  tags?: string[];
}

const searchText = ref('');
const loading = ref(false);
const templates = ref<Template[]>([]);

const filteredTemplates = computed(() => {
  if (!searchText.value) return templates.value;
  const keyword = searchText.value.toLowerCase();
  return templates.value.filter(
    (t) =>
      t.name.toLowerCase().includes(keyword) ||
      (t.description && t.description.toLowerCase().includes(keyword)) ||
      t.industry.toLowerCase().includes(keyword),
  );
});

const industryColors: Record<string, string> = {
  IT: 'blue',
  Manufacturing: 'orange',
  Sales: 'green',
  Education: 'purple',
  Healthcare: 'red',
  Finance: 'gold',
};

async function loadTemplates() {
  loading.value = true;
  try {
    const res = await getTemplatePageApi({ page: 1, pageSize: 50 });
    templates.value = res.items.map((t) => ({
      id: t.id,
      name: t.name,
      description: t.description,
      industry: t.industry,
      tags: t.fields?.map((f) => f.fieldLabel) || [],
    }));
  } catch (e: unknown) {
    message.error(e instanceof Error ? e.message : '加载模板失败');
  } finally {
    loading.value = false;
  }
}

function handleUseTemplate(id: string) {
  message.success(`已选择模板: ${id}`);
}

onMounted(() => {
  loadTemplates();
});
</script>

<template>
  <Page title="模板市场" description="浏览和使用各种模板">
    <div class="space-y-4">
      <Card>
        <div class="flex items-center justify-between">
          <Input.Search
            v-model:value="searchText"
            placeholder="搜索模板..."
            style="max-width: 400px"
          />
          <span class="text-gray-500">共 {{ filteredTemplates.length }} 个模板</span>
        </div>
      </Card>

      <Spin :spinning="loading">
        <Row v-if="filteredTemplates.length > 0" :gutter="[16, 16]">
          <Col
            v-for="template in filteredTemplates"
            :key="template.id"
            :xs="24"
            :sm="12"
            :md="8"
            :lg="6"
          >
            <Card hoverable class="h-full">
              <template #cover>
                <div
                  class="flex h-32 items-center justify-center bg-gradient-to-br from-blue-50 to-purple-50"
                >
                  <span class="text-4xl"> </span>
                </div>
              </template>
              <Card.Meta
                :title="template.name"
                :description="template.description || '暂无描述'"
              />
              <div class="mt-3">
                <Tag :color="industryColors[template.industry] || 'default'">
                  {{ template.industry }}
                </Tag>
              </div>
              <div class="mt-3">
                <Button
                  type="primary"
                  size="small"
                  block
                  @click="handleUseTemplate(template.id)"
                >
                  使用模板
                </Button>
              </div>
            </Card>
          </Col>
        </Row>

        <Empty v-else-if="!loading" description="未找到匹配的模板" />
      </Spin>
    </div>
  </Page>
</template>

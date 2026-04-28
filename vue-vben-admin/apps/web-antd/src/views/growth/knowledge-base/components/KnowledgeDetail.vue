<script lang="ts" setup>
import { computed } from 'vue';

import { Descriptions, Space, Tag } from 'ant-design-vue';

import type { KnowledgeArticle } from '#/api/growth';

defineProps<{ item?: KnowledgeArticle | null }>();

function renderMarkdown(text: string): string {
  if (!text) return '';
  return text
    .replace(/^### (.+)$/gm, '<h3 class="text-lg font-semibold mt-3 mb-1">$1</h3>')
    .replace(/^## (.+)$/gm, '<h2 class="text-xl font-semibold mt-4 mb-2">$1</h2>')
    .replace(/^# (.+)$/gm, '<h1 class="text-2xl font-bold mt-4 mb-2">$1</h1>')
    .replace(/\*\*(.+?)\*\*/g, '<strong class="font-semibold">$1</strong>')
    .replace(/\*(.+?)\*/g, '<em class="italic">$1</em>')
    .replace(/`(.+?)`/g, '<code class="px-1 py-0.5 rounded bg-gray-200 text-red-600 text-sm">$1</code>')
    .replace(/^- (.+)$/gm, '<li class="ml-4">$1</li>')
    .replace(/(<li.*<\/li>)/s, '<ul class="list-disc my-2">$1</ul>')
    .replace(/\n\n/g, '</p><p class="my-2">')
    .replace(/\n/g, '<br />');
}
</script>

<template>
  <div v-if="item" class="space-y-4">
    <Descriptions bordered :column="1" size="small">
      <Descriptions.Item label="标题">{{ item.title }}</Descriptions.Item>
      <Descriptions.Item label="分类">
        <Tag color="purple">{{ item.category }}</Tag>
      </Descriptions.Item>
      <Descriptions.Item label="标签">
        <Space v-if="item.tags?.length" wrap>
          <Tag v-for="tag in item.tags" :key="tag">{{ tag }}</Tag>
        </Space>
        <span v-else>-</span>
      </Descriptions.Item>
      <Descriptions.Item label="内容摘要">{{ item.summary || '-' }}</Descriptions.Item>
      <Descriptions.Item label="是否收藏">
        <Tag :color="item.favorite ? 'gold' : 'default'">
          {{ item.favorite ? '已收藏' : '未收藏' }}
        </Tag>
      </Descriptions.Item>
      <Descriptions.Item label="来源链接">
        <a v-if="item.sourceUrl" :href="item.sourceUrl" target="_blank" class="text-blue-500 hover:underline">{{ item.sourceUrl }}</a>
        <span v-else>-</span>
      </Descriptions.Item>
    </Descriptions>
    <div v-if="item.markdownContent" class="border rounded p-4 bg-gray-50 dark:bg-gray-800">
      <div class="text-sm text-text-secondary mb-2 font-medium">Markdown 内容</div>
      <div class="prose dark:prose-invert max-w-none" v-html="renderMarkdown(item.markdownContent)" />
    </div>
  </div>
</template>

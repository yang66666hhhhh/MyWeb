<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Row,
  Tag,
} from 'ant-design-vue';
import {
  BookOutlined,
  LinkOutlined,
  PlayCircleOutlined,
} from '@ant-design/icons-vue';

interface Resource {
  id: string;
  title: string;
  description: string;
  type: 'article' | 'tool' | 'video';
  url: string;
  tags: string[];
}

const resources = ref<Resource[]>([
  {
    id: '1',
    title: '个人财务管理指南',
    description: '学习如何制定预算、储蓄和投资',
    type: 'article',
    url: '#',
    tags: ['理财', '入门'],
  },
  {
    id: '2',
    title: '时间管理技巧',
    description: '提高工作效率的时间管理方法',
    type: 'article',
    url: '#',
    tags: ['效率', '时间管理'],
  },
  {
    id: '3',
    title: '记账工具推荐',
    description: '好用的记账 APP 和工具',
    type: 'tool',
    url: '#',
    tags: ['工具', '记账'],
  },
  {
    id: '4',
    title: '投资入门教程',
    description: '基金、股票投资基础知识',
    type: 'video',
    url: '#',
    tags: ['投资', '理财'],
  },
  {
    id: '5',
    title: '效率工具合集',
    description: '提升工作效率的必备工具',
    type: 'tool',
    url: '#',
    tags: ['工具', '效率'],
  },
  {
    id: '6',
    title: '学习方法论',
    description: '高效学习的科学方法',
    type: 'article',
    url: '#',
    tags: ['学习', '方法'],
  },
]);

const typeIcons: Record<string, any> = {
  article: BookOutlined,
  video: PlayCircleOutlined,
  tool: LinkOutlined,
};

const typeLabels: Record<string, string> = {
  article: '文章',
  video: '视频',
  tool: '工具',
};

const typeColors: Record<string, string> = {
  article: 'blue',
  video: 'red',
  tool: 'green',
};
</script>

<template>
  <Page title="资源中心" description="学习资源和工具推荐">
    <div class="space-y-4">
      <Card>
        <div class="text-gray-500">
          精选的理财、效率和学习资源，帮助您更好地管理个人成长。
        </div>
      </Card>

      <Row :gutter="[16, 16]">
        <Col
          v-for="resource in resources"
          :key="resource.id"
          :xs="24"
          :sm="12"
          :md="8"
        >
          <Card hoverable class="h-full">
            <template #cover>
              <div
                class="flex h-24 items-center justify-center bg-gradient-to-br from-blue-50 to-green-50"
              >
                <component
                  :is="typeIcons[resource.type]"
                  style="font-size: 36px; color: #1890ff"
                />
              </div>
            </template>
            <Card.Meta
              :title="resource.title"
              :description="resource.description"
            />
            <div class="mt-3 flex flex-wrap gap-1">
              <Tag :color="typeColors[resource.type]">
                {{ typeLabels[resource.type] }}
              </Tag>
              <Tag v-for="tag in resource.tags" :key="tag">{{ tag }}</Tag>
            </div>
            <div class="mt-3">
              <Button type="link" :href="resource.url" block>
                查看资源
              </Button>
            </div>
          </Card>
        </Col>
      </Row>

      <Card title="资源统计">
        <Row :gutter="[16, 16]">
          <Col :span="8">
            <div class="text-center">
              <div class="text-2xl font-bold text-blue-500">
                {{ resources.filter((r) => r.type === 'article').length }}
              </div>
              <div class="text-gray-500">文章</div>
            </div>
          </Col>
          <Col :span="8">
            <div class="text-center">
              <div class="text-2xl font-bold text-red-500">
                {{ resources.filter((r) => r.type === 'video').length }}
              </div>
              <div class="text-gray-500">视频</div>
            </div>
          </Col>
          <Col :span="8">
            <div class="text-center">
              <div class="text-2xl font-bold text-green-500">
                {{ resources.filter((r) => r.type === 'tool').length }}
              </div>
              <div class="text-gray-500">工具</div>
            </div>
          </Col>
        </Row>
      </Card>
    </div>
  </Page>
</template>

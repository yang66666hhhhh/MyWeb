<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Input,
  List,
  message,
  Popconfirm,
  Row,
  Space,
  Statistic,
} from 'ant-design-vue';

import type { KnowledgeChatSession } from '#/api/ai/extended';

import {
  deleteKnowledgeChatSessionApi,
  getKnowledgeChatSessionsApi,
  sendKnowledgeChatMessageApi,
} from '#/api/ai/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const chatInput = ref('');
const sending = ref(false);
const selectedSessionId = ref<null | string>(null);

const { items, load, loading, query, total, changePage } = usePagedQuery<
  KnowledgeChatSession,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getKnowledgeChatSessionsApi,
});

async function handleSendMessage() {
  if (!chatInput.value.trim()) {
    message.warning('请输入消息');
    return;
  }
  sending.value = true;
  try {
    await sendKnowledgeChatMessageApi({
      message: chatInput.value,
      sessionId: selectedSessionId.value || undefined,
    });
    message.success('发送成功');
    chatInput.value = '';
    await load();
  } catch (e: any) {
    message.error(e?.message || '发送失败');
  } finally {
    sending.value = false;
  }
}

async function handleDeleteSession(id: string) {
  try {
    await deleteKnowledgeChatSessionApi(id);
    message.success('删除成功');
    if (selectedSessionId.value === id) {
      selectedSessionId.value = null;
    }
    await load();
  } catch (e: any) {
    message.error(e?.message || '删除失败');
  }
}

function handleSelectSession(id: string) {
  selectedSessionId.value = id;
}

function handlePageChange(page: number, pageSize: number) {
  void changePage(page, pageSize);
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="基于知识库的智能问答" title="知识问答">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="问答会话" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总消息数" :value="items.reduce((sum: number, i: KnowledgeChatSession) => sum + i.messageCount, 0)" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="8" :xs="24">
        <Card title="会话列表" class="h-full">
          <List
            :data-source="items"
            :loading="loading"
            size="small"
          >
            <template #renderItem="{ item }">
              <List.Item
                :class="{ 'bg-blue-50': selectedSessionId === item.id }"
                class="cursor-pointer"
                @click="handleSelectSession(item.id)"
              >
                <List.Item.Meta :title="item.title || '未命名会话'">
                  <template #description>
                    <div>
                      <div v-if="item.lastMessage" class="truncate">{{ item.lastMessage }}</div>
                      <div class="text-gray-400">{{ item.messageCount }} 条消息</div>
                    </div>
                  </template>
                </List.Item.Meta>
                <Popconfirm title="确认删除？" @confirm.stop="handleDeleteSession(item.id)">
                  <Button danger size="small" type="link" @click.stop>删除</Button>
                </Popconfirm>
              </List.Item>
            </template>
          </List>
          <div v-if="total > query.pageSize" class="mt-4 text-center">
            <Button type="link" @click="handlePageChange(query.page + 1, query.pageSize)">
              加载更多
            </Button>
          </div>
        </Card>
      </Col>

      <Col :lg="16" :xs="24">
        <Card title="智能问答" class="h-full">
          <div class="mb-4">
            <Space>
              <Input
                v-model:value="chatInput"
                placeholder="输入您的问题..."
                style="width: 400px"
                @press-enter="handleSendMessage"
              />
              <Button type="primary" :loading="sending" @click="handleSendMessage">
                发送
              </Button>
            </Space>
          </div>
          <div v-if="selectedSessionId" class="text-gray-500">
            当前会话: {{ items.find((i: KnowledgeChatSession) => i.id === selectedSessionId)?.title || selectedSessionId }}
          </div>
          <div v-else class="text-gray-400">
            选择一个会话或发送新消息开始问答
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>

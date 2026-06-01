<script lang="ts" setup>
import { ref } from 'vue';
import { Page } from '@vben/common-ui';
import { Button, Card, Input, List, Select, Space, Tag, message } from 'ant-design-vue';

const prompt = ref('');
const loading = ref(false);
const messages = ref<Array<{ role: string; content: string; time: string }>>([]);
const selectedModel = ref('gpt-4');
const temperature = ref(0.7);

const modelOptions = [
  { label: 'GPT-4', value: 'gpt-4' },
  { label: 'GPT-3.5 Turbo', value: 'gpt-3.5-turbo' },
  { label: 'Claude 3', value: 'claude-3' },
];

async function handleSend() {
  if (!prompt.value.trim()) return;
  
  const userMessage = { role: 'user', content: prompt.value, time: new Date().toLocaleTimeString() };
  messages.value.push(userMessage);
  
  loading.value = true;
  try {
    // Simulate AI response
    await new Promise(resolve => setTimeout(resolve, 1000));
    const aiMessage = { 
      role: 'assistant', 
      content: `[模拟响应] 收到您的消息: "${prompt.value}"。这是一个 AI 实验室的模拟响应，实际使用时会调用真实的 AI API。`,
      time: new Date().toLocaleTimeString()
    };
    messages.value.push(aiMessage);
  } catch (e: unknown) {
    message.error(e instanceof Error ? e.message : '请求失败');
  } finally {
    loading.value = false;
    prompt.value = '';
  }
}

function handleClear() {
  messages.value = [];
}
</script>

<template>
  <Page title="AI 实验室" description="交互式 AI 测试环境">
    <div class="space-y-4">
      <Card>
        <Space>
          <span>模型:</span>
          <Select v-model:value="selectedModel" :options="modelOptions" style="width: 150px" />
          <span>温度:</span>
          <Select v-model:value="temperature" style="width: 80px">
            <Select.Option :value="0">0</Select.Option>
            <Select.Option :value="0.3">0.3</Select.Option>
            <Select.Option :value="0.5">0.5</Select.Option>
            <Select.Option :value="0.7">0.7</Select.Option>
            <Select.Option :value="1">1</Select.Option>
          </Select>
          <Button @click="handleClear">清空对话</Button>
        </Space>
      </Card>

      <Card title="对话">
        <div class="mb-4 max-h-[500px] overflow-y-auto">
          <List :data-source="messages" :locale="{ emptyText: '暂无对话，请输入消息开始' }">
            <template #renderItem="{ item }">
              <List.Item>
                <div class="w-full" :class="item.role === 'user' ? 'text-right' : 'text-left'">
                  <Tag :color="item.role === 'user' ? 'blue' : 'green'">
                    {{ item.role === 'user' ? '用户' : 'AI' }}
                  </Tag>
                  <span class="ml-2 text-gray-400">{{ item.time }}</span>
                  <div class="mt-1 p-3 rounded" :class="item.role === 'user' ? 'bg-blue-50 inline-block' : 'bg-gray-50'">
                    {{ item.content }}
                  </div>
                </div>
              </List.Item>
            </template>
          </List>
        </div>
        
        <div class="flex gap-2">
          <Input.TextArea v-model:value="prompt" :rows="2" placeholder="输入消息..." @pressEnter="handleSend" />
          <Button type="primary" :loading="loading" @click="handleSend">发送</Button>
        </div>
      </Card>
    </div>
  </Page>
</template>
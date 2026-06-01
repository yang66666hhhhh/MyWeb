<script lang="ts" setup>
import { ref } from 'vue';
import { Page } from '@vben/common-ui';
import { Button, Card, Input, Select, Space, Table, message } from 'ant-design-vue';

const dataSource = ref('');
const chartType = ref('bar');
const parsedData = ref<any[]>([]);

const chartOptions = [
  { label: '柱状图', value: 'bar' },
  { label: '折线图', value: 'line' },
  { label: '饼图', value: 'pie' },
];

const columns = [
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '值', dataIndex: 'value', key: 'value' },
];

function handleParse() {
  try {
    const data = JSON.parse(dataSource.value);
    if (Array.isArray(data)) {
      parsedData.value = data;
      message.success('数据解析成功');
    } else {
      message.warning('请输入 JSON 数组格式');
    }
  } catch {
    message.error('JSON 格式错误');
  }
}

function handleLoadSample() {
  const sample = [
    { name: '一月', value: 120 },
    { name: '二月', value: 200 },
    { name: '三月', value: 150 },
    { name: '四月', value: 80 },
    { name: '五月', value: 170 },
  ];
  dataSource.value = JSON.stringify(sample, null, 2);
  parsedData.value = sample;
}
</script>

<template>
  <Page title="数据实验室" description="数据可视化和分析工具">
    <div class="space-y-4">
      <Card>
        <Space>
          <span>图表类型:</span>
          <Select v-model:value="chartType" :options="chartOptions" style="width: 120px" />
          <Button @click="handleLoadSample">加载示例数据</Button>
          <Button type="primary" @click="handleParse">解析数据</Button>
        </Space>
      </Card>

      <Card title="数据输入">
        <Input.TextArea v-model:value="dataSource" :rows="6" placeholder='输入 JSON 数组，如: [{"name": "A", "value": 100}]' />
      </Card>

      <Card title="数据预览">
        <Table :columns="columns" :data-source="parsedData" :pagination="false" row-key="name" />
      </Card>
    </div>
  </Page>
</template>
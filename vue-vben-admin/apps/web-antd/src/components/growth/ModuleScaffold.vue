<script lang="ts" setup>
import { Button, Card, Form, Input, Space, Table, Tag } from 'ant-design-vue';

defineProps<{
  endpoint: string;
  moduleName: string;
  permission: string;
}>();

const columns = [
  { dataIndex: 'name', key: 'name', title: '名称' },
  { dataIndex: 'status', key: 'status', title: '状态', width: 120 },
  { dataIndex: 'updatedAt', key: 'updatedAt', title: '更新时间', width: 180 },
  { key: 'action', title: '操作', width: 120 },
];

const locale = {
  emptyText: '后端模块接口就绪后可复用 DailyPlan/Habit 的列表和表单结构',
};
</script>

<template>
  <div>
    <Card class="mb-4">
      <Form layout="inline">
        <Form.Item label="关键词">
          <Input allow-clear placeholder="输入关键词查询" />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary"> 查询 </Button>
            <Button> 新增 </Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>

    <Card>
      <Table
        :columns="columns"
        :data-source="[]"
        :locale="locale"
        row-key="id"
      >
        <template #bodyCell="{ column }">
          <template v-if="column.key === 'status'">
            <Tag color="processing">进行中</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Button size="small" type="link">编辑</Button>
          </template>
        </template>
      </Table>
      <div class="mt-3 text-sm text-gray-500">
        {{ moduleName }} 接口预留：{{ endpoint }}，权限码：{{ permission }}
      </div>
    </Card>
  </div>
</template>

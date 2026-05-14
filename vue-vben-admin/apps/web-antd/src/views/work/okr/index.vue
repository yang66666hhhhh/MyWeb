<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const okrs = ref([
  { id: '1', objective: '提升产品质量', keyResults: ['Bug数量减少50%', '用户满意度提升到90%', '代码覆盖率达到80%'], progress: 65, status: '进行中' },
  { id: '2', objective: '提高开发效率', keyResults: ['部署时间减少30%', '自动化测试覆盖90%', '文档完善度100%'], progress: 45, status: '进行中' },
  { id: '3', objective: '技术能力提升', keyResults: ['团队培训4次', '技术分享8次', '新技术引入2个'], progress: 75, status: '进行中' },
]);

const columns = [
  { title: '目标', dataIndex: 'objective', key: 'objective' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
  { title: '状态', dataIndex: 'status', key: 'status' },
];

const statusColors: Record<string, string> = {
  '进行中': 'processing',
  '已完成': 'success',
};
</script>

<template>
  <Page description="设定和追踪OKR目标" title="OKR管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="目标数" :value="okrs.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="关键结果" :value="9" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="62" suffix="%" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成率" :value="0" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="OKR列表">
      <template #extra><Button type="primary">新建OKR</Button></template>
      <Table :columns="columns" :data-source="okrs" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'objective'">
            <div>
              <div class="font-bold mb-2">{{ record.objective }}</div>
              <div v-for="(kr, index) in record.keyResults" :key="index" class="text-gray-500 text-sm">
                {{ Number(index) + 1 }}. {{ kr }}
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="w-20 h-2 bg-gray-200 rounded-full">
                <div class="h-full bg-blue-500 rounded-full" :style="{ width: `${record.progress}%` }"></div>
              </div>
              <span>{{ record.progress }}%</span>
            </div>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ record.status }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>

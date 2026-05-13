<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, Row, Space, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const risks = ref([
  { id: '1', title: '技术方案不成熟', project: 'WebSite', level: '高', probability: '中', impact: '高', status: '已识别', mitigation: '进行技术预研和原型验证' },
  { id: '2', title: '人员变动', project: 'WebSite', level: '中', probability: '低', impact: '高', status: '监控中', mitigation: '知识共享和文档完善' },
  { id: '3', title: '需求变更频繁', project: 'WebSite', level: '中', probability: '高', impact: '中', status: '已识别', mitigation: '建立变更管理流程' },
]);

const columns = [
  { title: '风险', dataIndex: 'title', key: 'title' },
  { title: '项目', dataIndex: 'project', key: 'project' },
  { title: '等级', dataIndex: 'level', key: 'level' },
  { title: '概率', dataIndex: 'probability', key: 'probability' },
  { title: '影响', dataIndex: 'impact', key: 'impact' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '应对措施', dataIndex: 'mitigation', key: 'mitigation' },
];

const levelColors: Record<string, string> = {
  '高': 'red',
  '中': 'orange',
  '低': 'green',
};
</script>

<template>
  <Page description="识别和管理项目风险" title="风险管理">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="风险总数" :value="risks.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="高风险" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="中风险" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="低风险" :value="0" /></Card>
      </Col>
    </Row>

    <Card title="风险列表">
      <template #extra><Button type="primary">识别风险</Button></template>
      <Table :columns="columns" :data-source="risks" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'level'">
            <Tag :color="levelColors[record.level]">{{ record.level }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
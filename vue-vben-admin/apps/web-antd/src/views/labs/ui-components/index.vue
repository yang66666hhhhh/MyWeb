<script lang="ts" setup>
import { ref } from 'vue';
import { Page } from '@vben/common-ui';
import { Alert, Card, Col, Progress, Row, Space, Statistic, Tag, Timeline } from 'ant-design-vue';

const progress = ref(65);
</script>

<template>
  <Page title="UI 组件" description="展示可用的 UI 组件">
    <div class="space-y-4">
      <Card title="统计卡片">
        <Row :gutter="16">
          <Col :span="6">
            <Statistic title="总用户" :value="11280" />
          </Col>
          <Col :span="6">
            <Statistic title="活跃用户" :value="8920" :value-style="{ color: '#3f8600' }" />
          </Col>
          <Col :span="6">
            <Statistic title="新增今日" :value="236" />
          </Col>
          <Col :span="6">
            <Statistic title="转化率" :value="78.5" suffix="%" />
          </Col>
        </Row>
      </Card>

      <Card title="进度条">
        <Space direction="vertical" class="w-full">
          <Progress :percent="progress" />
          <Progress :percent="progress" status="active" />
          <Progress :percent="100" />
          <Progress :percent="progress" type="circle" />
        </Space>
      </Card>

      <Card title="标签">
        <Space>
          <Tag color="blue">Blue</Tag>
          <Tag color="green">Green</Tag>
          <Tag color="orange">Orange</Tag>
          <Tag color="red">Red</Tag>
          <Tag color="purple">Purple</Tag>
        </Space>
      </Card>

      <Card title="时间线">
        <Timeline>
          <Timeline.Item color="green">创建项目 2024-01-01</Timeline.Item>
          <Timeline.Item color="blue">开发阶段 2024-01-15</Timeline.Item>
          <Timeline.Item color="orange">测试阶段 2024-02-01</Timeline.Item>
          <Timeline.Item>发布上线 2024-02-15</Timeline.Item>
        </Timeline>
      </Card>

      <Card title="提示框">
        <Space direction="vertical" class="w-full">
          <Alert message="信息提示" type="info" show-icon />
          <Alert message="成功提示" type="success" show-icon />
          <Alert message="警告提示" type="warning" show-icon />
          <Alert message="错误提示" type="error" show-icon />
        </Space>
      </Card>
    </div>
  </Page>
</template>
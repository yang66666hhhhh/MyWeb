<script setup lang="ts">
import { Card, Descriptions, Space, Typography } from 'ant-design-vue';

const { Text, Title, Paragraph } = Typography;

const features = [
  { title: '个人成长', desc: '每日计划、习惯打卡、知识库、备考中心' },
  { title: '工作中心', desc: '项目管理、设备管理、工作日志、统计分析' },
  { title: '财务资产', desc: '收入管理、支出管理、投资记录、预算规划' },
  { title: '内容创作', desc: '文章管理、素材库、发布日历、SEO分析' },
  { title: '数据分析', desc: '成长分析、工作分析、财务分析、时间分析' },
  { title: 'AI 智能', desc: 'AI 助手、智能计划、周报生成、知识问答' },
];

const modules = [
  { name: '个人成长', path: '/growth' },
  { name: '工作中心', path: '/work' },
  { name: '财务资产', path: '/assets' },
  { name: '内容创作', path: '/content' },
  { name: '社交人脉', path: '/network' },
  { name: '数据分析', path: '/analytics' },
  { name: 'AI 智能', path: '/ai' },
  { name: '开放平台', path: '/platform' },
  { name: '实验室', path: '/labs' },
];
</script>

<template>
  <div class="p-6">
    <Space direction="vertical" :size="24" class="w-full">
      <Card title="Personal OS - 个人数字操作系统">
        <Text>
          基于 Vue 3 + .NET 10 构建的全栈应用，采用前后端分离架构，实现个人成长的全方位管理。
        </Text>
        <br />
        <Text type="secondary">
          前端：Vue 3.5 + Vite 6 + Ant Design Vue 4 | 后端：ASP.NET Core 10 + EF Core + MySQL
        </Text>
      </Card>

      <Card title="技术栈">
        <Descriptions :column="2" bordered size="small">
          <Descriptions.Item label="前端框架">Vue 3.5.x + Vite 6.x</Descriptions.Item>
          <Descriptions.Item label="后端框架">ASP.NET Core 10.0</Descriptions.Item>
          <Descriptions.Item label="UI 库">Ant Design Vue 4.x</Descriptions.Item>
          <Descriptions.Item label="数据库">MySQL + EF Core 10</Descriptions.Item>
          <Descriptions.Item label="状态管理">Pinia 3.x</Descriptions.Item>
          <Descriptions.Item label="认证">JWT Bearer Token</Descriptions.Item>
        </Descriptions>
      </Card>

      <Card title="功能模块">
        <Space direction="vertical" :size="12">
          <div v-for="item in features" :key="item.title">
            <Text strong>{{ item.title }}</Text>
            <Text type="secondary"> - {{ item.desc }}</Text>
          </div>
        </Space>
      </Card>

      <Card title="导航模块">
        <Space wrap :size="[8, 8]">
          <router-link v-for="item in modules" :key="item.path" :to="item.path">
            {{ item.name }}
          </router-link>
        </Space>
      </Card>

      <Card title="项目结构">
        <Text type="secondary">
          <pre class="text-xs">MyWebSite/
├── vue-vben-admin/          # 前端项目
│   ├── apps/web-antd/       # 主应用
│   └── apps/backend-mock/   # Mock 服务
└── WebApplication1/         # 后端项目
    ├── Features/             # 功能模块
    │   ├── Auth/           # 认证
    │   ├── DailyPlans/     # 每日计划
    │   ├── Menu/           # 菜单
    │   └── Work/           # 工作中心
    └── Shared/              # 共享模块
        ├── Common/         # 工具类
        ├── Data/          # 数据层
        └── Enums/          # 枚举</pre>
        </Text>
      </Card>

      <Card title="关于">
        <Paragraph>
          这是一个持续迭代的个人管理系统，旨在打造高效的 Personal OS。
        </Paragraph>
        <Text type="secondary">版本: 1.0.0 | 开发中功能会陆续上线</Text>
      </Card>
    </Space>
  </div>
</template>
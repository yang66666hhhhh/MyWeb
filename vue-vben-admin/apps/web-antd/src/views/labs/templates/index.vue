<script lang="ts" setup>
import { computed, ref } from 'vue';
import { Page } from '@vben/common-ui';
import { Button, Card, Col, Empty, Input, Row, Tag } from 'ant-design-vue';

const searchText = ref('');

const templates = ref([
  { id: '1', name: '周报模板', category: '工作', description: '标准周报格式', tags: ['周报', '工作'] },
  { id: '2', name: '学习计划模板', category: '学习', description: '制定学习计划的模板', tags: ['学习', '计划'] },
  { id: '3', name: '健身记录模板', category: '健康', description: '记录每日健身数据', tags: ['健身', '健康'] },
  { id: '4', name: '读书笔记模板', category: '阅读', description: '记录读书心得', tags: ['读书', '笔记'] },
  { id: '5', name: '项目管理模板', category: '工作', description: '项目进度跟踪', tags: ['项目', '管理'] },
  { id: '6', name: '日记模板', category: '个人', description: '每日日记记录', tags: ['日记', '个人'] },
]);

const filteredTemplates = computed(() => {
  if (!searchText.value) return templates.value;
  return templates.value.filter(t => 
    t.name.includes(searchText.value) || 
    t.description.includes(searchText.value) ||
    t.tags.some(tag => tag.includes(searchText.value))
  );
});
</script>

<template>
  <Page title="模板市场" description="浏览和使用各种模板">
    <div class="space-y-4">
      <Card>
        <Input.Search v-model:value="searchText" placeholder="搜索模板..." style="max-width: 400px" />
      </Card>

      <Row :gutter="[16, 16]">
        <Col v-for="template in filteredTemplates" :key="template.id" :xs="24" :sm="12" :md="8" :lg="6">
          <Card hoverable>
            <template #cover>
              <div class="flex h-32 items-center justify-center bg-gradient-to-br from-blue-100 to-purple-100">
                <span class="text-4xl"> </span>
              </div>
            </template>
            <Card.Meta :title="template.name" :description="template.description" />
            <div class="mt-3">
              <Tag v-for="tag in template.tags" :key="tag">{{ tag }}</Tag>
            </div>
            <div class="mt-3">
              <Button type="primary" size="small" block>使用模板</Button>
            </div>
          </Card>
        </Col>
      </Row>

      <Empty v-if="filteredTemplates.length === 0" description="未找到匹配的模板" />
    </div>
  </Page>
</template>
<script lang="ts" setup>
import { Button, Card, Col, Result, Row, Statistic } from 'ant-design-vue';

defineProps<{
  result: {
    success: boolean;
    successRows: number;
    failedRows: number;
    skippedRows: number;
    duplicateRows: number;
  };
}>();

const emit = defineEmits<{
  reset: [];
}>();
</script>

<template>
  <Result
    :status="result.success ? 'success' : 'error'"
    :title="result.success ? '导入完成' : '导入失败'"
    :sub-title="result.success ? '数据已成功导入到数据库' : '请检查错误后重新导入'"
  >
    <template #extra>
      <Card v-if="result.success" class="w-full max-w-xl">
        <Row :gutter="16">
          <Col :span="6">
            <Statistic title="成功" :value="result.successRows" :value-style="{ color: '#52c41a' }" />
          </Col>
          <Col :span="6">
            <Statistic title="失败" :value="result.failedRows" :value-style="{ color: '#ff4d4f' }" />
          </Col>
          <Col :span="6">
            <Statistic title="跳过" :value="result.skippedRows" :value-style="{ color: '#faad14' }" />
          </Col>
          <Col :span="6">
            <Statistic title="重复" :value="result.duplicateRows" :value-style="{ color: '#8c8c8c' }" />
          </Col>
        </Row>
      </Card>
      <Space class="mt-4">
        <Button type="primary" @click="emit('reset')">
          继续导入
        </Button>
        <Button @click="emit('reset')">
          返回列表
        </Button>
      </Space>
    </template>
  </Result>
</template>

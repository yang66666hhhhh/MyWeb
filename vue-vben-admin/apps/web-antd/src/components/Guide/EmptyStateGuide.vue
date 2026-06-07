<script setup lang="ts">
import { computed } from 'vue';

import { Button, Empty } from 'ant-design-vue';
import { PlusOutlined } from '@ant-design/icons-vue';

import { EMPTY_STATE_CONFIGS } from '#/config/guide';

interface Props {
  type: string;
  showPreview?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showPreview: false,
});

const emit = defineEmits<{
  action: [];
}>();

const config = computed(() => EMPTY_STATE_CONFIGS[props.type]);

const handleAction = () => {
  emit('action');
};
</script>

<template>
  <div v-if="config" class="empty-state-guide">
    <div class="empty-state-content">
      <div class="empty-state-icon">{{ config.icon }}</div>
      <h3 class="empty-state-title">{{ config.title }}</h3>
      <p class="empty-state-desc">{{ config.description }}</p>
      <Button type="primary" size="large" @click="handleAction">
        <template #icon><PlusOutlined /></template>
        {{ config.actionText }}
      </Button>
    </div>

    <div v-if="showPreview" class="empty-state-preview">
      <div class="preview-label">示例预览</div>
      <div class="preview-card">
        <slot name="preview">
          <div class="preview-placeholder">
            <div class="preview-line long" />
            <div class="preview-line medium" />
            <div class="preview-line short" />
          </div>
        </slot>
      </div>
    </div>
  </div>

  <Empty v-else :description="`暂无${type}数据`">
    <Button type="primary" @click="handleAction">
      <template #icon><PlusOutlined /></template>
      创建
    </Button>
  </Empty>
</template>

<style scoped>
.empty-state-guide {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 24px;
  text-align: center;
}

.empty-state-content {
  max-width: 400px;
}

.empty-state-icon {
  font-size: 64px;
  margin-bottom: 24px;
  line-height: 1;
}

.empty-state-title {
  font-size: 20px;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0 0 12px;
}

.empty-state-desc {
  font-size: 14px;
  color: #666;
  line-height: 1.6;
  margin: 0 0 32px;
}

.empty-state-preview {
  margin-top: 48px;
  width: 100%;
  max-width: 360px;
}

.preview-label {
  font-size: 12px;
  color: #999;
  margin-bottom: 12px;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.preview-card {
  background: #f8f9fa;
  border: 1px dashed #d9d9d9;
  border-radius: 8px;
  padding: 20px;
}

.preview-placeholder {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.preview-line {
  height: 12px;
  background: #e8e8e8;
  border-radius: 6px;
}

.preview-line.long {
  width: 100%;
}

.preview-line.medium {
  width: 75%;
}

.preview-line.short {
  width: 50%;
}
</style>

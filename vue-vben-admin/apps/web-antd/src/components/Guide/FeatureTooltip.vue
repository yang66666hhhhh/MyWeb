<script setup lang="ts">
import { ref } from 'vue';

import { Tooltip } from 'ant-design-vue';
import { InfoCircleOutlined } from '@ant-design/icons-vue';

import { type FeatureTooltipConfig } from '#/config/guide';
import { useGuide } from '#/composables/useGuide';

interface Props {
  config: FeatureTooltipConfig;
  showIcon?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showIcon: true,
});

const { isFeatureDismissed, dismissFeature } = useGuide();
const visible = ref(!isFeatureDismissed(props.config.id));

const handleDismiss = (e: Event) => {
  e.stopPropagation();
  dismissFeature(props.config.id);
  visible.value = false;
};
</script>

<template>
  <Tooltip
    :open="visible"
    :placement="config.placement || 'top'"
    :title="null"
    :overlay-inner-style="{ padding: '12px 16px', maxWidth: '280px' }"
  >
    <template #title>
      <div class="feature-tooltip-content">
        <div class="feature-tooltip-header">
          <span class="feature-tooltip-title">{{ config.title }}</span>
          <button class="feature-tooltip-close" @click="handleDismiss">不再显示</button>
        </div>
        <p class="feature-tooltip-desc">{{ config.description }}</p>
      </div>
    </template>
    <slot>
      <InfoCircleOutlined v-if="showIcon" class="feature-tooltip-icon" />
    </slot>
  </Tooltip>
</template>

<style scoped>
.feature-tooltip-content {
  min-width: 200px;
}

.feature-tooltip-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.feature-tooltip-title {
  font-size: 14px;
  font-weight: 600;
  color: #fff;
}

.feature-tooltip-close {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.65);
  background: none;
  border: none;
  cursor: pointer;
  padding: 2px 6px;
  border-radius: 4px;
  transition: all 0.2s;
}

.feature-tooltip-close:hover {
  background: rgba(255, 255, 255, 0.15);
  color: #fff;
}

.feature-tooltip-desc {
  font-size: 13px;
  color: rgba(255, 255, 255, 0.85);
  line-height: 1.5;
  margin: 0;
}

.feature-tooltip-icon {
  color: #1890ff;
  cursor: pointer;
  transition: color 0.2s;
}

.feature-tooltip-icon:hover {
  color: #40a9ff;
}
</style>

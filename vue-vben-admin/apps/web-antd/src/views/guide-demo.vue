<script setup lang="ts">
import { TourGuide, FeatureTooltip, EmptyStateGuide } from '#/components/Guide';
import { useGuide } from '#/composables/useGuide';
import { FEATURE_TOOLTIPS } from '#/config/guide';

const { resetTour } = useGuide();

const handleTourComplete = () => {
  console.log('引导完成');
};

const handleTourSkip = () => {
  console.log('引导跳过');
};

const handleGoalAction = () => {
  console.log('创建目标');
};

const tooltipConfigs = FEATURE_TOOLTIPS.slice(0, 3);
</script>

<template>
  <div class="guide-demo">
    <h2>用户引导系统演示</h2>

    <div class="demo-section">
      <h3>1. 新手引导</h3>
      <p>首次访问时自动触发，或点击下方按钮手动触发</p>
      <button @click="resetTour">重置引导状态</button>
      <TourGuide
        page-key="dashboard"
        :auto-start="true"
        @complete="handleTourComplete"
        @skip="handleTourSkip"
      />
    </div>

    <div class="demo-section">
      <h3>2. 功能介绍 Tooltip</h3>
      <p>鼠标悬停在图标上查看功能说明，可选择"不再显示"</p>
      <div class="tooltip-demo">
        <FeatureTooltip v-for="config in tooltipConfigs" :key="config.id" :config="config" />
      </div>
    </div>

    <div class="demo-section">
      <h3>3. 空状态引导</h3>
      <p>列表为空时显示引导界面</p>
      <EmptyStateGuide type="goals" @action="handleGoalAction" />
    </div>
  </div>
</template>

<style scoped>
.guide-demo {
  padding: 24px;
}

.demo-section {
  margin-bottom: 32px;
  padding: 24px;
  background: #f8f9fa;
  border-radius: 8px;
}

.demo-section h3 {
  margin: 0 0 12px;
  font-size: 16px;
  font-weight: 600;
}

.demo-section p {
  margin: 0 0 16px;
  color: #666;
}

.tooltip-demo {
  display: flex;
  gap: 24px;
}
</style>

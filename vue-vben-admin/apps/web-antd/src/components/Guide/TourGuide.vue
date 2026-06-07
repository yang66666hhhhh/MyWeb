<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue';

import { driver, type Driver, type DriveStep } from 'driver.js';
import 'driver.js/dist/driver.css';

import { type GuideStep, TOUR_STEPS } from '#/config/guide';
import { useGuide } from '#/composables/useGuide';

interface Props {
  pageKey: string;
  autoStart?: boolean;
  steps?: GuideStep[];
}

const props = withDefaults(defineProps<Props>(), {
  autoStart: false,
  steps: undefined,
});

const emit = defineEmits<{
  complete: [];
  skip: [];
}>();

const { markTourCompleted, markTourSkipped, shouldShowTour } = useGuide();

const driverInstance = ref<Driver | null>(null);

const getSteps = (): DriveStep[] => {
  const steps = props.steps || TOUR_STEPS[props.pageKey] || [];
  return steps.map((step) => ({
    element: step.element,
    popover: {
      title: step.popover.title,
      description: step.popover.description,
      side: step.popover.side || 'bottom',
      align: step.popover.align || 'start',
    },
  }));
};

const startTour = () => {
  if (!driverInstance.value) return;

  const steps = getSteps();
  if (steps.length === 0) return;

  driverInstance.value.setSteps(steps);
  driverInstance.value.drive();
};

const handleComplete = () => {
  markTourCompleted();
  emit('complete');
};

const handleSkip = () => {
  markTourSkipped();
  emit('skip');
};

onMounted(() => {
  driverInstance.value = driver({
    showProgress: true,
    showButtons: ['next', 'previous', 'close'],
    nextBtnText: '下一步',
    prevBtnText: '上一步',
    doneBtnText: '完成',
    progressText: '{{current}} / {{total}}',
    allowClose: true,
    stagePadding: 8,
    stageRadius: 4,
    popoverClass: 'guide-popover',
    onDestroyStarted: () => {
      handleSkip();
      driverInstance.value?.destroy();
    },
    onDeselected: () => {},
    onNextClick: () => {
      const currentStep = driverInstance.value?.getActiveIndex() || 0;
      const totalSteps = getSteps().length;
      if (currentStep >= totalSteps - 1) {
        handleComplete();
        driverInstance.value?.destroy();
      } else {
        driverInstance.value?.moveNext();
      }
    },
    onPrevClick: () => {
      driverInstance.value?.movePrevious();
    },
  });

  if (props.autoStart && shouldShowTour()) {
    setTimeout(startTour, 500);
  }
});

onUnmounted(() => {
  driverInstance.value?.destroy();
});

defineExpose({ startTour });
</script>

<template>
  <div class="tour-guide" />
</template>

<style>
.guide-popover {
  --guide-popover-bg: #fff;
  --guide-popover-border: #e8e8e8;
  --guide-popover-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  --guide-popover-radius: 8px;
  --guide-btn-primary: #1890ff;
  --guide-btn-primary-hover: #40a9ff;
  --guide-btn-text: #666;
}

.driver-popover.guide-popover {
  background: var(--guide-popover-bg);
  border: 1px solid var(--guide-popover-border);
  border-radius: var(--guide-popover-radius);
  box-shadow: var(--guide-popover-shadow);
  padding: 16px;
}

.driver-popover.guide-popover .driver-popover-title {
  font-size: 16px;
  font-weight: 600;
  color: #1a1a1a;
  margin-bottom: 8px;
}

.driver-popover.guide-popover .driver-popover-description {
  font-size: 14px;
  color: #666;
  line-height: 1.5;
  margin-bottom: 16px;
}

.driver-popover.guide-popover .driver-popover-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 12px;
  border-top: 1px solid #f0f0f0;
}

.driver-popover.guide-popover .driver-popover-progress-text {
  font-size: 12px;
  color: #999;
}

.driver-popover.guide-popover .driver-popover-navigation-btns button {
  padding: 6px 16px;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-next-btn,
.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-done-btn {
  background: var(--guide-btn-primary);
  color: white;
  border: none;
}

.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-next-btn:hover,
.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-done-btn:hover {
  background: var(--guide-btn-primary-hover);
}

.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-prev-btn {
  background: transparent;
  color: var(--guide-btn-text);
  border: 1px solid #d9d9d9;
}

.driver-popover.guide-popover .driver-popover-navigation-btns button.driver-popover-prev-btn:hover {
  color: var(--guide-btn-primary);
  border-color: var(--guide-btn-primary);
}

.driver-popover.guide-popover .driver-popover-close-btn {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  color: #999;
  cursor: pointer;
  font-size: 18px;
  line-height: 1;
  padding: 0;
}

.driver-popover.guide-popover .driver-popover-close-btn:hover {
  color: #666;
}
</style>

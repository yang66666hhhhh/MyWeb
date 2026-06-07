<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { LearningTimeTrend } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: LearningTimeTrend[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed<Record<string, any>>(() => ({
  grid: {
    bottom: '3%',
    containLabel: true,
    left: '3%',
    right: '4%',
    top: '10%',
  },
  series: [
    {
      areaStyle: { color: 'rgba(64,158,255,0.15)' },
      data: props.data.map((item) => item.hours),
      itemStyle: { color: '#409eff' },
      name: '学习时长',
      smooth: true,
      type: 'line' as const,
    },
  ],
  tooltip: { trigger: 'axis' as const },
  xAxis: {
    axisTick: { show: false },
    data: props.data.map((item) => item.date),
    type: 'category' as const,
  },
  yAxis: {
    axisTick: { show: false },
    name: '小时',
    type: 'value' as const,
  },
}));

renderEcharts(option.value);
</script>

<template>
  <div class="h-72">
    <EchartsUI v-if="!loading && data.length > 0" ref="chartRef" />
    <div v-else class="flex h-full items-center justify-center text-gray-400">
      暂无数据
    </div>
  </div>
</template>

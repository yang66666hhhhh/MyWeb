<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { WeeklyTrend } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: WeeklyTrend[];
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
  legend: { show: true },
  series: [
    {
      areaStyle: { color: 'rgba(24,144,255,0.15)' },
      data: props.data.map((item) => item.workHours),
      itemStyle: { color: '#1890ff' },
      name: '工作',
      smooth: true,
      stack: 'total',
      type: 'line' as const,
    },
    {
      areaStyle: { color: 'rgba(250,140,22,0.15)' },
      data: props.data.map((item) => item.learningHours),
      itemStyle: { color: '#fa8c16' },
      name: '学习',
      smooth: true,
      stack: 'total',
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

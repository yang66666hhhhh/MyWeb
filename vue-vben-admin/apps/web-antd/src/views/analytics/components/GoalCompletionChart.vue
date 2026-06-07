<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { GoalCompletion } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: GoalCompletion[];
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
      barWidth: '40%',
      data: props.data.map((item) => item.completed),
      itemStyle: { borderRadius: [4, 4, 0, 0] },
      name: '已完成',
      stack: 'total',
      type: 'bar' as const,
    },
    {
      barWidth: '40%',
      data: props.data.map((item) => item.total - item.completed),
      itemStyle: { borderRadius: [4, 4, 0, 0], color: '#e8e8e8' },
      name: '未完成',
      stack: 'total',
      type: 'bar' as const,
    },
  ],
  tooltip: { trigger: 'axis' as const },
  xAxis: {
    axisTick: { show: false },
    data: props.data.map((item) => item.goal),
    type: 'category' as const,
  },
  yAxis: {
    axisTick: { show: false },
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

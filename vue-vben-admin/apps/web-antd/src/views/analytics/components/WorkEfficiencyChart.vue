<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: Array<{ date: string; hours: number; logCount: number }>;
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
      data: props.data.map((item) => item.hours),
      itemStyle: { color: '#1890ff' },
      name: '工时',
      smooth: true,
      type: 'line' as const,
      yAxisIndex: 0,
    },
    {
      barWidth: '30%',
      data: props.data.map((item) => item.logCount),
      itemStyle: { borderRadius: [4, 4, 0, 0], color: '#52c41a' },
      name: '日志数',
      type: 'bar' as const,
      yAxisIndex: 1,
    },
  ],
  tooltip: { trigger: 'axis' as const },
  xAxis: {
    axisTick: { show: false },
    data: props.data.map((item) => item.date),
    type: 'category' as const,
  },
  yAxis: [
    { axisTick: { show: false }, name: '工时(h)', type: 'value' as const },
    { axisTick: { show: false }, name: '日志数', type: 'value' as const, position: 'right' as const },
  ],
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

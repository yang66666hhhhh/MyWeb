<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { MonthlyFinanceTrend } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: MonthlyFinanceTrend[];
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
      areaStyle: { color: 'rgba(82,196,26,0.15)' },
      data: props.data.map((item) => item.income),
      itemStyle: { color: '#52c41a' },
      name: '收入',
      smooth: true,
      type: 'line' as const,
    },
    {
      areaStyle: { color: 'rgba(245,34,45,0.15)' },
      data: props.data.map((item) => item.expense),
      itemStyle: { color: '#f5222d' },
      name: '支出',
      smooth: true,
      type: 'line' as const,
    },
  ],
  tooltip: {
    formatter(params: any) {
      return params
        .map(
          (p: any) =>
            `${p.seriesName}: ¥${p.value.toLocaleString()}`,
        )
        .join('<br/>');
    },
    trigger: 'axis' as const,
  },
  xAxis: {
    axisTick: { show: false },
    data: props.data.map((item) => item.month),
    type: 'category' as const,
  },
  yAxis: {
    axisTick: { show: false },
    name: '金额(¥)',
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

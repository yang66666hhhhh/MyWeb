<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { HabitHeatmapData } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: HabitHeatmapData[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

function getVirtualData(): [string, number][] {
  const dates = new Set(props.data.map((item) => item.date));
  const dateMap = new Map(props.data.map((item) => [item.date, item.count]));
  const result: [string, number][] = [];

  if (dates.size === 0) return result;

  const sorted = [...dates].sort();
  const start = new Date(sorted[0]!);
  const end = new Date(sorted[sorted.length - 1]!);

  const current = new Date(start);
  while (current <= end) {
    const dateStr = current.toISOString().slice(0, 10);
    result.push([dateStr, dateMap.get(dateStr) || 0]);
    current.setDate(current.getDate() + 1);
  }
  return result;
}

const option = computed<Record<string, any>>(() => {
  const data = getVirtualData();
  return {
    calendar: {
      cellSize: ['auto', 13],
      itemStyle: { borderWidth: 2 },
      range: data.length > 0 ? [data[0]![0], data[data.length - 1]![0]] : undefined,
      splitLine: { show: true },
      yearLabel: { show: true },
    },
    series: [
      {
        calendarIndex: 0,
        coordinateSystem: 'calendar',
        data,
        itemStyle: { color: '#52c41a', borderRadius: 2 },
        type: 'heatmap' as const,
      },
    ],
    tooltip: {
      formatter(params: any) {
        return `${params.value[0]}: ${params.value[1]} 次打卡`;
      },
    },
    visualMap: {
      calculable: true,
      inRange: { color: ['#ebedf0', '#c6e48b', '#7bc96f', '#239a3b', '#196127'] },
      max: Math.max(...data.map((d) => d[1]), 5),
      min: 0,
      orient: 'horizontal',
      show: false,
    },
  };
});

renderEcharts(option.value);
</script>

<template>
  <div class="h-40">
    <EchartsUI v-if="!loading && data.length > 0" ref="chartRef" />
    <div v-else class="flex h-full items-center justify-center text-gray-400">
      暂无数据
    </div>
  </div>
</template>

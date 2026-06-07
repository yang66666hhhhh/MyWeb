<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { HourlyDistribution } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: HourlyDistribution[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed<Record<string, any>>(() => {
  const categoryMap = new Map<string, number>();
  for (const item of props.data) {
    categoryMap.set(item.category, (categoryMap.get(item.category) || 0) + item.value);
  }

  return {
    tooltip: {
      formatter(params: any) {
        return `${params.name}: ${params.value}分钟 (${params.percent}%)`;
      },
      trigger: 'item' as const,
    },
    legend: {
      bottom: '0%',
      left: 'center',
    },
    series: [
      {
        data: Array.from(categoryMap.entries()).map(([name, value]) => ({
          name,
          value,
        })),
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)',
          },
        },
        itemStyle: { borderRadius: 10, borderWidth: 2 },
        label: {
          formatter: '{b}: {d}%',
          show: true,
        },
        name: '时间分配',
        radius: ['40%', '70%'],
        type: 'pie' as const,
      },
    ],
  };
});

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

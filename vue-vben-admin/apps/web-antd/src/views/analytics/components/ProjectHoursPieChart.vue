<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: Array<{ projectName: string; totalHours: number; percentage: number }>;
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed<Record<string, any>>(() => ({
  tooltip: {
    formatter(params: any) {
      return `${params.name}: ${params.value.toFixed(1)}h (${params.percent}%)`;
    },
    trigger: 'item' as const,
  },
  legend: {
    bottom: '0%',
    left: 'center',
  },
  series: [
    {
      data: props.data.map((item) => ({
        name: item.projectName,
        value: item.totalHours,
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
      name: '项目工时',
      radius: ['40%', '70%'],
      type: 'pie' as const,
    },
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

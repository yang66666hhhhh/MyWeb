<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

import type { TaskDistribution } from '#/api/analytics';

interface Props {
  data: TaskDistribution[];
  loading?: boolean;
}

const props = defineProps<Props>();

const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const chartData = computed(() => {
  return props.data.map((item) => ({
    name: item.type,
    value: item.count,
  }));
});

const option = computed(() => ({
  tooltip: {
    trigger: 'item',
  },
  legend: {
    bottom: '0%',
    left: 'center',
  },
  series: [
    {
      data: chartData.value,
      emphasis: {
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)',
        },
      },
      itemStyle: {
        borderRadius: 10,
        borderWidth: 2,
      },
      label: {
        formatter: '{b}: {c} ({d}%)',
        show: true,
      },
      name: '任务类型',
      radius: ['40%', '70%'],
      type: 'pie',
    },
  ],
}));

renderEcharts(option.value);
</script>

<template>
  <div class="h-72">
    <EchartsUI v-if="!loading && data.length > 0" ref="chartRef" />
    <div v-else class="flex items-center justify-center h-full text-gray-400">
      暂无数据
    </div>
  </div>
</template>
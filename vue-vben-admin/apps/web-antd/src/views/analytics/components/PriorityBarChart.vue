<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';
import { computed, ref } from 'vue';
import { EchartsUI, useEcharts } from '@vben/plugins/echarts';
import type { TaskPriorityDistribution } from '#/api/analytics';

interface Props {
  data: TaskPriorityDistribution[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed(() => {
  const sortedData = [...props.data].sort((a, b) => {
    const order: Record<string, number> = { Low: 1, Medium: 2, High: 3, Urgent: 4 };
    return (order[a.priority] || 0) - (order[b.priority] || 0);
  });

  return {
    color: ['#52c41a', '#1890ff', '#fa8c16', '#f5222d'],
    grid: {
      bottom: '3%',
      containLabel: true,
      left: '3%',
      right: '3%',
      top: '3%',
    },
    series: [
      {
        barWidth: '50%',
        data: sortedData.map((item) => item.count),
        itemStyle: { borderRadius: [4, 4, 0, 0] },
        label: { show: true, position: 'top' },
        name: '任务数',
        type: 'bar',
      },
    ],
    tooltip: { trigger: 'axis' },
    xAxis: {
      axisTick: { show: false },
      data: sortedData.map((item) => item.priority),
      type: 'category',
    },
    yAxis: {
      axisTick: { show: false },
      type: 'value',
    },
  };
});

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
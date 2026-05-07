<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';
import { computed, ref } from 'vue';
import { EchartsUI, useEcharts } from '@vben/plugins/echarts';
import type { TaskTrend } from '#/api/analytics';

interface Props {
  data: TaskTrend[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed(() => ({
  grid: {
    bottom: '3%',
    containLabel: true,
    left: '3%',
    right: '4%',
    top: '3%',
  },
  legend: { show: true },
  series: [
    {
      areaStyle: {},
      data: props.data.map((item) => item.created),
      itemStyle: { color: '#5ab1ef' },
      name: '创建',
      smooth: true,
      type: 'line',
    },
    {
      areaStyle: {},
      data: props.data.map((item) => item.completed),
      itemStyle: { color: '#019680' },
      name: '完成',
      smooth: true,
      type: 'line',
    },
  ],
  tooltip: { trigger: 'axis' },
  xAxis: {
    axisTick: { show: false },
    data: props.data.map((item) => item.date),
    type: 'category',
  },
  yAxis: [
    {
      axisTick: { show: false },
      type: 'value',
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
<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import type { SkillMastery } from '#/api/analytics/extended';

import { computed, ref } from 'vue';

import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

interface Props {
  data: SkillMastery[];
  loading?: boolean;
}

const props = defineProps<Props>();
const chartRef = ref<EchartsUIType>();
const { renderEcharts } = useEcharts(chartRef);

const option = computed<Record<string, any>>(() => ({
  radar: {
    indicator: props.data.map((item) => ({
      name: item.skill,
      max: 100,
    })),
  },
  series: [
    {
      areaStyle: { opacity: 0.2 },
      data: [
        {
          value: props.data.map((item) => item.value),
          name: '技能掌握度',
        },
      ],
      label: { show: true, formatter: '{c}%' },
      type: 'radar' as const,
    },
  ],
  tooltip: { trigger: 'item' as const },
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

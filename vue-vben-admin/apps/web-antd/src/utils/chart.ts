import { formatDate, formatDateTime } from './date';

export interface ChartDataItem {
  xAxis: string;
  value: number;
  [key: string]: any;
}

export interface ChartSeries {
  name: string;
  data: number[];
  color?: string;
}

export function formatChartData(data: any[], xKey: string, yKey: string): ChartDataItem[] {
  return data.map((item) => ({
    xAxis: item[xKey] || '',
    value: Number(item[yKey]) || 0,
    ...item,
  }));
}

export function getChartColors(): string[] {
  return [
    '#5470c6',
    '#91cc75',
    '#fac858',
    '#ee6666',
    '#73c0de',
    '#3ba272',
    '#fc8452',
    '#9a60b4',
    '#ea7ccc',
  ];
}

export function getDefaultChartOption() {
  return {
    tooltip: {
      trigger: 'axis',
    },
    legend: {
      data: [],
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true,
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: [],
    },
    yAxis: {
      type: 'value',
    },
  };
}

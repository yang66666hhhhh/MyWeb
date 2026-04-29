import dayjs from 'dayjs';
import { DATE_FORMAT, DATETIME_FORMAT, TIME_FORMAT } from '../constants/app';

export function formatDate(date: string | Date | number, format = DATE_FORMAT): string {
  if (!date) return '';
  return dayjs(date).format(format);
}

export function formatDateTime(date: string | Date | number, format = DATETIME_FORMAT): string {
  if (!date) return '';
  return dayjs(date).format(format);
}

export function formatTime(date: string | Date | number, format = TIME_FORMAT): string {
  if (!date) return '';
  return dayjs(date).format(format);
}

export function getWeekDay(date: string | Date | number): string {
  const weekDays = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'];
  return weekDays[dayjs(date).day()];
}

export function getStartOfDay(date: string | Date | number): Date {
  return dayjs(date).startOf('day').toDate();
}

export function getEndOfDay(date: string | Date | number): Date {
  return dayjs(date).endOf('day').toDate();
}

export function addDays(date: string | Date | number, days: number): Date {
  return dayjs(date).add(days, 'day').toDate();
}

export function diffDays(date1: string | Date, date2: string | Date): number {
  return dayjs(date1).diff(dayjs(date2), 'day');
}

export function isToday(date: string | Date | number): boolean {
  return dayjs(date).isSame(dayjs(), 'day');
}

export function isSameDay(date1: string | Date | number, date2: string | Date | number): boolean {
  return dayjs(date1).isSame(dayjs(date2), 'day');
}

export { dayjs };

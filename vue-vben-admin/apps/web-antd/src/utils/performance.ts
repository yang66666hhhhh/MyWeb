interface PerformanceMetric {
  name: string;
  startTime: number;
  endTime?: number;
  duration?: number;
  metadata?: Record<string, any>;
}

class PerformanceMonitor {
  private metrics: PerformanceMetric[] = [];
  private maxMetrics = 100;

  start(name: string, metadata?: Record<string, any>): void {
    const metric: PerformanceMetric = {
      name,
      startTime: performance.now(),
      metadata,
    };
    this.metrics.push(metric);

    // 限制存储数量
    if (this.metrics.length > this.maxMetrics) {
      this.metrics.shift();
    }
  }

  end(name: string): number | null {
    const metric = [...this.metrics].reverse().find((m) => m.name === name && !m.endTime);
    if (!metric) return null;

    metric.endTime = performance.now();
    metric.duration = metric.endTime - metric.startTime;

    // 慢操作警告
    if (metric.duration > 3000) {
      console.warn(`Slow operation detected: ${name} took ${metric.duration.toFixed(2)}ms`);
    }

    return metric.duration;
  }

  measure<T>(name: string, fn: () => T, metadata?: Record<string, any>): T {
    this.start(name, metadata);
    try {
      const result = fn();
      if (result instanceof Promise) {
        return result.finally(() => this.end(name)) as T;
      }
      this.end(name);
      return result;
    } catch (error) {
      this.end(name);
      throw error;
    }
  }

  async measureAsync<T>(name: string, fn: () => Promise<T>, metadata?: Record<string, any>): Promise<T> {
    this.start(name, metadata);
    try {
      return await fn();
    } finally {
      this.end(name);
    }
  }

  getMetrics(): PerformanceMetric[] {
    return [...this.metrics];
  }

  getAverageDuration(name: string): number {
    const namedMetrics = this.metrics.filter((m) => m.name === name && m.duration);
    if (namedMetrics.length === 0) return 0;
    const total = namedMetrics.reduce((sum, m) => sum + (m.duration || 0), 0);
    return total / namedMetrics.length;
  }

  clear(): void {
    this.metrics = [];
  }

  report(): void {
    const grouped = new Map<string, PerformanceMetric[]>();
    for (const metric of this.metrics) {
      if (!metric.duration) continue;
      const existing = grouped.get(metric.name) || [];
      existing.push(metric);
      grouped.set(metric.name, existing);
    }

    console.group('Performance Report');
    for (const [name, metrics] of grouped) {
      const durations = metrics.map((m) => m.duration!);
      const avg = durations.reduce((a, b) => a + b, 0) / durations.length;
      const max = Math.max(...durations);
      const min = Math.min(...durations);
      console.log(`${name}: avg=${avg.toFixed(2)}ms, min=${min.toFixed(2)}ms, max=${max.toFixed(2)}ms, count=${metrics.length}`);
    }
    console.groupEnd();
  }
}

export const perfMonitor = new PerformanceMonitor();

export function measurePerformance<T>(name: string, fn: () => T): T {
  return perfMonitor.measure(name, fn);
}

export async function measureAsyncPerformance<T>(name: string, fn: () => Promise<T>): Promise<T> {
  return perfMonitor.measureAsync(name, fn);
}

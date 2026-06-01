interface ErrorReport {
  message: string;
  stack?: string;
  component?: string;
  url?: string;
  userId?: string;
  timestamp: number;
  metadata?: Record<string, any>;
}

class ErrorReporter {
  private queue: ErrorReport[] = [];
  private maxQueueSize = 50;
  private flushInterval = 30000; // 30 seconds
  private timer: ReturnType<typeof setInterval> | null = null;

  constructor() {
    this.startAutoFlush();
  }

  captureError(error: Error, component?: string, metadata?: Record<string, any>): void {
    const report: ErrorReport = {
      message: error.message,
      stack: error.stack,
      component,
      url: globalThis.location?.href,
      timestamp: Date.now(),
      metadata,
    };

    this.queue.push(report);

    if (this.queue.length > this.maxQueueSize) {
      this.queue.shift();
    }

    // 严重错误立即上报
    if (this.isCriticalError(error)) {
      this.flush();
    }
  }

  captureMessage(message: string, level: 'error' | 'info' | 'warning' = 'info', metadata?: Record<string, any>): void {
    const report: ErrorReport = {
      message: `[${level.toUpperCase()}] ${message}`,
      url: globalThis.location?.href,
      timestamp: Date.now(),
      metadata,
    };

    this.queue.push(report);
  }

  private isCriticalError(error: Error): boolean {
    const criticalPatterns = [
      'ChunkLoadError',
      'Loading chunk',
      'Network Error',
      'out of memory',
    ];
    return criticalPatterns.some((pattern) => error.message.includes(pattern));
  }

  async flush(): Promise<void> {
    if (this.queue.length === 0) return;

    const reports = [...this.queue];
    this.queue = [];

    try {
      // 这里可以发送到后端或第三方服务
      console.group('Error Reports');
      for (const report of reports) {
        console.error(`[${new Date(report.timestamp).toISOString()}] ${report.message}`, report);
      }
      console.groupEnd();
    } catch {
      // 发送失败，重新加入队列
      this.queue.unshift(...reports);
    }
  }

  private startAutoFlush(): void {
    if (typeof window !== 'undefined') {
      this.timer = setInterval(() => this.flush(), this.flushInterval);
    }
  }

  stopAutoFlush(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  getQueue(): ErrorReport[] {
    return [...this.queue];
  }

  clearQueue(): void {
    this.queue = [];
  }
}

export const errorReporter = new ErrorReporter();

export function captureError(error: Error, component?: string, metadata?: Record<string, any>): void {
  errorReporter.captureError(error, component, metadata);
}

export function captureMessage(message: string, level: 'error' | 'info' | 'warning' = 'info', metadata?: Record<string, any>): void {
  errorReporter.captureMessage(message, level, metadata);
}

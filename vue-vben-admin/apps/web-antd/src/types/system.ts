export interface SystemSettings {
  appName: string;
  appVersion: string;
  locale: string;
  theme: 'light' | 'dark';
  primaryColor: string;
}

export interface SystemLog {
  id: string;
  level: 'debug' | 'info' | 'warn' | 'error';
  message: string;
  stack?: string;
  timestamp: string;
  userId?: string;
  ip?: string;
}

export interface SystemBackup {
  id: string;
  name: string;
  size: number;
  path: string;
  createdAt: string;
  status: 'pending' | 'running' | 'completed' | 'failed';
}

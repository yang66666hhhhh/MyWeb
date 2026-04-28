import { defineConfig } from '@vben/vite-config';

const createApiProxy = (target: string) => ({
  changeOrigin: true,
  rewrite: (path: string) => path.replace(/^\/api/, ''),
  target,
  ws: true,
});

const mockApi = createApiProxy('http://localhost:5320/api');
const dotnetApi = createApiProxy('http://localhost:5062/api');

export default defineConfig(async () => {
  return {
    application: {},
    vite: {
      server: {
        proxy: {
          '/api/auth': mockApi,
          '/api/user': mockApi,
          '/api/menu': mockApi,
          '/api/daily-plans': dotnetApi,
          '/api/habits': dotnetApi,
          '/api/work-logs': dotnetApi,
          '/api/knowledge-base': dotnetApi,
          '/api/postgraduate': dotnetApi,
          '/api/projects': dotnetApi,
          '/api/work/logs': dotnetApi,
          '/api/work/projects': dotnetApi,
          '/api/work/devices': dotnetApi,
          '/api/work/task-types': dotnetApi,
          '/api/work/import': dotnetApi,
          '/api/work/statistics': dotnetApi,
          '/api/work/daily-plans': dotnetApi,
          '/api': mockApi,
        },
      },
    },
  };
});

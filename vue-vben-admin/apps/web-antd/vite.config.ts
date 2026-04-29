import { defineConfig } from '@vben/vite-config';

const createApiProxy = (target: string) => ({
  changeOrigin: true,
  rewrite: (path: string) => path.replace(/^\/api/, ''),
  target,
  ws: true,
});

// const mockApi = createApiProxy('http://localhost:5320/api');
const dotnetApi = createApiProxy('http://localhost:5000/api');

export default defineConfig(async () => {
  return {
    application: {},
    vite: {
      server: {
        proxy: {
          // === Mock 环境 (取消注释启用) ===
          // '/api/auth': mockApi,
          // '/api/user': mockApi,
          // '/api/menu': mockApi,
          // '/api/daily-plans': mockApi,
          // '/api/habits': mockApi,
          // '/api/work-logs': mockApi,
          // '/api/knowledge-base': mockApi,
          // '/api/postgraduate': mockApi,
          // '/api/projects': mockApi,
          // '/api/work/logs': mockApi,
          // '/api/work/projects': mockApi,
          // '/api/work/devices': mockApi,
          // '/api/work/task-types': mockApi,
          // '/api/work/import': mockApi,
          // '/api/work/statistics': mockApi,
          // '/api/work/daily-plans': mockApi,
          // '/api': mockApi,

          // === 真实后端环境 ===
          '/api': dotnetApi,
        },
      },
    },
  };
});

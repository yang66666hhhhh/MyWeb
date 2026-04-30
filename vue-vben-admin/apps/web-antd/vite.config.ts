import { defineConfig } from '@vben/vite-config';

const createApiProxy = (target: string) => ({
  changeOrigin: true,
  rewrite: (path: string) => path.replace(/^\/api/, ''),
  target,
  ws: true,
});

const dotnetApi = createApiProxy('http://localhost:5062/api');

export default defineConfig(async () => {
  return {
    application: {
      nitroMock: false,
    },
    vite: {
      server: {
        proxy: {
          '/api': dotnetApi,
        },
      },
    },
  };
});

import { defineConfig } from '@vben/vite-config';

import { pwaConfig } from './src/pwa';

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
      pwa: true,
      pwaOptions: pwaConfig,
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

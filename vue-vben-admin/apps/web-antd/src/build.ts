import type { BuildOptions } from 'vite';

export const buildConfig: BuildOptions = {
  // 启用源码映射（生产环境可关闭）
  sourcemap: false,

  // 启用 CSS 代码分割
  cssCodeSplit: true,

  // 启用压缩
  minify: 'terser',

  // Terser 配置
  terserOptions: {
    compress: {
      // 移除 console
      drop_console: true,
      // 移除 debugger
      drop_debugger: true,
      // 移除无用代码
      dead_code: true,
    },
    format: {
      // 移除注释
      comments: false,
    },
  },

  // Rollup 配置
  rollupOptions: {
    output: {
      // 文件命名
      chunkFileNames: 'assets/js/[name]-[hash].js',
      entryFileNames: 'assets/js/[name]-[hash].js',
      assetFileNames: 'assets/[ext]/[name]-[hash].[ext]',
    },
  },

  // 构建目标
  target: 'es2015',

  // 块大小警告限制
  chunkSizeWarningLimit: 2000,
};

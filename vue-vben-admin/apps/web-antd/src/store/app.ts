import { defineStore } from 'pinia';

export const useAppStore = defineStore('app', () => {
  const name = 'Personal OS';
  const version = '1.0.0';

  return { name, version };
});

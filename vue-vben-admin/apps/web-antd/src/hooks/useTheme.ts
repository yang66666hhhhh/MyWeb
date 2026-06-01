import { ref } from 'vue';

type ThemeMode = 'dark' | 'light' | 'system';

const STORAGE_KEY = 'app-theme';

export function useTheme() {
  const theme = ref<ThemeMode>((localStorage.getItem(STORAGE_KEY) as ThemeMode) || 'system');
  const isDark = ref(false);

  function applyTheme(mode: ThemeMode) {
    const html = document.documentElement;

    if (mode === 'system') {
      isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches;
    } else {
      isDark.value = mode === 'dark';
    }

    html.classList.toggle('dark', isDark.value);
    html.setAttribute('data-theme', isDark.value ? 'dark' : 'light');
  }

  function setTheme(mode: ThemeMode) {
    theme.value = mode;
    localStorage.setItem(STORAGE_KEY, mode);
    applyTheme(mode);
  }

  function toggleTheme() {
    const next: Record<ThemeMode, ThemeMode> = {
      light: 'dark',
      dark: 'system',
      system: 'light',
    };
    setTheme(next[theme.value]);
  }

  // 监听系统主题变化
  if (typeof window !== 'undefined') {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    mediaQuery.addEventListener('change', () => {
      if (theme.value === 'system') {
        applyTheme('system');
      }
    });
  }

  // 初始化
  applyTheme(theme.value);

  return {
    theme,
    isDark,
    setTheme,
    toggleTheme,
  };
}

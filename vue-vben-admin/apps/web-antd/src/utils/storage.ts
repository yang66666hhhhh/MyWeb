const PREFIX = 'PGS_';

export function getStorageItem<T = any>(key: string, defaultValue?: T): T | null {
  try {
    const item = localStorage.getItem(PREFIX + key);
    if (item === null) return defaultValue ?? null;
    return JSON.parse(item) as T;
  } catch {
    return defaultValue ?? null;
  }
}

export function setStorageItem<T = any>(key: string, value: T): void {
  try {
    localStorage.setItem(PREFIX + key, JSON.stringify(value));
  } catch (error) {
    console.error('Failed to set storage item:', error);
  }
}

export function removeStorageItem(key: string): void {
  localStorage.removeItem(PREFIX + key);
}

export function clearStorage(): void {
  localStorage.clear();
}

export function getSessionItem<T = any>(key: string, defaultValue?: T): T | null {
  try {
    const item = sessionStorage.getItem(PREFIX + key);
    if (item === null) return defaultValue ?? null;
    return JSON.parse(item) as T;
  } catch {
    return defaultValue ?? null;
  }
}

export function setSessionItem<T = any>(key: string, value: T): void {
  try {
    sessionStorage.setItem(PREFIX + key, JSON.stringify(value));
  } catch (error) {
    console.error('Failed to set session item:', error);
  }
}

export function removeSessionItem(key: string): void {
  sessionStorage.removeItem(PREFIX + key);
}

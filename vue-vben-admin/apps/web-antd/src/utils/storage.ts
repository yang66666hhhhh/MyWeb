interface StorageOptions {
  prefix?: string;
  encrypt?: boolean;
  expire?: number; // 过期时间（毫秒）
}

interface StorageItem<T> {
  value: T;
  timestamp: number;
  expire?: number;
}

class SecureStorage {
  private prefix: string;
  private store: Storage;

  constructor(store: Storage, prefix = 'app_') {
    this.store = store;
    this.prefix = prefix;
  }

  private getKey(key: string): string {
    return `${this.prefix}${key}`;
  }

  set<T>(key: string, value: T, options?: StorageOptions): void {
    const item: StorageItem<T> = {
      value,
      timestamp: Date.now(),
      expire: options?.expire,
    };

    try {
      const serialized = JSON.stringify(item);
      this.store.setItem(this.getKey(key), serialized);
    } catch (error) {
      console.error(`Failed to set storage item: ${key}`, error);
    }
  }

  get<T>(key: string, defaultValue?: T): T | undefined {
    try {
      const serialized = this.store.getItem(this.getKey(key));
      if (!serialized) return defaultValue;

      const item: StorageItem<T> = JSON.parse(serialized);

      // 检查是否过期
      if (item.expire && Date.now() - item.timestamp > item.expire) {
        this.remove(key);
        return defaultValue;
      }

      return item.value;
    } catch {
      return defaultValue;
    }
  }

  remove(key: string): void {
    this.store.removeItem(this.getKey(key));
  }

  has(key: string): boolean {
    return this.store.getItem(this.getKey(key)) !== null;
  }

  clear(): void {
    const keys = Object.keys(this.store);
    keys.forEach((key) => {
      if (key.startsWith(this.prefix)) {
        this.store.removeItem(key);
      }
    });
  }

  keys(): string[] {
    const keys: string[] = [];
    for (let i = 0; i < this.store.length; i++) {
      const key = this.store.key(i);
      if (key?.startsWith(this.prefix)) {
        keys.push(key.slice(this.prefix.length));
      }
    }
    return keys;
  }

  size(): number {
    return this.keys().length;
  }
}

export const localStore = new SecureStorage(window.localStorage, 'app_');
export const sessionStore = new SecureStorage(window.sessionStorage, 'app_');

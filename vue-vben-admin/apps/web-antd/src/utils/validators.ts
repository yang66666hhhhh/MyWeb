// 验证邮箱
export function isEmail(value: string): boolean {
  return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(value);
}

// 验证手机号（中国大陆）
export function isPhone(value: string): boolean {
  return /^1[3-9]\d{9}$/.test(value);
}

// 验证URL
export function isUrl(value: string): boolean {
  try {
    new URL(value);
    return true;
  } catch {
    return false;
  }
}

// 验证身份证号（中国大陆）
export function isIdCard(value: string): boolean {
  return /^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}[\dXx]$/.test(value);
}

// 验证密码强度
export function getPasswordStrength(password: string): 'medium' | 'strong' | 'weak' {
  let score = 0;

  if (password.length >= 8) score++;
  if (password.length >= 12) score++;
  if (/[a-z]/.test(password)) score++;
  if (/[A-Z]/.test(password)) score++;
  if (/[0-9]/.test(password)) score++;
  if (/[^a-zA-Z0-9]/.test(password)) score++;

  if (score <= 2) return 'weak';
  if (score <= 4) return 'medium';
  return 'strong';
}

// 验证是否为空
export function isEmpty(value: any): boolean {
  if (value === null || value === undefined) return true;
  if (typeof value === 'string') return value.trim().length === 0;
  if (Array.isArray(value)) return value.length === 0;
  if (typeof value === 'object') return Object.keys(value).length === 0;
  return false;
}

// 验证数字范围
export function isInRange(value: number, min: number, max: number): boolean {
  return value >= min && value <= max;
}

// 验证字符串长度
export function isLengthInRange(value: string, min: number, max: number): boolean {
  return value.length >= min && value.length <= max;
}

// 验证是否为数字
export function isNumeric(value: string): boolean {
  return /^\d+$/.test(value);
}

// 验证是否为中文
export function isChinese(value: string): boolean {
  return /^[\u4e00-\u9fa5]+$/.test(value);
}

// 验证是否为字母数字
export function isAlphanumeric(value: string): boolean {
  return /^[a-zA-Z0-9]+$/.test(value);
}

// 自定义验证器
export function createValidator(rules: Array<(value: any) => string | null>) {
  return (value: any): string | null => {
    for (const rule of rules) {
      const error = rule(value);
      if (error) return error;
    }
    return null;
  };
}

// 常用验证规则
export const rules = {
  required: (message = '此项为必填项') => (value: any) => isEmpty(value) ? message : null,
  email: (message = '请输入有效的邮箱地址') => (value: string) => !isEmail(value) ? message : null,
  phone: (message = '请输入有效的手机号') => (value: string) => !isPhone(value) ? message : null,
  minLength: (min: number, message?: string) => (value: string) =>
    value.length < min ? message || `至少需要 ${min} 个字符` : null,
  maxLength: (max: number, message?: string) => (value: string) =>
    value.length > max ? message || `最多 ${max} 个字符` : null,
  pattern: (pattern: RegExp, message = '格式不正确') => (value: string) =>
    !pattern.test(value) ? message : null,
};

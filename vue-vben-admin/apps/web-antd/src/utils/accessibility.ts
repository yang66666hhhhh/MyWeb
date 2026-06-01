// 无障碍访问工具

// 生成唯一ID（用于 ARIA 属性）
let idCounter = 0;
export function generateAriaId(prefix = 'aria'): string {
  return `${prefix}-${++idCounter}`;
}

// 键盘导航支持
export function handleKeyboardNavigation(
  event: KeyboardEvent,
  options: {
    onEnter?: () => void;
    onEscape?: () => void;
    onArrowDown?: () => void;
    onArrowUp?: () => void;
    onArrowLeft?: () => void;
    onArrowRight?: () => void;
    onTab?: () => void;
    onSpace?: () => void;
  },
): void {
  switch (event.key) {
    case 'Enter': {
      options.onEnter?.();
      break;
    }
    case 'Escape': {
      options.onEscape?.();
      break;
    }
    case 'ArrowDown': {
      event.preventDefault();
      options.onArrowDown?.();
      break;
    }
    case 'ArrowUp': {
      event.preventDefault();
      options.onArrowUp?.();
      break;
    }
    case 'ArrowLeft': {
      options.onArrowLeft?.();
      break;
    }
    case 'ArrowRight': {
      options.onArrowRight?.();
      break;
    }
    case 'Tab': {
      options.onTab?.();
      break;
    }
    case ' ': {
      event.preventDefault();
      options.onSpace?.();
      break;
    }
  }
}

// 焦点管理
export function focusElement(selector: string): void {
  const element = document.querySelector(selector) as HTMLElement;
  element?.focus();
}

export function focusFirstFocusable(container: HTMLElement): void {
  const focusable = container.querySelector(
    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])',
  ) as HTMLElement;
  focusable?.focus();
}

export function trapFocus(container: HTMLElement): () => void {
  const focusableElements = container.querySelectorAll(
    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])',
  );
  const firstFocusable = focusableElements[0] as HTMLElement;
  const lastFocusable = focusableElements[focusableElements.length - 1] as HTMLElement;

  function handleTab(e: KeyboardEvent) {
    if (e.key !== 'Tab') return;

    if (e.shiftKey) {
      if (document.activeElement === firstFocusable) {
        lastFocusable.focus();
        e.preventDefault();
      }
    } else {
      if (document.activeElement === lastFocusable) {
        firstFocusable.focus();
        e.preventDefault();
      }
    }
  }

  container.addEventListener('keydown', handleTab);
  firstFocusable?.focus();

  return () => {
    container.removeEventListener('keydown', handleTab);
  };
}

// 屏幕阅读器公告
export function announce(message: string, priority: 'assertive' | 'polite' = 'polite'): void {
  const announcer = document.getElementById('aria-announcer');
  if (announcer) {
    announcer.setAttribute('aria-live', priority);
    announcer.textContent = message;
  } else {
    const newAnnouncer = document.createElement('div');
    newAnnouncer.id = 'aria-announcer';
    newAnnouncer.setAttribute('aria-live', priority);
    newAnnouncer.setAttribute('aria-atomic', 'true');
    newAnnouncer.className = 'sr-only';
    document.body.appendChild(newAnnouncer);
    newAnnouncer.textContent = message;
  }
}

// ARIA 属性生成器
export function ariaProps(options: {
  label?: string;
  labelledBy?: string;
  describedBy?: string;
  expanded?: boolean;
  hasPopup?: boolean | 'dialog' | 'grid' | 'listbox' | 'menu' | 'tree';
  hidden?: boolean;
  live?: 'assertive' | 'off' | 'polite';
  role?: string;
  selected?: boolean;
  checked?: boolean;
  disabled?: boolean;
  required?: boolean;
  invalid?: boolean;
  busy?: boolean;
  current?: boolean | 'date' | 'location' | 'page' | 'step' | 'time';
}): Record<string, any> {
  const props: Record<string, any> = {};

  if (options.label) props['aria-label'] = options.label;
  if (options.labelledBy) props['aria-labelledby'] = options.labelledBy;
  if (options.describedBy) props['aria-describedby'] = options.describedBy;
  if (options.expanded !== undefined) props['aria-expanded'] = options.expanded;
  if (options.hasPopup !== undefined) props['aria-haspopup'] = options.hasPopup;
  if (options.hidden !== undefined) props['aria-hidden'] = options.hidden;
  if (options.live) props['aria-live'] = options.live;
  if (options.role) props['role'] = options.role;
  if (options.selected !== undefined) props['aria-selected'] = options.selected;
  if (options.checked !== undefined) props['aria-checked'] = options.checked;
  if (options.disabled !== undefined) props['aria-disabled'] = options.disabled;
  if (options.required !== undefined) props['aria-required'] = options.required;
  if (options.invalid !== undefined) props['aria-invalid'] = options.invalid;
  if (options.busy !== undefined) props['aria-busy'] = options.busy;
  if (options.current !== undefined) props['aria-current'] = options.current;

  return props;
}

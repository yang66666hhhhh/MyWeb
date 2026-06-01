import { ref } from 'vue';

// 动画工具

// 淡入
export function fadeIn(element: HTMLElement, duration = 300): Promise<void> {
  return new Promise((resolve) => {
    element.style.opacity = '0';
    element.style.display = '';

    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      element.style.opacity = progress.toString();

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 淡出
export function fadeOut(element: HTMLElement, duration = 300): Promise<void> {
  return new Promise((resolve) => {
    const start = performance.now();
    const startOpacity = parseFloat(getComputedStyle(element).opacity);

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      element.style.opacity = (startOpacity * (1 - progress)).toString();

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.display = 'none';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 滑入
export function slideDown(element: HTMLElement, duration = 300): Promise<void> {
  return new Promise((resolve) => {
    element.style.display = '';
    const height = element.scrollHeight;
    element.style.height = '0px';
    element.style.overflow = 'hidden';

    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      element.style.height = `${height * progress}px`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.height = '';
        element.style.overflow = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 滑出
export function slideUp(element: HTMLElement, duration = 300): Promise<void> {
  return new Promise((resolve) => {
    const height = element.scrollHeight;
    element.style.height = `${height}px`;
    element.style.overflow = 'hidden';

    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      element.style.height = `${height * (1 - progress)}px`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.display = 'none';
        element.style.height = '';
        element.style.overflow = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 弹跳效果
export function bounce(element: HTMLElement, duration = 600): Promise<void> {
  return new Promise((resolve) => {
    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      // 弹跳曲线
      const bounceProgress = Math.abs(Math.sin(progress * Math.PI * 3)) * (1 - progress);

      element.style.transform = `translateY(${-20 * bounceProgress}px)`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.transform = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 抖动效果
export function shake(element: HTMLElement, duration = 500): Promise<void> {
  return new Promise((resolve) => {
    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      const shakeX = Math.sin(progress * Math.PI * 8) * 10 * (1 - progress);

      element.style.transform = `translateX(${shakeX}px)`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.transform = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 脉冲效果
export function pulse(element: HTMLElement, duration = 1000): Promise<void> {
  return new Promise((resolve) => {
    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      const scale = 1 + 0.1 * Math.sin(progress * Math.PI * 2);

      element.style.transform = `scale(${scale})`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.transform = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 旋转效果
export function spin(element: HTMLElement, duration = 1000): Promise<void> {
  return new Promise((resolve) => {
    const start = performance.now();

    function animate(currentTime: number) {
      const elapsed = currentTime - start;
      const progress = Math.min(elapsed / duration, 1);

      element.style.transform = `rotate(${progress * 360}deg)`;

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        element.style.transform = '';
        resolve();
      }
    }

    requestAnimationFrame(animate);
  });
}

// 组合动画
export async function sequence(
  element: HTMLElement,
  animations: Array<{ duration?: number; type: 'bounce' | 'fadeIn' | 'fadeOut' | 'pulse' | 'shake' | 'slideDown' | 'slideUp' | 'spin' }>,
): Promise<void> {
  for (const animation of animations) {
    const duration = animation.duration || 300;
    switch (animation.type) {
      case 'fadeIn': {
        await fadeIn(element, duration);
        break;
      }
      case 'fadeOut': {
        await fadeOut(element, duration);
        break;
      }
      case 'slideDown': {
        await slideDown(element, duration);
        break;
      }
      case 'slideUp': {
        await slideUp(element, duration);
        break;
      }
      case 'bounce': {
        await bounce(element, duration);
        break;
      }
      case 'shake': {
        await shake(element, duration);
        break;
      }
      case 'pulse': {
        await pulse(element, duration);
        break;
      }
      case 'spin': {
        await spin(element, duration);
        break;
      }
    }
  }
}

// Vue 组合式函数
export function useAnimation() {
  const isAnimating = ref(false);

  async function animate(fn: () => Promise<void>) {
    isAnimating.value = true;
    try {
      await fn();
    } finally {
      isAnimating.value = false;
    }
  }

  return {
    isAnimating,
    animate,
    fadeIn,
    fadeOut,
    slideDown,
    slideUp,
    bounce,
    shake,
    pulse,
    spin,
    sequence,
  };
}

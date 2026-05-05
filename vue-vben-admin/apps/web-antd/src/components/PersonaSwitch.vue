<template>
  <a-dropdown>
    <div class="persona-switch">
      <span class="persona-icon">{{ personaStore.personaDisplay?.icon || '👤' }}</span>
      <span class="persona-name">{{ personaStore.personaDisplay?.name || '选择身份' }}</span>
      <DownOutlined />
    </div>
    <template #overlay>
      <a-menu @click="handleSwitch">
        <a-menu-item
          v-for="p in personaStore.availablePersonas"
          :key="p.id"
          :disabled="p.id === personaStore.currentPersona?.id"
        >
          <span class="menu-icon">{{ p.icon }}</span>
          {{ p.name }}
        </a-menu-item>
      </a-menu>
    </template>
  </a-dropdown>
</template>

<script setup lang="ts">
import { message } from 'ant-design-vue';
import { DownOutlined } from '@ant-design/icons-vue';
import { usePersonaStore } from '#/store/persona';

const personaStore = usePersonaStore();

const handleSwitch = async ({ key }: { key: string }) => {
  const success = await personaStore.switchPersona(key);
  if (success) {
    message.success('身份切换成功');
  } else {
    message.error('切换失败');
  }
};
</script>

<style scoped>
.persona-switch {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 4px 12px;
  cursor: pointer;
  border-radius: 4px;
  transition: background 0.2s;
}

.persona-switch:hover {
  background: rgba(255, 255, 255, 0.1);
}

.persona-icon {
  font-size: 16px;
}

.persona-name {
  font-size: 14px;
}

.menu-icon {
  margin-right: 8px;
}
</style>

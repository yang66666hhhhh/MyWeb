import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import { currentPersonaApi } from '#/api/system/persona';

interface PersonaType {
  id: string;
  code: string;
  name: string;
  icon: string;
  description?: string;
  defaultHomeRoute?: string;
  isPrimary?: boolean;
}

export const usePersonaStore = defineStore('persona', () => {
  const currentPersona = ref<PersonaType | null>(null);
  const availablePersonas = ref<PersonaType[]>([]);
  const loading = ref(false);
  const initialized = ref(false);

  const personaDisplay = computed(() => {
    if (!currentPersona.value) return null;
    return {
      icon: currentPersona.value.icon || '👤',
      name: currentPersona.value.name || '未设置',
      code: currentPersona.value.code || '',
    };
  });

  async function loadPersonaData() {
    loading.value = true;
    try {
      const currentRes = await currentPersonaApi.getCurrent() as any;
      const availableRes = await currentPersonaApi.getAvailable() as any;

      if (currentRes) {
        currentPersona.value = currentRes;
      }
      if (availableRes && Array.isArray(availableRes)) {
        availablePersonas.value = availableRes;
      }
      initialized.value = true;
    } catch (e) {
      // 静默处理 - 用户可能没有 persona
      initialized.value = true;
    } finally {
      loading.value = false;
    }
  }

  async function switchPersona(personaTypeId: string) {
    try {
      await currentPersonaApi.switch(personaTypeId);
      await loadPersonaData();
      return true;
    } catch {
      return false;
    }
  }

  function $reset() {
    currentPersona.value = null;
    availablePersonas.value = [];
    initialized.value = false;
  }

  return {
    currentPersona,
    availablePersonas,
    loading,
    initialized,
    personaDisplay,
    loadPersonaData,
    switchPersona,
    $reset,
  };
});

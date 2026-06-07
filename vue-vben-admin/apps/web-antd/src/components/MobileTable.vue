<script setup lang="ts">
import { useIsMobile } from '#/composables/useIsMobile';

const { isMobile } = useIsMobile();
</script>

<template>
  <div :class="{ 'table-responsive': isMobile }">
    <slot />
  </div>
</template>

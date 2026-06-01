<script lang="ts" setup>
interface Props {
  rows?: number;
  avatar?: boolean;
  active?: boolean;
}

withDefaults(defineProps<Props>(), {
  rows: 4,
  avatar: false,
  active: true,
});
</script>

<template>
  <div class="animate-pulse space-y-4">
    <div v-if="avatar" class="flex items-center space-x-4">
      <div class="h-12 w-12 rounded-full bg-gray-200" />
      <div class="flex-1 space-y-2">
        <div class="h-4 w-1/4 rounded bg-gray-200" />
        <div class="h-3 w-1/2 rounded bg-gray-200" />
      </div>
    </div>
    <div class="space-y-3">
      <div
        v-for="i in rows"
        :key="i"
        class="h-4 rounded bg-gray-200"
        :style="{ width: i === rows ? '60%' : '100%' }"
      />
    </div>
  </div>
</template>

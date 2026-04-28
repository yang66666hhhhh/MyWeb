import { reactive, ref, shallowRef } from 'vue';

import { defineStore } from 'pinia';

import type { DailyPlan, DailyPlanQuery } from '#/api/growth';

import {
  completeDailyPlanApi,
  deleteDailyPlanApi,
  getDailyPlanPageApi,
  updateDailyPlanStatusApi,
} from '#/api/growth';

export const useDailyPlanStore = defineStore('daily-plan', () => {
  const loading = ref(false);
  const items = shallowRef<DailyPlan[]>([]);
  const total = ref(0);
  const formOpen = ref(false);
  const editingId = ref<null | string>(null);

  const query = reactive<DailyPlanQuery>({
    date: new Date().toISOString().slice(0, 10),
    page: 1,
    pageSize: 10,
  });

  async function fetchPage() {
    loading.value = true;
    try {
      const result = await getDailyPlanPageApi({ ...query });
      const pageItems = result.items.filter((item) => {
        const inPriority =
          query.priority === undefined || item.priority === query.priority;
        const inDate = !query.date || item.planDate === query.date;
        return inPriority && inDate;
      });
      items.value = pageItems;
      total.value =
        query.priority !== undefined || query.date ? pageItems.length : result.total;
      query.page = result.page;
      query.pageSize = result.pageSize;
    } finally {
      loading.value = false;
    }
  }

  async function search() {
    query.page = 1;
    await fetchPage();
  }

  function openCreate() {
    editingId.value = null;
    formOpen.value = true;
  }

  function openEdit(id: string) {
    editingId.value = id;
    formOpen.value = true;
  }

  function closeForm() {
    formOpen.value = false;
    editingId.value = null;
  }

  async function complete(id: string) {
    await completeDailyPlanApi(id);
    await fetchPage();
  }

  async function changeStatus(plan: DailyPlan, status: DailyPlan['status']) {
    if (status === 2) {
      await completeDailyPlanApi(plan.id);
    } else {
      await updateDailyPlanStatusApi(plan, status);
    }
    await fetchPage();
  }

  async function remove(id: string) {
    await deleteDailyPlanApi(id);
    await fetchPage();
  }

  function $reset() {
    loading.value = false;
    items.value = [];
    total.value = 0;
    formOpen.value = false;
    editingId.value = null;
    Object.assign(query, {
      date: new Date().toISOString().slice(0, 10),
      endDate: undefined,
      keyword: undefined,
      page: 1,
      pageSize: 10,
      priority: undefined,
      startDate: undefined,
      status: undefined,
    });
  }

  return {
    $reset,
    changeStatus,
    closeForm,
    complete,
    editingId,
    fetchPage,
    formOpen,
    items,
    loading,
    openCreate,
    openEdit,
    query,
    remove,
    search,
    total,
  };
});

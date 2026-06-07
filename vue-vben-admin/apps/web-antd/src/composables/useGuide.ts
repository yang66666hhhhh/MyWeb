import { ref } from 'vue';

import { GUIDE_STORAGE_KEYS } from '#/config/guide';

export function useGuide() {
  const isTourActive = ref(false);

  const hasTourCompleted = (): boolean => {
    return localStorage.getItem(GUIDE_STORAGE_KEYS.TOUR_COMPLETED) === 'true';
  };

  const hasTourSkipped = (): boolean => {
    return localStorage.getItem(GUIDE_STORAGE_KEYS.TOUR_SKIPPED) === 'true';
  };

  const markTourCompleted = () => {
    localStorage.setItem(GUIDE_STORAGE_KEYS.TOUR_COMPLETED, 'true');
    isTourActive.value = false;
  };

  const markTourSkipped = () => {
    localStorage.setItem(GUIDE_STORAGE_KEYS.TOUR_SKIPPED, 'true');
    isTourActive.value = false;
  };

  const shouldShowTour = (): boolean => {
    return !hasTourCompleted() && !hasTourSkipped();
  };

  const resetTour = () => {
    localStorage.removeItem(GUIDE_STORAGE_KEYS.TOUR_COMPLETED);
    localStorage.removeItem(GUIDE_STORAGE_KEYS.TOUR_SKIPPED);
  };

  const isFeatureDismissed = (featureId: string): boolean => {
    return localStorage.getItem(`${GUIDE_STORAGE_KEYS.FEATURE_DISMISSED}${featureId}`) === 'true';
  };

  const dismissFeature = (featureId: string) => {
    localStorage.setItem(`${GUIDE_STORAGE_KEYS.FEATURE_DISMISSED}${featureId}`, 'true');
  };

  const hasPageVisited = (pageId: string): boolean => {
    return localStorage.getItem(`${GUIDE_STORAGE_KEYS.PAGE_VISITED}${pageId}`) === 'true';
  };

  const markPageVisited = (pageId: string) => {
    localStorage.setItem(`${GUIDE_STORAGE_KEYS.PAGE_VISITED}${pageId}`, 'true');
  };

  const shouldShowPageGuide = (pageId: string): boolean => {
    return !hasPageVisited(pageId) && !hasTourCompleted();
  };

  return {
    isTourActive,
    hasTourCompleted,
    hasTourSkipped,
    markTourCompleted,
    markTourSkipped,
    shouldShowTour,
    resetTour,
    isFeatureDismissed,
    dismissFeature,
    hasPageVisited,
    markPageVisited,
    shouldShowPageGuide,
  };
}

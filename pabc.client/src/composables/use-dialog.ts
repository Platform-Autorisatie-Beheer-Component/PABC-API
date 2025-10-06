import { readonly, ref } from "vue";

export const useDialog = () => {
  const isOpen = ref(false);

  let resolvePromise: ((value: boolean) => void) | null = null;

  const open = (): Promise<boolean> => {
    isOpen.value = true;

    return new Promise((resolve) => {
      resolvePromise = resolve;
    });
  };

  const confirm = () => {
    isOpen.value = false;

    resolvePromise?.(true);
    resolvePromise = null;
  };

  const cancel = () => {
    isOpen.value = false;

    resolvePromise?.(false);
    resolvePromise = null;
  };

  return {
    isOpen: readonly(isOpen),
    open,
    confirm,
    cancel
  };
};

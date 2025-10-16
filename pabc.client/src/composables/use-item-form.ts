import { ref, watch } from "vue";
import toast from "@/components/toast/toast";
import type { Item, PabcService } from "@/services/pabcService";

export const useItemForm = <T extends Item>(pabcService: PabcService<T>, itemName: string) => {
  const item = ref<Item | null>(null);
  const form = ref<T>({} as T);

  const loading = ref(false);
  const error = ref("");
  const invalid = ref("");

  watch(item, (value) => (form.value = { ...(value ?? {}) }));

  const fetchItem = async (id: string) => {
    clearItem();

    loading.value = true;

    try {
      item.value = await pabcService.getById(id);
    } catch (err: unknown) {
      error.value = `${err instanceof Error ? err.message : err}`;
    } finally {
      loading.value = false;
    }
  };

  const submitItem = async () => {
    loading.value = true;
    invalid.value = "";

    try {
      if (!item.value?.id) {
        item.value = await pabcService.create(form.value);
      } else {
        item.value = await pabcService.update(form.value);
      }

      toast.add({ text: `${itemName} succesvol opgeslagen.` });
    } catch (err: unknown) {
      invalid.value = `${err instanceof Error ? err.message : err}`;

      throw err;
    } finally {
      loading.value = false;
    }
  };

  const deleteItem = async () => {
    if (!item.value?.id) return;

    loading.value = true;

    try {
      await pabcService.delete(item.value.id);

      clearItem();

      toast.add({ text: `${itemName} succesvol verwijderd.` });
    } catch (err: unknown) {
      toast.add({ text: `${err instanceof Error ? err.message : err}`, type: "error" });
    } finally {
      loading.value = false;
    }
  };

  const clearItem = () => {
    item.value = null;
    error.value = "";
    invalid.value = "";
  };

  return {
    form,
    loading,
    error,
    invalid,
    fetchItem,
    submitItem,
    deleteItem,
    clearItem
  };
};

import { ref, watch } from "vue";
import toast from "@/components/toast/toast";
import type { PabcService } from "@/services/pabcService";

export const useItemForm = <T extends { name: string }>(
  pabcService: PabcService<T>,
  itemName: string
) => {
  const form = ref<T>({} as T);

  const item = ref<T | null>(null);
  const loading = ref(false);
  const error = ref("");
  const invalid = ref("");

  watch(item, (value) => value && (form.value = { ...value }));

  const fetchItem = async (id: string) => {
    loading.value = true;
    error.value = "";

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
      if (!form.value?.id) {
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

  const deleteItem = async (id: string) => {
    loading.value = true;

    try {
      await pabcService.delete(id);

      toast.add({ text: `${itemName} succesvol verwijderd.` });
    } catch (err: unknown) {
      toast.add({ text: `${err instanceof Error ? err.message : err}`, type: "error" });
    } finally {
      loading.value = false;
    }
  };

  return {
    form,
    loading,
    error,
    invalid,
    fetchItem,
    submitItem,
    deleteItem
  };
};

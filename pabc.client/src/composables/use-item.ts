import { readonly, ref } from "vue";
import toast from "@/components/toast/toast";
import type { Item, PabcService } from "@/services/pabcService";

export const useItem = <T extends Item>(pabcService: PabcService<T>, itemName: string) => {
  const loading = ref(false);
  const invalid = ref("");

  const submitItem = async (payload: T) => {
    loading.value = true;
    invalid.value = "";

    try {
      if (!payload.id) {
        await pabcService.create(payload);
      } else {
        await pabcService.update(payload);
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

  const clearInvalid = () => (invalid.value = "");

  return {
    loading: readonly(loading),
    invalid: readonly(invalid),
    submitItem,
    deleteItem,
    clearInvalid
  };
};

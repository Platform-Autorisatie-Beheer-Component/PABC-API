import { ref, watch } from "vue";
import { useRouter } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import toast from "@/components/toast/toast";
import type { PabcService } from "@/services/pabcService";
import { knownErrorMessages } from "@/utils/fetchWrapper";

export const useItemForm = <T extends { name: string }>(
  pabcService: PabcService<T>,
  itemName: string,
  redirectRoute: string
) => {
  const router = useRouter();

  const form = ref<T>({} as T);

  const item = ref<T | null>(null);
  const loading = ref(false);
  const error = ref("");

  watch(item, (value) => value && (form.value = { ...value }));

  const confirmDialog = useConfirmDialog();

  const fetchItem = async (id: string) => {
    loading.value = true;
    error.value = "";

    try {
      item.value = await pabcService.getById(id);
    } catch (err: unknown) {
      error.value = `${itemName}: fout bij het ophalen - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  const submitItem = async () => {
    loading.value = true;

    try {
      if (!item.value?.id) {
        item.value = await pabcService.create(form.value);
      } else {
        item.value = await pabcService.update(form.value);
      }

      toast.add({ text: `${itemName}: succesvol opgeslagen.` });

      router.push({ name: redirectRoute });
    } catch (err: unknown) {
      if (err instanceof Error && err.message === knownErrorMessages.conflict) {
        toast.add({
          text: `${itemName}: '${form.value.name}' bestaat al. Kies een andere naam.`,
          type: "error"
        });
      } else {
        toast.add({ text: `${itemName}: fout bij het opslaan - ${err}`, type: "error" });
      }
    } finally {
      loading.value = false;
    }
  };

  const deleteItem = async (id: string) => {
    if ((await confirmDialog.reveal()).isCanceled) return;

    loading.value = true;

    try {
      await pabcService.delete(id);

      toast.add({ text: `${itemName}: succesvol verwijderd.` });

      router.push({ name: redirectRoute });
    } catch (err: unknown) {
      toast.add({ text: `${itemName}: fout bij het verwijderen - ${err}`, type: "error" });
    } finally {
      loading.value = false;
    }
  };

  return {
    form,
    loading,
    error,
    fetchItem,
    submitItem,
    deleteItem,
    confirmDialog
  };
};

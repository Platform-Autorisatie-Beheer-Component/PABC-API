import { ref } from "vue";
import type { Item, PabcService } from "@/services/pabcService";

export const useItemList = <T extends Item>(pabcService: PabcService<T>, itemName: string) => {
  const items = ref<T[]>([]);
  const loading = ref(false);
  const error = ref("");

  const fetchItems = async () => {
    loading.value = true;
    error.value = "";

    try {
      items.value = await pabcService.getAll();
    } catch (err: unknown) {
      error.value = `${itemName}: fout bij het ophalen - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  return {
    items,
    loading,
    error,
    fetchItems
  };
};

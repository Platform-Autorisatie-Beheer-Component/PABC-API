import { ref } from "vue";
import type { PabcService } from "@/services/pabcService";

export const useItemList = <T extends { name: string }>(
  pabcService: PabcService<T>,
  errorPrefix: string
) => {
  const items = ref<T[]>([]);
  const loading = ref(false);
  const error = ref("");

  const fetchItems = async () => {
    loading.value = true;
    error.value = "";

    try {
      items.value = (await pabcService.getAll()).sort((a, b) =>
        a.name.toLowerCase().localeCompare(b.name.toLowerCase())
      );
    } catch (err: unknown) {
      error.value = `${errorPrefix} - ${err}`;
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

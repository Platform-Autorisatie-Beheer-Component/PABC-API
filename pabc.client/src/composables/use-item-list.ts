import { onMounted, ref } from "vue";

export const useItemList = <T extends { name: string }>(
  service: { getAll(): Promise<T[]> },
  errorPrefix: string
) => {
  const items = ref<T[]>([]);
  const loading = ref(false);
  const error = ref("");

  const fetchItems = async () => {
    loading.value = true;
    error.value = "";

    try {
      items.value = (await service.getAll()).sort((a, b) =>
        a.name.toLowerCase().localeCompare(b.name.toLowerCase())
      );
    } catch (err: unknown) {
      error.value = `${errorPrefix} - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  onMounted(() => fetchItems());

  return {
    items,
    loading,
    error,
    fetchItems
  };
};

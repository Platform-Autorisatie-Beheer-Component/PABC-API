import { readonly, ref } from "vue";
import { promiseAll } from "@/utils";
import { applicationRoleService, domainService } from "@/services/pabcService";

const fetchDomainsAndApplicationRoles = async () =>
  promiseAll({
    applicationRoles: applicationRoleService.getAll(),
    domains: domainService.getAll()
  });

export const useDomainsAndApplicationRoles = () => {
  const items = ref<Awaited<ReturnType<typeof fetchDomainsAndApplicationRoles>>>();

  const loading = ref(false);
  const error = ref("");

  const fetchItems = async () => {
    loading.value = true;
    error.value = "";

    try {
      items.value = await fetchDomainsAndApplicationRoles();
    } catch (err: unknown) {
      error.value = `Fout bij het ophalen van domeinen en applicatie rollen - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  return {
    items: readonly(items),
    loading: readonly(loading),
    error: readonly(error),
    fetchItems
  };
};

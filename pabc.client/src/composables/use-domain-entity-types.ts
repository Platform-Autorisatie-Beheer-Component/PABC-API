import { readonly, ref } from "vue";
import toast from "@/components/toast/toast";
import { domainEntityTypesService, type DomainEntityTypes } from "@/services/pabcService";

export const useDomainEntityTypes = () => {
  const domains = ref<DomainEntityTypes[]>([]);

  const loading = ref(false);
  const error = ref("");
  const invalid = ref("");

  const fetchDomains = async () => {
    loading.value = true;
    error.value = "";

    try {
      domains.value = await domainEntityTypesService.getAll();
    } catch (err: unknown) {
      error.value = `Fout bij het ophalen van domeinen - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  const addEntityTypeToDomain = async (domainId: string, entityTypeId: string) => {
    loading.value = true;
    invalid.value = "";

    try {
      await domainEntityTypesService.add(domainId, entityTypeId);

      toast.add({ text: "Entiteitstype succesvol toegevoegd aan domein." });
    } catch (err: unknown) {
      invalid.value = `${err instanceof Error ? err.message : err}`;

      throw err;
    } finally {
      loading.value = false;
    }
  };

  const removeEntityTypeFromDomain = async (domainId: string, entityTypeId: string) => {
    loading.value = true;

    try {
      await domainEntityTypesService.remove(domainId, entityTypeId);

      toast.add({ text: "Entiteitstype succesvol verwijderd uit domein." });
    } catch (err: unknown) {
      toast.add({ text: `${err instanceof Error ? err.message : err}`, type: "error" });
    } finally {
      loading.value = false;
    }
  };

  const clearInvalid = () => (invalid.value = "");

  return {
    domains: readonly(domains),
    loading: readonly(loading),
    error: readonly(error),
    invalid: readonly(invalid),
    fetchDomains,
    addEntityTypeToDomain,
    removeEntityTypeFromDomain,
    clearInvalid
  };
};

import { ref } from "vue";
import { domainEntityTypesService, type DomainEntityTypes } from "@/services/pabcService";

export const useDomainEntityTypes = () => {
  const domains = ref<DomainEntityTypes[]>([]);
  const loading = ref(false);
  const error = ref("");

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

  return {
    domains,
    loading,
    error,
    fetchDomains
  };
};

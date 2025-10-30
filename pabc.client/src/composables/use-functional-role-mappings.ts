import { ref } from "vue";
import { functionalRoleMappingsService, type FunctionalRoleMappings } from "@/services/pabcService";

export const useFunctionalRoleMappings = () => {
  const functionalRoles = ref<FunctionalRoleMappings[]>([]);

  const loading = ref(false);
  const error = ref("");
  const invalid = ref("");

  const fetchFunctionalRoles = async () => {
    loading.value = true;
    error.value = "";

    try {
      functionalRoles.value = await functionalRoleMappingsService.getAll();
    } catch (err: unknown) {
      error.value = `Fout bij het ophalen van functionele rollen - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  return {
    functionalRoles,
    loading,
    error,
    invalid,
    fetchFunctionalRoles
  };
};

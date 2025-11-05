import { readonly, ref } from "vue";
import toast from "@/components/toast/toast";
import {
  functionalRoleMappingsService,
  type FunctionalRoleMappings,
  type Mapping
} from "@/services/pabcService";

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

  const addMapping = async (payload: Mapping) => {
    loading.value = true;
    invalid.value = "";

    try {
      await functionalRoleMappingsService.add(payload);

      toast.add({ text: "Mapping succesvol toegevoegd aan functionele rol." });
    } catch (err: unknown) {
      invalid.value = `${err instanceof Error ? err.message : err}`;

      throw err;
    } finally {
      loading.value = false;
    }
  };

  const removeMapping = async (functionalRoleId: string, mappingId: string) => {
    loading.value = true;

    try {
      await functionalRoleMappingsService.remove(functionalRoleId, mappingId);

      toast.add({ text: "Mapping succesvol verwijderd van functionele rol." });
    } catch (err: unknown) {
      toast.add({ text: `${err instanceof Error ? err.message : err}`, type: "error" });
    } finally {
      loading.value = false;
    }
  };

  const clearInvalid = () => (invalid.value = "");

  return {
    functionalRoles: readonly(functionalRoles),
    loading: readonly(loading),
    error: readonly(error),
    invalid: readonly(invalid),
    fetchFunctionalRoles,
    removeMapping,
    addMapping,
    clearInvalid
  };
};

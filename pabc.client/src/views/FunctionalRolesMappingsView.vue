<template>
  <div class="page">
    <h1>Functionele rollen</h1>

    <p>
      Hier zie je al jouw functionele rollen, en de koppelingen die aan de functionele rollen
      gekoppeld zijn. Wil je functionele rollen toevoegen, bewerken of verwijderen? Ga dan naar de
      <router-link :to="{ name: 'admin' }">Beheer pagina</router-link>.
    </p>

    <small-spinner v-if="loading" />

    <alert-inline v-else-if="error">{{ error }}</alert-inline>

    <div v-show="!loading && !error">
      <p v-if="!functionalRoles.length">Geen functionele rollen gevonden.</p>

      <template v-else-if="items">
        <functional-role-mappings-details
          v-for="functionalRole in functionalRoles"
          :functional-role="functionalRole"
          :domains="items.domains"
          :application-roles="items.applicationRoles"
          :key="functionalRole.id"
          @refresh="fetchFunctionalRoles"
        />
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import { useFunctionalRoleMappings } from "@/composables/use-functional-role-mappings";
import { useDomainsAndApplicationRoles } from "@/composables/use-domains-application-roles";
import FunctionalRoleMappingsDetails from "@/components/functional-role-mappings/FunctionalRoleMappingsDetails.vue";

const loading = computed(() => functionalRolesLoading.value || itemsLoading.value);
const error = computed(() => functionalRolesError.value || itemsError.value);

const {
  functionalRoles,
  loading: functionalRolesLoading,
  error: functionalRolesError,
  fetchFunctionalRoles
} = useFunctionalRoleMappings();

const {
  items,
  loading: itemsLoading,
  error: itemsError,
  fetchItems
} = useDomainsAndApplicationRoles();

onMounted(() => {
  fetchFunctionalRoles();
  fetchItems();
});
</script>

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
        <text-filter v-model="filterText" label="Filteren op functionele rol..." />

        <p aria-live="polite" class="visually-hidden">
          {{ filteredFunctionalRoles.length }} van {{ functionalRoles.length }} functionele rollen
          getoond.
        </p>

        <p v-if="filteredFunctionalRoles.length === 0">
          Geen functionele rollen gevonden voor "{{ filterText }}".
        </p>

        <functional-role-mappings-details
          v-for="functionalRole in filteredFunctionalRoles"
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
import { computed, onMounted, ref } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import TextFilter from "@/components/TextFilter.vue";
import { useFunctionalRoleMappings } from "@/composables/use-functional-role-mappings";
import { useDomainsAndApplicationRoles } from "@/composables/use-domains-application-roles";
import FunctionalRoleMappingsDetails from "@/components/functional-role-mappings/FunctionalRoleMappingsDetails.vue";

const filterText = ref("");

const filteredFunctionalRoles = computed(() => {
  const query = filterText.value.toLowerCase();

  return !query
    ? functionalRoles.value
    : functionalRoles.value.filter((r) => r.name.toLowerCase().includes(query));
});

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

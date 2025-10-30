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

      <template v-else>
        <functional-role-mappings-details
          v-for="functionalRole in functionalRoles"
          :functional-role="functionalRole"
          :key="functionalRole.id"
          @refresh="fetchFunctionalRoles"
        />
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import { useFunctionalRoleMappings } from "@/composables/use-functional-role-mappings";
import FunctionalRoleMappingsDetails from "@/components/functional-role-mappings/FunctionalRoleMappingsDetails.vue";

const {
  functionalRoles,
  loading,
  error,
  fetchFunctionalRoles
} = useFunctionalRoleMappings();

onMounted(() => fetchFunctionalRoles());
</script>

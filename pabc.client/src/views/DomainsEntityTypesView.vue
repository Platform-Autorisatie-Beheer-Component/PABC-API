<template>
  <div class="page">
    <h1>Domeinen</h1>

    <p>
      Hier zie je al jouw domeinen, en de entiteitstypes die aan de domeinen gekoppeld zijn. Wil je
      domeinen toevoegen, bewerken of verwijderen? Ga dan naar de
      <router-link :to="{ name: 'admin' }">Beheer pagina</router-link>.
    </p>

    <small-spinner v-if="loading" />

    <alert-inline v-else-if="error">{{ error }}</alert-inline>

    <div v-show="!loading && !error">
      <p v-if="!domains.length">Geen domeinen gevonden.</p>

      <template v-else>
        <domain-entity-types-details
          v-for="domain in domains"
          :domain="domain"
          :entity-types="entityTypes"
          :key="domain.id"
          @refresh="fetchDomainsAndEntityTypes"
        />
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import { useDomainEntityTypes } from "@/composables/use-domain-entity-types";
import DomainEntityTypesDetails from "@/components/domain-entity-types/DomainEntityTypesDetails.vue";
import { entityTypeService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";

const loading = computed(() => domainsLoading.value || entityTypesLoading.value);
const error = computed(() => domainsError.value || entityTypesError.value);

const {
  domains,
  loading: domainsLoading,
  error: domainsError,
  fetchDomains
} = useDomainEntityTypes();

const {
  items: entityTypes,
  loading: entityTypesLoading,
  error: entityTypesError,
  fetchItems: fetchEntityTypes
} = useItemList(entityTypeService, "Entiteitstypes");

const fetchDomainsAndEntityTypes = () => {
  fetchDomains();
  fetchEntityTypes();
};

onMounted(() => fetchDomainsAndEntityTypes());
</script>

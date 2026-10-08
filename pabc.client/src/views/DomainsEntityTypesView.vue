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
        <text-filter v-model="filterText" label="Filteren op domeinnaam..." />

        <p aria-live="polite" class="visually-hidden">
          {{ filteredDomains.length }} van {{ domains.length }} domeinen getoond.
        </p>

        <p v-if="filteredDomains.length === 0">Geen domeinen gevonden voor "{{ filterText }}".</p>

        <domain-entity-types-details
          v-for="domain in filteredDomains"
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
import { computed, onMounted, ref } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import TextFilter from "@/components/TextFilter.vue";
import { useDomainEntityTypes } from "@/composables/use-domain-entity-types";
import DomainEntityTypesDetails from "@/components/domain-entity-types/DomainEntityTypesDetails.vue";
import { entityTypeService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";

const filterText = ref("");

const filteredDomains = computed(() => {
  const query = filterText.value.toLowerCase();

  return !query ? domains.value : domains.value.filter((d) => d.name.toLowerCase().includes(query));
});

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

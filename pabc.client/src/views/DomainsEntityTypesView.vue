<template>
  <section class="page">
    <h1>Domeinen</h1>

    <p>
      Hier zie je al jouw domeinen, en de entiteitstypes die aan de domeinen gekoppeld zijn. Wil je
      domeinen toevoegen, bewerken of verwijderen? Ga dan naar de
      <router-link :to="{ name: 'admin' }">Beheer pagina</router-link>.
    </p>

    <alert-inline v-if="error">{{ error }}</alert-inline>

    <small-spinner v-else-if="loading" />

    <template v-else>
      <p v-if="!domains.length">Geen domeinen gevonden.</p>

      <section v-else>
        <domain-entity-types-details v-for="domain in domains" :domain="domain" :key="domain.id" />
      </section>
    </template>
  </section>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import DomainEntityTypesDetails from "@/components/domain-entity-types/DomainEntityTypesDetails.vue";
import { useDomainEntityTypes } from "@/composables/use-domain-entity-types";

const { domains, loading, error, fetchDomains } = useDomainEntityTypes();

onMounted(() => fetchDomains());
</script>

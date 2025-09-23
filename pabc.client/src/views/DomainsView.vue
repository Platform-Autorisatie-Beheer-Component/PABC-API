<template>
  <section class="header">
    <h1>Domeinen</h1>

    <p>
      <router-link :to="{ name: 'domain' }" class="button">Nieuw domein</router-link>
    </p>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!domains.length">Geen domeinen gevonden.</p>

  <ul v-else class="reset">
    <li v-for="{ id, name } in domains" :key="id">
      <router-link
        :to="{
          name: 'domain',
          params: { id }
        }"
        class="button button-secondary"
        >{{ name }}</router-link
      >
    </li>
  </ul>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { pabcService, type Domain } from "@/services/pabcService";

const domains = ref<Domain[]>([]);

const loading = ref(false);
const error = ref("");

const fetchDomains = async () => {
  loading.value = true;
  error.value = "";

  try {
    domains.value = await pabcService.getAllDomains();
  } catch (err: unknown) {
    error.value = `Fout bij het ophalen van de domeinen - ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchDomains());
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.header {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
  justify-content: space-between;
  align-items: center;
  padding-block: var(--spacing-default);

  @media (min-width: variables.$breakpoint-md) {
    & {
      flex-direction: row;
    }
  }

  * {
    margin: 0;
  }
}

ul {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(var(--section-width-small), 1fr));
  grid-gap: var(--spacing-default);

  .button {
    display: flex;
    padding-block: var(--spacing-default);
    margin-block-end: 0;
  }
}
</style>

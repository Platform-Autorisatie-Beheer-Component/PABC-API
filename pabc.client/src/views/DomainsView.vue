<template>
  <section>
    <h1>Domeinen</h1>

    <p>
      <router-link :to="{ name: 'domain' }" class="button">Nieuw domein</router-link>
    </p>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!domains.length">Geen domeinen gevonden.</p>

  <ul v-else class="reset">
    <li v-for="{ id, name, description } in domains" :key="id">
      <router-link
        :to="{
          name: 'domain',
          params: { id }
        }"
        class="card"
      >
        <h2>{{ name }}</h2>
        <p>{{ description }}</p>
      </router-link>
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
    domains.value = (await pabcService.getAllDomains()).sort((a, b) =>
      a.name.toLowerCase().localeCompare(b.name.toLowerCase())
    );
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

section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
  justify-content: space-between;
  align-items: center;
  padding-block: var(--spacing-default);

  @media (min-width: variables.$breakpoint-md) {
    flex-direction: row;
  }

  * {
    margin: 0;
  }
}

ul {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(var(--section-width-small), 1fr));
  grid-gap: var(--spacing-default);
}

.card {
  display: flex;
  flex-direction: column;
  row-gap: var(--spacing-small);
  block-size: 100%;
  padding: var(--spacing-default);
  border: 1px solid var(--border);
  text-decoration: none;

  &:hover,
  &:focus-visible {
    h2 {
      text-decoration: underline;
    }
  }

  h2,
  p {
    font-weight: 300;
    margin: 0;
  }

  p {
    color: var(--text);
  }
}
</style>

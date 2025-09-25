<template>
  <section class="heading-section">
    <h1>Domeinen</h1>

    <p>
      <router-link :to="{ name: 'domain' }" class="button">Nieuw domein</router-link>
    </p>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!domains.length">Geen domeinen gevonden.</p>

  <ul v-else class="simple-grid reset">
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
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { domainService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";

const {
  items: domains,
  loading,
  error,
  fetchItems
} = useItemList(domainService, "Fout bij het ophalen van de domeinen");

onMounted(() => fetchItems());
</script>

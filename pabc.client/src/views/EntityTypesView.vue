<template>
  <section class="heading-section">
    <h1>Entiteit types</h1>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!entityTypes.length">Geen entiteit types gevonden.</p>

  <ul v-else class="simple-grid reset">
    <li v-for="{ id, name, entityTypeId, type, uri } in entityTypes" :key="id">
      <router-link
        :to="{
          name: 'entityTypes',
          params: { id }
        }"
        class="card"
      >
        <h2>{{ name }}</h2>
        <p>{{ entityTypeId }}</p>
        <p>{{ type }}</p>
        <p>{{ uri }}</p>
      </router-link>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { entityTypeService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";

const {
  items: entityTypes,
  loading,
  error,
  fetchItems
} = useItemList(entityTypeService, "Fout bij het ophalen van de entiteit types");

onMounted(() => fetchItems());
</script>

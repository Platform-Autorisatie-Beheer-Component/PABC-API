<template>
  <section class="heading-section">
    <h1>Functionele rollen</h1>

    <p>
      <router-link :to="{ name: 'functionalRole' }" class="button"
        >Nieuwe functionele rol</router-link
      >
    </p>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!functionalRoles.length">Geen functionele rollen gevonden.</p>

  <ul v-else class="simple-grid reset">
    <li v-for="{ id, name } in functionalRoles" :key="id">
      <router-link
        :to="{
          name: 'functionalRole',
          params: { id }
        }"
        class="card"
      >
        <h2>{{ name }}</h2>
      </router-link>
    </li>
  </ul>
</template>

<script setup lang="ts">
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { functionalRoleService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";

const {
  items: functionalRoles,
  loading,
  error
} = useItemList(functionalRoleService, "Fout bij het ophalen van de functionele rollen");
</script>

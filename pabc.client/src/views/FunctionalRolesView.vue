<template>
  <section>
    <h1>Functionele rollen</h1>

    <p>
      <router-link :to="{ name: 'functionalRole' }" class="button">Nieuwe functionele rol</router-link>
    </p>
  </section>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!functionalRoles.length">Geen functionele rollen gevonden.</p>

  <ul v-else class="reset">
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

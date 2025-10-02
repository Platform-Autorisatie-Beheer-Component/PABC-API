<template>
  <h1>Beheer</h1>

  <p>Op deze pagina kan je lijst-items aanvullen, aanpassen of verwijderen.</p>

  <section>
    <item-details :pabc-service="domainService" item-name="Domeinen">
      <template #item="{ item: domain }">
        <h2>{{ domain.name }}</h2>
        <p>{{ domain.description }}</p>
      </template>

      <template #form="{ form }">
        <h2>{{ form.name }}</h2>
        <p>{{ form.description }}</p>
      </template>
    </item-details>

    <item-details :pabc-service="functionalRoleService" item-name="Functionele rollen">
      <template #item="{ item: functionalRole }">
        <h2>{{ functionalRole.name }}</h2>
      </template>
    </item-details>

    <item-details :pabc-service="entityTypeService" item-name="Entiteit types">
      <template #item="{ item: entityType }">
        <h2>{{ entityType.type }}</h2>
        <p>{{ entityType.name }}</p>
      </template>
    </item-details>
  </section>
</template>

<script setup lang="ts">
import { domainService, functionalRoleService, entityTypeService } from "@/services/pabcService";
import ItemDetails from "@/components/admin/ItemDetails.vue";
</script>

<style lang="scss" scoped>
section {
  h2,
  p {
    margin-block: 0;
    font-size: inherit;
  }
}
</style>

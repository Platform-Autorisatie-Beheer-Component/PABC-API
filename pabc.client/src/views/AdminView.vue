<template>
  <h1>Beheer</h1>

  <p>Op deze pagina kan je lijst-items aanvullen, aanpassen of verwijderen.</p>

  <section>
    <item-details
      :pabc-service="domainService"
      item-name-singular="Domein"
      item-name-plural="Domeinen"
    >
      <template #item="{ item: domain }">
        <h2>{{ domain.name }}</h2>
        <p>{{ domain.description }}</p>
      </template>

      <template #form="{ form }">
        <domain-form :domain="form" />
      </template>
    </item-details>

    <item-details
      :pabc-service="functionalRoleService"
      item-name-singular="Functionele rol"
      item-name-plural="Functionele rollen"
    >
      <template #item="{ item: functionalRole }">
        <h2>{{ functionalRole.name }}</h2>
      </template>

      <template #form="{ form }">
        <functional-role-form :functional-role="form" />
      </template>
    </item-details>

    <item-details
      :pabc-service="entityTypeService"
      item-name-singular="Entiteit type"
      item-name-plural="Entiteit types"
    >
      <template #item="{ item: entityType }">
        <h2>{{ entityType.type }}</h2>
        <p>{{ entityType.name }}</p>
      </template>

      <template #form="{ form }">
        <entity-type-form :entity-type="form" />
      </template>
    </item-details>
  </section>
</template>

<script setup lang="ts">
import { domainService, functionalRoleService, entityTypeService } from "@/services/pabcService";
import ItemDetails from "@/components/ItemDetails.vue";
import DomainForm from "@/components/forms/DomainForm.vue";
import FunctionalRoleForm from "@/components/forms/FunctionalRoleForm.vue";
import EntityTypeForm from "@/components/forms/EntityTypeForm.vue";
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

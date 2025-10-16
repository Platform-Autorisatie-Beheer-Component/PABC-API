<template>
  <h1>Beheer</h1>

  <p>Op deze pagina kan je lijst-items aanvullen, aanpassen of verwijderen.</p>

  <h2>Hoofdlijsten</h2>

  <section>
    <item-details
      :pabc-service="applicationRoleService"
      item-name-singular="Applicatierol"
      item-name-plural="Applicatierollen"
    >
      <template #item="{ item: applicationRole }">
        <h3>{{ applicationRole.application }}</h3>
        <p>{{ applicationRole.name }}</p>
      </template>

      <template #form="{ form }">
        <application-role-form :application-role="form" />
      </template>
    </item-details>

    <item-details
      :pabc-service="domainService"
      item-name-singular="Domein"
      item-name-plural="Domeinen"
    >
      <template #item="{ item: domain }">
        <h3>{{ domain.name }}</h3>
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
        <h3>{{ functionalRole.name }}</h3>
      </template>

      <template #form="{ form }">
        <functional-role-form :functional-role="form" />
      </template>
    </item-details>
  </section>

  <h2>Overige lijsten</h2>

  <section>
    <item-details
      :pabc-service="entityTypeService"
      item-name-singular="Entiteitstype"
      item-name-plural="Entiteitstypes"
    >
      <template #item="{ item: entityType }">
        <h3>{{ entityType.type }}</h3>
        <p>{{ entityType.name }}</p>
      </template>

      <template #form="{ form }">
        <entity-type-form :entity-type="form" />
      </template>
    </item-details>
  </section>
</template>

<script setup lang="ts">
import {
  domainService,
  functionalRoleService,
  entityTypeService,
  applicationRoleService
} from "@/services/pabcService";
import ItemDetails from "@/components/ItemDetails.vue";
import DomainForm from "@/components/forms/DomainForm.vue";
import FunctionalRoleForm from "@/components/forms/FunctionalRoleForm.vue";
import EntityTypeForm from "@/components/forms/EntityTypeForm.vue";
import ApplicationRoleForm from "@/components/forms/ApplicationRoleForm.vue";
</script>

<style lang="scss" scoped>
section {
  inline-size: min(100%, var(--section-width-large));

  h3,
  p {
    margin-block: 0;
    font-size: inherit;
  }
}
</style>

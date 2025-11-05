<template>
  <div class="page">
    <h1>Beheer</h1>

    <p>Op deze pagina kan je lijst-item aanvullen, aanpassen of verwijderen.</p>

    <h2>Hoofdlijsten</h2>

    <small-spinner v-if="loading" />

    <alert-inline v-else-if="error">{{ error }}</alert-inline>

    <item-details
      v-else
      :pabc-service="applicationRoleService"
      item-name-singular="Applicatierol"
      item-name-plural="Applicatierollen"
    >
      <template #item="{ item: applicationRole }">
        <p>{{ applicationRole.name }}</p>
      </template>

      <template #form="{ form }">
        <application-role-form :application-role="form" :applications="applications" />
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

    <h2>Overige lijsten</h2>

    <item-details
      :pabc-service="applicationService"
      item-name-singular="Applicatie"
      item-name-plural="Applicaties"
    >
      <template #item="{ item: application }">
        <p>{{ application.name }}</p>
      </template>

      <template #form="{ form }">
        <application-form :application="form" />
      </template>
    </item-details>

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
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import {
  domainService,
  functionalRoleService,
  entityTypeService,
  applicationRoleService,
  applicationService
} from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";
import ItemDetails from "@/components/item/ItemDetails.vue";
import DomainForm from "@/components/item/forms/DomainForm.vue";
import FunctionalRoleForm from "@/components/item/forms/FunctionalRoleForm.vue";
import EntityTypeForm from "@/components/item/forms/EntityTypeForm.vue";
import ApplicationRoleForm from "@/components/item/forms/ApplicationRoleForm.vue";
import ApplicationForm from "@/components/item/forms/ApplicationForm.vue";

const {
  items: applications,
  loading,
  error,
  fetchItems: fetchApplications
} = useItemList(applicationService, "Applicaties");

onMounted(() => fetchApplications());
</script>

<style lang="scss" scoped>
.spinner {
  margin-block-end: var(--spacing-default);
}

h3,
p {
  margin-block: 0;
  font-size: inherit;
}
</style>

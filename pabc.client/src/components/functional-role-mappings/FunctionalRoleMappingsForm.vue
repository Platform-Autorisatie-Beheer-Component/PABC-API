<template>
  <fieldset aria-labelledby="mapping">
    <h2 id="mapping">Koppeling toevoegen</h2>

    <div class="form-group">
      <label for="applicationRoleId">Applicatierol *</label>

      <select
        name="applicationRoleId"
        id="applicationRoleId"
        v-model="mapping.applicationRoleId"
        required
        aria-required="true"
        aria-describedby="applicationRoleIdError"
        :aria-invalid="!mapping.applicationRoleId"
      >
        <option v-if="!mapping.applicationRoleId" value="">- Selecteer -</option>

        <option v-for="{ id, name, application } in applicationRoles" :key="id" :value="id">
          {{ name }} ({{ application }})
        </option>
      </select>

      <span id="applicationRoleIdError" class="error">Applicatierol is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label> <input type="checkbox" v-model="mapping.isAllEntityTypes" /> IsAllEntityTypes </label>
    </div>

    <div v-if="!mapping.isAllEntityTypes" class="form-group">
      <label for="domainId">Domein</label>

      <select name="domainId" id="domainId" v-model="mapping.domainId">
        <option :value="null">- Geen domein gelecteerd -</option>

        <option v-for="{ id, name } in domains" :key="id" :value="id">
          {{ name }}
        </option>
      </select>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import { watch, type DeepReadonly } from "vue";
import type { ApplicationRole, Domain, Mapping } from "@/services/pabcService";

defineProps<{
  applicationRoles: DeepReadonly<ApplicationRole[]>;
  domains: DeepReadonly<Domain[]>;
}>();

const mapping = defineModel<Mapping>("mapping", { required: true });

watch(
  () => mapping.value.isAllEntityTypes,
  (isAllEntityTypes) => !isAllEntityTypes && (mapping.value.domainId = null)
);
</script>

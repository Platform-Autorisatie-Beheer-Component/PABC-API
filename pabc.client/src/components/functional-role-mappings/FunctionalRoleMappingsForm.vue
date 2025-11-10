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

    <div role="radiogroup" aria-labelledby="entityTypes" class="form-group">
      <h3 id="entityTypes">Deze koppeling geldt voor *</h3>

      <label>
        <input
          type="radio"
          name="entityTypeOption"
          value="all"
          v-model="selectedOption"
          required
          aria-required="true"
        />

        Alle entiteitstypes
      </label>

      <label>
        <input
          type="radio"
          name="entityTypeOption"
          value="none"
          v-model="selectedOption"
          required
          aria-required="true"
        />

        Geen enkel entiteitstype
      </label>

      <label>
        <input
          type="radio"
          name="entityTypeOption"
          value="domain"
          v-model="selectedOption"
          required
          aria-required="true"
        />

        De entiteitstypes van een specifiek domein
      </label>
    </div>

    <div class="form-group" v-if="selectedOption === 'domain'">
      <select
        name="domainId"
        id="domainId"
        v-model="domainId"
        required
        aria-label="Domein"
        aria-required="true"
        aria-describedby="domainIdError"
        :aria-invalid="!domainId"
      >
        <option v-if="!domainId" value="">- Selecteer domein -</option>

        <option v-for="{ id, name } in domains" :key="id" :value="id">
          {{ name }}
        </option>
      </select>

      <span id="domainIdError" class="error">Domein is een verplicht veld</span>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import type { ApplicationRole, Domain, Mapping } from "@/services/pabcService";

defineProps<{
  applicationRoles: readonly ApplicationRole[];
  domains: readonly Domain[];
}>();

const mapping = defineModel<Mapping>("mapping", { required: true });

const domainId = ref("");

// reset domainId when mapping reference changes (createEmptyMapping)
watch(mapping, () => (domainId.value = mapping.value.domainId || ""));

// set mapping.domainId when domainId is selected
watch(domainId, (id) => (mapping.value.domainId = id));

const selectedOption = computed({
  get() {
    if (mapping.value.isAllEntityTypes) return "all";
    if (mapping.value.domainId === null) return "none";

    return "domain";
  },
  set(value: "all" | "none" | "domain") {
    mapping.value.isAllEntityTypes = value === "all";
    mapping.value.domainId = value === "domain" ? domainId.value : null;
  }
});
</script>

<style lang="scss" scoped>
[role="radiogroup"] {
  margin-block-end: 0;

  h3 {
    font-size: inherit;
    margin-block-start: 0;
    margin-block-end: var(--spacing-default);
  }

  label {
    font-weight: 300;
  }
}
</style>

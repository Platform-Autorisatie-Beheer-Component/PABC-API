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

    <div role="radiogroup" aria-labelledby="entityTypes" class="form-group" :key="radioGroupKey">
      <h3 id="entityTypes">Deze koppeling geldt voor *</h3>

      <label>
        <input
          type="radio"
          name="entityTypeOption"
          value="all"
          v-model="mode"
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
          v-model="mode"
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
          v-model="mode"
          required
          aria-required="true"
        />

        De entiteitstypes van een specifiek domein
      </label>
    </div>

    <div class="form-group" v-if="mode === 'domain'">
      <select
        name="domainId"
        id="domainId"
        v-model="mapping.domainId"
        required
        aria-label="Domein"
        aria-required="true"
        aria-describedby="domainIdError"
        :aria-invalid="!mapping.domainId"
      >
        <option v-if="!mapping.domainId" value="">- Selecteer domein -</option>

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

const mode = computed({
  get() {
    if (mapping.value.isAllEntityTypes) return "all";
    if (mapping.value.domainId != null) return "domain";
    return "none";
  },
  set(value) {
    mapping.value.isAllEntityTypes = value === "all";
    mapping.value.domainId = value === "domain" ? "" : null;
  }
});

// remount radiogroup when mapping reference changes
// to resync derived radio state, which might be lost after e.g. native form reset
const radioGroupKey = ref(0);
watch(mapping, () => radioGroupKey.value++);
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

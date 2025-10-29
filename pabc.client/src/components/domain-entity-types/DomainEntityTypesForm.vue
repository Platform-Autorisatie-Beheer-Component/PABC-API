<template>
  <fieldset aria-labelledby="entityType">
    <h2 id="entityType">Entiteitstype toevoegen</h2>

    <div class="form-group">
      <label for="type">Selecteer entiteitstype *</label>

      <select
        name="type"
        id="type"
        v-model="selectedEntityTypeId"
        required
        aria-required="true"
        aria-describedby="typeError"
        :aria-invalid="!selectedEntityTypeId"
      >
        <option v-if="!selectedEntityTypeId" value="">- Selecteer -</option>

        <option v-for="{ id, name } in entityTypes" :key="id" :value="id">
          {{ name }}
        </option>
      </select>

      <span id="typeError" class="error">Entiteitstype is een verplicht veld</span>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import type { EntityType } from "@/services/pabcService";

defineProps<{ entityTypes: EntityType[] }>();

const selectedEntityTypeId = defineModel<string>("selectedEntityTypeId", { required: true });
</script>

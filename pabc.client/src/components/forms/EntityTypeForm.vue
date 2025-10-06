<template>
  <fieldset aria-labelledby="entityType">
    <h2 id="entityType">Entiteitstype {{ !entityType.id ? `toevoegen` : `bijwerken` }}</h2>

    <div class="form-group">
      <label for="type">Soort entiteitstype *</label>

      <input
        id="type"
        type="text"
        v-model.trim="entityType.type"
        :maxlength="MAXLENGTH"
        required
        aria-required="true"
        aria-describedby="typeError"
        :aria-invalid="!entityType.type"
      />

      <span id="typeError" class="error">Soort entiteitstype is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="entityTypeId">Type ID *</label>

      <input
        id="entityTypeId"
        type="text"
        v-model.trim="entityType.entityTypeId"
        :maxlength="MAXLENGTH"
        required
        aria-required="true"
        aria-describedby="entityTypeIdError"
        :aria-invalid="!entityType.entityTypeId"
      />

      <span id="entityTypeIdError" class="error">Type ID is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="name">Naam entiteitstype *</label>

      <input
        id="name"
        type="text"
        v-model.trim="entityType.name"
        :maxlength="MAXLENGTH"
        required
        aria-required="true"
        aria-describedby="nameError"
        :aria-invalid="!entityType.name"
      />

      <span id="nameError" class="error">Naam is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="uri">URL</label>

      <input id="uri" type="text" v-model.trim="entityType.uri" />
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import type { EntityType } from "@/services/pabcService";

const entityType = defineModel<EntityType>("entityType", { required: true });

const MAXLENGTH = 256;
</script>

<template>
  <fieldset aria-labelledby="entityType">
    <h2 id="entityType">Entiteitstype {{ !entityType.id ? `toevoegen` : `bijwerken` }}</h2>

    <div class="form-group">
      <label for="type">Entiteitstype *</label>

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

      <span id="typeError" class="error">Entiteitstype is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="entityTypeId">Entiteitstype ID *</label>

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

      <span id="entityTypeIdError" class="error">Entiteitstype ID is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="name">Entiteitstype naam *</label>

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

      <span id="nameError" class="error">Entiteitstype naam is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="uri">Entiteitstype URL</label>

      <input id="uri" type="text" v-model.trim="entityType.uri" />
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import type { EntityType } from "@/services/pabcService";

const entityType = defineModel<EntityType>("entityType", { required: true });

const MAXLENGTH = 256;
</script>

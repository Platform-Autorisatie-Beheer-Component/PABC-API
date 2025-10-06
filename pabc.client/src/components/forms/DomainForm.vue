<template>
  <div class="form-group">
    <label for="name">Domeinnaam *</label>

    <input
      id="name"
      type="text"
      v-model.trim="domain.name"
      :maxlength="MAXLENGTH"
      required
      aria-required="true"
      aria-describedby="nameError"
      :aria-invalid="!domain.name"
    />

    <span id="nameError" class="error">Naam is een verplicht veld</span>
  </div>

  <div class="form-group">
    <label for="description">Omschrijving *</label>

    <textarea
      id="description"
      rows="4"
      v-model.trim="domain.description"
      :maxlength="MAXLENGTH"
      required
      aria-required="true"
      aria-describedby="descriptionError"
      :aria-invalid="!domain.description"
    ></textarea>

    <span id="descriptionError" class="error">Omschrijving is een verplicht veld</span>
  </div>
</template>

<script setup lang="ts">
import type { Domain } from "@/services/pabcService";

const domain = defineModel<Domain>("domain", { required: true });

const MAXLENGTH = 256;
</script>

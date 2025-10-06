<template>
  <div class="form-group">
    <label for="name">Rolnaam *</label>

    <input
      id="name"
      type="text"
      v-model.trim="functionalRole.name"
      :maxlength="MAXLENGTH"
      required
      aria-required="true"
      aria-describedby="nameError"
      :aria-invalid="!functionalRole.name"
    />

    <span id="nameError" class="error">Naam is een verplicht veld</span>
  </div>
</template>

<script setup lang="ts">
import type { FunctionalRole } from "@/services/pabcService";

const functionalRole = defineModel<FunctionalRole>("functionalRole", { required: true });

const MAXLENGTH = 256;
</script>

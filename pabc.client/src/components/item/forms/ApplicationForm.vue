<template>
  <fieldset aria-labelledby="application">
    <h2 id="application">Applicatie {{ !application.id ? `toevoegen` : `bijwerken` }}</h2>

    <div class="form-group">
      <label for="name">Applicatienaam *</label>

      <input
        id="name"
        type="text"
        v-model.trim="application.name"
        :maxlength="MAXLENGTH"
        required
        aria-required="true"
        aria-describedby="nameError"
        :aria-invalid="!application.name"
      />

      <span id="nameError" class="error">Naam is een verplicht veld</span>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import type { Application } from "@/services/pabcService";

const application = defineModel<Application>("application", { required: true });

const MAXLENGTH = 256;
</script>

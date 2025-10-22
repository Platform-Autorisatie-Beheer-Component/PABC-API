<template>
  <fieldset aria-labelledby="applicationRole">
    <h2 id="applicationRole">
      Applicatierol {{ !applicationRole.id ? `toevoegen` : `bijwerken` }}
    </h2>

    <div class="form-group">
      <label for="name">Applicatie *</label>

      <input id="name"
             type="text"
             v-model.trim="applicationRole.application"
             :maxlength="MAXLENGTH"
             required
             aria-required="true"
             aria-describedby="applicationError"
             :aria-invalid="!applicationRole.application" />

      <span id="applicationError" class="error">Applicatie is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="name">Naam applicatierol *</label>

      <input id="name"
             type="text"
             v-model.trim="applicationRole.name"
             :maxlength="MAXLENGTH"
             required
             aria-required="true"
             aria-describedby="nameError"
             :aria-invalid="!applicationRole.name" />

      <span id="nameError" class="error">Naam is een verplicht veld</span>
    </div>

  </fieldset>
</template>

<script setup lang="ts">
import type { ApplicationRole } from "@/services/pabcService";

const applicationRole = defineModel<ApplicationRole>("applicationRole", { required: true });

const MAXLENGTH = 256;
</script>

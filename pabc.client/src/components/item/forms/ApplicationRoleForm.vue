<template>
  <fieldset aria-labelledby="applicationRole">
    <h2 id="applicationRole">
      Applicatierol {{ !applicationRole.id ? `toevoegen` : `bijwerken` }}
    </h2>

    <div class="form-group">
      <label for="applicationId">Applicatie *</label>

      <select
        name="applicationId"
        id="applicationId"
        v-model="applicationRole.applicationId"
        required
        aria-required="true"
        aria-describedby="applicationIdError"
        :aria-invalid="!applicationRole.applicationId"
      >
        <option v-if="!applicationRole.applicationId" value="">- Selecteer -</option>

        <option v-for="{ id, name } in applications" :key="id" :value="id">
          {{ name }}
        </option>
      </select>

      <span id="applicationIdError" class="error">Applicatie is een verplicht veld</span>
    </div>

    <div class="form-group">
      <label for="name">Naam applicatierol *</label>

      <input
        id="name"
        type="text"
        v-model.trim="applicationRole.name"
        :maxlength="MAXLENGTH"
        required
        aria-required="true"
        aria-describedby="nameError"
        :aria-invalid="!applicationRole.name"
      />

      <span id="nameError" class="error">Naam is een verplicht veld</span>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import { type DeepReadonly } from "vue";
import type { Application, ApplicationRole } from "@/services/pabcService";

defineProps<{ applications: DeepReadonly<Application[]> }>();

const applicationRole = defineModel<ApplicationRole>("applicationRole", { required: true });

const MAXLENGTH = 256;
</script>

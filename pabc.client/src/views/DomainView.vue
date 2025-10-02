<template>
  <h1>{{ id ? "Domein" : "Nieuw domein" }}</h1>

  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="submitItem">
    <alert-inline v-if="error">{{ error }}</alert-inline>

    <fieldset v-else>
      <legend>Domeingegevens</legend>

      <div class="form-group">
        <label for="name">Naam *</label>

        <input
          id="name"
          type="text"
          v-model.trim="form.name"
          :maxlength="MAXLENGTH"
          required
          aria-required="true"
          aria-describedby="nameError"
          :aria-invalid="!form.name"
        />

        <span id="nameError" class="error">Naam is een verplicht veld</span>
      </div>

      <div class="form-group">
        <label for="description">Omschrijving *</label>

        <textarea
          id="description"
          rows="4"
          v-model.trim="form.description"
          :maxlength="MAXLENGTH"
          required
          aria-required="true"
          aria-describedby="descriptionError"
          :aria-invalid="!form.description"
        ></textarea>

        <span id="descriptionError" class="error">Omschrijving is een verplicht veld</span>
      </div>
    </fieldset>

    <menu class="reset">
      <li>
        <router-link :to="{ name: 'domains' }" class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!error">
        <li v-if="id">
          <button type="button" class="button danger" @click="deleteItem(id)">
            Domein verwijderen
          </button>
        </li>

        <li>
          <button type="submit">Domein opslaan</button>
        </li>
      </template>
    </menu>

    <prompt-modal
      :dialog="confirmDialog"
      cancel-text="Nee, niet verwijderen"
      confirm-text="Ja, verwijderen"
    >
      <h2>Domein verwijderen</h2>

      <p>
        Weet je zeker dat je het domein <em>{{ form.name }}</em> wilt verwijderen?
      </p>
    </prompt-modal>
  </form>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import { domainService, type Domain } from "@/services/pabcService";
import { useItemForm } from "@/composables/use-item-form";

const MAXLENGTH = 256;

const { id } = defineProps<{ id?: string }>();

const { form, loading, error, fetchItem, submitItem, deleteItem, confirmDialog } =
  useItemForm<Domain>(domainService, "Domein");

onMounted(() => id && fetchItem(id));
</script>

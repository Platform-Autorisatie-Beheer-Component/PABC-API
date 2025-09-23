<template>
  <h1>{{ id ? "Domein" : "Nieuw domein" }}</h1>

  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="submitDomain">
    <alert-inline v-if="error">{{ error }}</alert-inline>

    <fieldset v-else>
      <legend>Domeingegevens</legend>

      <div class="form-group">
        <label for="name">Naam *</label>

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
    </fieldset>

    <menu class="reset">
      <li>
        <router-link :to="{ name: 'domains' }" class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!error">
        <li v-if="id">
          <button type="button" class="button danger" @click="deleteDomain">
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
        Weet je zeker dat je domein <em>{{ domain.name }}</em> wilt verwijderen?
      </p>
    </prompt-modal>
  </form>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import toast from "@/components/toast/toast";
import { pabcService, type Domain } from "@/services/pabcService";
import { knownErrorMessages } from "@/utils/fetchWrapper";

const MAXLENGTH = 255;

const { id } = defineProps<{ id?: string }>();

const router = useRouter();

const domain = ref<Domain>({ name: "", description: "" });

const loading = ref(false);
const error = ref("");

const confirmDialog = useConfirmDialog();

const fetchDomain = async () => {
  if (!id) return;

  loading.value = true;
  error.value = "";

  try {
    domain.value = await pabcService.getDomainById(id);
  } catch (err: unknown) {
    error.value = `Fout bij het ophalen van de domeingegevens - ${err}`;
  } finally {
    loading.value = false;
  }
};

const submitDomain = async () => {
  loading.value = true;

  try {
    if (!id) {
      await pabcService.createDomain(domain.value);
    } else {
      await pabcService.updateDomain(domain.value);
    }

    toast.add({ text: "Het domein is succesvol opgeslagen." });

    router.push({ name: "domains" });
  } catch (err: unknown) {
    if (err instanceof Error && err.message === knownErrorMessages.conflict) {
      toast.add({
        text: `Het domein '${domain.value.name}' bestaat al.`,
        type: "error"
      });
    } else {
      toast.add({ text: `Fout bij het opslaan van het domein - ${err}`, type: "error" });
    }
  } finally {
    loading.value = false;
  }
};

const deleteDomain = async () => {
  if (!id || (await confirmDialog.reveal()).isCanceled) return;

  loading.value = true;

  try {
    await pabcService.deleteDomain(id);

    toast.add({ text: "Het domein is succesvol verwijderd." });

    router.push({ name: "domains" });
  } catch (err: unknown) {
    toast.add({ text: `Fout bij het verwijderen van het domein - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchDomain());
</script>

<style lang="scss" scoped></style>

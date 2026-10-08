<template>
  <template v-if="enabled">
    <button type="button" class="button secondary" @click="openDialog">
      Importeren <icon-container icon="import" />
    </button>

    <dialog ref="importDialog" @cancel.prevent="handleClose">
      <button type="button" aria-label="Sluiten" class="dialog-close" @click="handleClose">
        <icon-container icon="xmark" />
      </button>

      <template v-if="!result">
        <h2>Zaaktypes importeren</h2>

        <p>Importeer zaaktypes vanuit het gekoppelde zaakregister.</p>

        <p>Zaaktypes die al bestaan in PABC worden overgeslagen.</p>

        <small-spinner v-if="loading" />

        <alert-inline v-if="error">{{ error }}</alert-inline>

        <menu v-if="!loading" class="reset">
          <li>
            <button type="button" class="button" @click="handleImport">Importeren</button>
          </li>

          <li>
            <button type="button" class="button secondary" @click="handleClose">Annuleren</button>
          </li>
        </menu>
      </template>

      <template v-else>
        <h2>Import resultaat</h2>

        <p>
          <strong>{{ result.created.length }}</strong> zaaktype(s) aangemaakt.
        </p>

        <details v-if="result.created.length" open>
          <summary>Aangemaakt ({{ result.created.length }})</summary>

          <ul>
            <li v-for="item in result.created" :key="item">{{ item }}</li>
          </ul>
        </details>

        <details v-if="result.skipped.length">
          <summary>Overgeslagen — bestonden al ({{ result.skipped.length }})</summary>

          <ul>
            <li v-for="item in result.skipped" :key="item">{{ item }}</li>
          </ul>
        </details>

        <details v-if="result.stale.length">
          <summary>Niet meer in zaakregister ({{ result.stale.length }})</summary>
          <ul>
            <li v-for="item in result.stale" :key="item">{{ item }}</li>
          </ul>
        </details>

        <menu class="reset">
          <li>
            <button type="button" class="button" @click="handleClose">Sluiten</button>
          </li>
        </menu>
      </template>
    </dialog>
  </template>
</template>

<script setup lang="ts">
import { onMounted, ref, useTemplateRef } from "vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import IconContainer from "@/components/IconContainer.vue";
import { importZaaktypesService, type ImportZaaktypesResponse } from "@/services/pabcService";

const emit = defineEmits<{ (e: "refresh"): void }>();

const enabled = ref(false);
const loading = ref(false);
const error = ref("");
const result = ref<ImportZaaktypesResponse | null>(null);

const importDialog = useTemplateRef("importDialog");

const openDialog = () => {
  result.value = null;
  error.value = "";
  importDialog.value?.showModal();
};

const handleImport = async () => {
  loading.value = true;
  error.value = "";

  try {
    result.value = await importZaaktypesService.import();
    emit("refresh");
  } catch (e) {
    error.value = e instanceof Error ? e.message : "Er is een fout opgetreden bij het importeren.";
  } finally {
    loading.value = false;
  }
};

const handleClose = () => {
  if (loading.value) return;

  importDialog.value?.close();
};

onMounted(async () => {
  try {
    const response = await importZaaktypesService.isEnabled();
    enabled.value = response.enabled;
  } catch {
    // Feature not available, hide button
  }
});
</script>

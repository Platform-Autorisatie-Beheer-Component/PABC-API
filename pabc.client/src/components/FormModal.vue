<template>
  <dialog ref="dialogRef" @cancel.prevent="$emit(`cancel`)">
    <form @submit.prevent="$emit(`confirm`)">
      <small-spinner v-if="loading" />

      <alert-inline v-else-if="error">{{ error }}</alert-inline>

      <slot v-else></slot>

      <menu class="reset">
        <li v-if="!loading && !error">
          <button type="submit" :class="['button', { danger: submitType === 'delete' }]">
            {{ SubmitTypes[submitType] }}
          </button>
        </li>

        <li>
          <button type="button" class="button secondary" @click="$emit(`cancel`)">Annuleren</button>
        </li>
      </menu>
    </form>
  </dialog>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";

const SubmitTypes = Object.freeze({
  create: "Toevoegen",
  update: "Bijwerken",
  delete: "Verwijderen"
});

const { isOpen, submitType, loading, error } = defineProps<{
  isOpen: boolean;
  submitType: keyof typeof SubmitTypes;
  loading?: boolean;
  error?: string;
}>();

const dialogRef = ref<HTMLDialogElement>();

watch(
  () => isOpen,
  (value) => (value ? dialogRef.value?.showModal() : dialogRef.value?.close())
);
</script>

<style lang="scss" scoped>
dialog {
  min-inline-size: 50%;
  padding: var(--spacing-large);
  border: 1px solid var(--border);
  border-radius: var(--radius-default);
}

::backdrop {
  background-color: rgb(102 102 102 / 80%);
}
</style>

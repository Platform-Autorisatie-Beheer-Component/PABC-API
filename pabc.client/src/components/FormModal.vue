<template>
  <dialog ref="dialogRef" @cancel.prevent="handleCancel">
    <form ref="formRef" @submit.prevent="handleSubmit">
      <small-spinner v-if="loading" />

      <alert-inline v-else-if="error">{{ error }}</alert-inline>

      <slot v-else></slot>

      <alert-inline v-if="invalid">{{ invalid }}</alert-inline>

      <menu v-if="!loading" class="reset">
        <li v-if="!error">
          <button type="submit" :class="['button', { danger: isDelete }]">
            {{ SubmitTypes[submitType] }}
          </button>
        </li>

        <li>
          <button type="button" class="button secondary" @click="handleCancel">Annuleren</button>
        </li>
      </menu>
    </form>
  </dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";

const SubmitTypes = {
  create: "Toevoegen",
  update: "Bijwerken",
  delete: "Verwijderen"
} as const;

type SubmitType = keyof typeof SubmitTypes;

const { isOpen, submitType } = defineProps<{
  isOpen: boolean;
  submitType: SubmitType;
  loading?: boolean;
  error?: string;
  invalid?: string;
}>();

const emit = defineEmits<{ (e: "submit"): void; (e: "cancel"): void }>();

const dialogRef = ref<HTMLDialogElement>();

const formRef = ref<HTMLFormElement>();

const isDelete = computed(() => submitType === ("delete" satisfies SubmitType));

watch(
  () => isOpen,
  (value) => (value ? dialogRef.value?.showModal() : dialogRef.value?.close())
);

const handleSubmit = () => emit("submit");

const handleCancel = () => {
  formRef.value?.reset();

  emit("cancel");
};
</script>

<style lang="scss" scoped>
dialog {
  inline-size: min(90vw, var(--section-width-medium));
  padding: var(--spacing-large);
  margin: auto;
  border: 1px solid var(--border);

  menu {
    margin-block-start: var(--spacing-default);
  }
}

::backdrop {
  background-color: rgb(102 102 102 / 80%);
}
</style>

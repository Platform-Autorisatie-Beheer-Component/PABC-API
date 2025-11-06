<template>
  <dialog ref="dialogRef" @cancel.prevent="handleCancel">
    <button type="button" aria-label="Sluiten" class="dialog-close" @click="handleCancel">
      <icon-container icon="xmark" />
    </button>

    <form ref="formRef" @submit.prevent="handleSubmit">
      <small-spinner v-if="loading" />

      <slot v-else></slot>

      <alert-inline v-if="invalid">{{ invalid }}</alert-inline>

      <menu v-if="!loading" class="reset">
        <li>
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
import { computed, ref } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import IconContainer from "@/components/IconContainer.vue";

const SubmitTypes = {
  create: "Toevoegen",
  update: "Bijwerken",
  delete: "Verwijderen"
} as const;

type SubmitType = keyof typeof SubmitTypes;

const { submitType, onSubmit } = defineProps<{
  submitType: SubmitType;
  onSubmit: () => unknown;
}>();

const loading = ref(false);

const invalid = ref("");

const dialogRef = ref<HTMLDialogElement>();

const formRef = ref<HTMLFormElement>();

const isDelete = computed(() => submitType === ("delete" satisfies SubmitType));

const promiseTry = <T,>(func: () => T | Promise<T>) =>
  new Promise<T>((resolve, reject) => {
    try {
      resolve(func());
    } catch (error) {
      reject(error);
    }
  });

const handleSubmit = () => {
  invalid.value = "";
  loading.value = true;
  return promiseTry(onSubmit)
    .then(() => {
      dialogRef.value?.close();
    })
    .catch((err) => {
      invalid.value = err instanceof Error ? err.message : err;
    })
    .finally(() => {
      loading.value = false;
    });
};

const handleCancel = () => {
  formRef.value?.reset();
  dialogRef.value?.close();
  invalid.value = "";
};

defineExpose({ open: () => dialogRef?.value?.showModal() });
</script>

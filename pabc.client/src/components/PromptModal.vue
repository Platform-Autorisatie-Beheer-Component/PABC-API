<template>
  <dialog ref="dialogRef" @close="onClose">
    <form method="dialog">
      <slot></slot>

      <menu class="reset">
        <li>
          <button type="submit" value="cancel" class="button secondary">
            {{ cancelText }}
          </button>
        </li>

        <li>
          <button type="submit" value="confirm">
            {{ confirmText }}
          </button>
        </li>
      </menu>
    </form>
  </dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { whenever, type UseConfirmDialogReturn } from "@vueuse/core";

const {
  dialog,
  cancelText = "Nee",
  confirmText = "Ja"
} = defineProps<{
  dialog: UseConfirmDialogReturn<unknown, unknown, unknown>;
  cancelText?: string;
  confirmText?: string;
}>();

const dialogRef = ref<HTMLDialogElement>();

const onClose = () => {
  if (dialogRef.value?.returnValue === "confirm") {
    dialog.confirm();
  } else {
    dialog.cancel();
  }
};

whenever(
  () => dialog.isRevealed.value,
  () => {
    dialogRef.value?.showModal();
  },
  { immediate: true }
);
</script>

<style lang="scss" scoped>
menu {
  display: flex;
  gap: var(--spacing-default);
  justify-content: flex-end;
}

form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);

  > * {
    margin-block: 0;
  }
}

dialog {
  min-width: 33%;
  padding: var(--spacing-large);
  border: 1px solid var(--border);
  border-radius: var(--radius-default);
}

::backdrop {
  background-color: rgb(102 102 102 / 80%);
}
</style>

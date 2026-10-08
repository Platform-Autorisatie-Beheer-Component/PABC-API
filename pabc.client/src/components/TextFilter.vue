<template>
  <search class="filter" role="search">
    <input
      v-model="model"
      name="filter"
      type="text"
      ref="inputRef"
      :aria-label="label"
      :placeholder="label"
      :maxlength="maxlength"
    />

    <button v-if="model" type="button" @click="clear">
      <icon-container icon="xmark" />

      <span class="visually-hidden">Filter wissen</span>
    </button>
  </search>
</template>

<script setup lang="ts">
import { ref } from "vue";
import IconContainer from "@/components/IconContainer.vue";

const model = defineModel<string>({ required: true });

const { label, maxlength = 256 } = defineProps<{
  label: string;
  maxlength?: number;
}>();

const inputRef = ref<HTMLInputElement | null>(null);

const clear = () => {
  model.value = "";
  inputRef.value?.focus();
};
</script>

<style lang="scss" scoped>
.filter {
  display: grid;
  margin-block-end: var(--spacing-default);

  > * {
    grid-area: 1 / 1;
  }

  input {
    padding-inline-end: var(--spacing-large);
    margin: 0;
  }

  button {
    justify-self: end;
    align-self: center;
    color: inherit;
    font-size: var(--font-small);
    margin: 0;
    border: none;
    background: none;
  }
}
</style>

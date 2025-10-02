<template>
  <span class="icon-container" role="presentation" aria-hidden="true">
    <svg fill="currentColor">
      <use :xlink:href="`${sprite}#${icon}`"></use>
    </svg>
  </span>
</template>

<script setup lang="ts">
import sprite from "@/assets/icon-sprite.svg";

defineProps<{ icon: string }>();
</script>

<style lang="scss" scoped>
.icon-container {
  --_icon-size: calc(1em * var(--line-height));

  display: inline-block;
  position: relative;
  font-size: var(--_icon-size);
  block-size: var(--_icon-size);
  inline-size: var(--_icon-size);

  svg {
    block-size: 100%;
    inline-size: 100%;
  }
}
</style>

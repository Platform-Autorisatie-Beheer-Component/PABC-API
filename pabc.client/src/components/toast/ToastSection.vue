<template>
  <transition-group name="toast" tag="section">
    <output
      v-for="(message, key) in messages"
      :key="key"
      role="status"
      :class="message.type"
      @click="toast.remove(message)"
      ><div v-html="message.text"></div
    ></output>
  </transition-group>
</template>

<script lang="ts" setup>
import toast, { messages } from "./toast";
</script>

<style scoped lang="scss">
section {
  position: fixed;
  z-index: 2;
  display: grid;
  justify-items: center;
  justify-content: center;
  gap: var(--spacing-default);
  pointer-events: none;
  inset-block-start: min(50vh, 10rem);
  inset-inline: 0;
  padding: 0;
  margin: 0;
  border: none;
}

output {
  display: grid;
  grid: auto-flow / auto 1fr auto;
  column-gap: var(--spacing-default);
  text-align: center;
  padding: var(--spacing-default);
  margin-inline: var(--spacing-default);
  border: 1px solid var(--border);
  pointer-events: auto;
}

.toast-move,
.toast-enter-active,
.toast-leave-active {
  transition: all 0.5s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
}

.toast-leave-active {
  position: absolute;
}

.confirm {
  background-color: var(--success);
}

.error {
  cursor: pointer;
  color: var(--danger-text);
  background-color: var(--danger-hover);
}
</style>

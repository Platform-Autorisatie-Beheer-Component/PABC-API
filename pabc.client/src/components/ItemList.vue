<template>
  <ul class="reset">
    <li v-for="item in items" :key="item.id" class="list-item">
      <section>
        <slot name="item" :item="item"></slot>
      </section>

      <menu class="reset" v-if="item.id">
        <li v-if="hasUpdateListener">
          <button type="button" class="button secondary" @click="$emit('update', item.id)">
            <icon-container icon="pen" />

            <span class="visually-hidden">Item aanpassen</span>
          </button>
        </li>

        <li v-if="hasDeleteListener">
          <button type="button" class="button danger" @click="$emit('delete', item.id)">
            <icon-container icon="trash" />

            <span class="visually-hidden">Item verwijderen</span>
          </button>
        </li>
      </menu>
    </li>
  </ul>
</template>

<script setup lang="ts" generic="T extends Item">
import { computed, useAttrs, type DeepReadonly } from "vue";
import IconContainer from "@/components/IconContainer.vue";
import type { Item } from "@/services/pabcService";

const { items } = defineProps<{ items: DeepReadonly<T[]> }>();

const attrs = useAttrs();

const hasUpdateListener = computed(() => "onUpdate" in attrs);
const hasDeleteListener = computed(() => "onDelete" in attrs);
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.list-item {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
  padding-block-start: var(--spacing-small);
  margin-block-end: var(--spacing-small);
  border-block-start: 1px solid var(--secondary);

  @media (min-width: variables.$breakpoint-md) {
    & {
      flex-direction: row;
      align-items: center;
    }
  }

  section {
    flex: 1;
    display: flex;
    flex-direction: column;
    row-gap: var(--spacing-small);
  }

  menu {
    display: flex;
    column-gap: var(--spacing-small);

    button {
      margin-block-end: 0;
    }
  }
}
</style>

<template>
  <details open>
    <summary>{{ itemName }}</summary>

    <small-spinner v-if="loading" />

    <alert-inline v-else-if="error">{{ error }}</alert-inline>

    <p v-else-if="!items.length">Geen {{ itemName }} gevonden.</p>

    <template v-else>
      <p>
        <button type="button" class="button secondary" @click="$emit(`handleAdd`)">
          <icon-container icon="plus" /> Toevoegen
        </button>
      </p>

      <ul class="reset">
        <li v-for="item in items" :key="item.id">
          <section>
            <slot name="item" :item="item"></slot>
          </section>

          <menu class="reset">
            <li>
              <button type="button" class="button secondary" @click="handleEdit(item.id)">
                <icon-container icon="pen" />

                <span class="visually-hidden">Item aanpassen</span>
              </button>
            </li>

            <li>
              <button type="button" class="button danger" @click="$emit(`handleDelete`)">
                <icon-container icon="trash" />

                <span class="visually-hidden">Item verwijderen</span>
              </button>
            </li>
          </menu>
        </li>
      </ul>
    </template>
  </details>

  <prompt-modal :dialog="formDialog" cancel-text="Annuleren" confirm-text="Toevoegen">
    <form @submit.prevent="submitItem">
      <slot name="form" :form="form"></slot>
    </form>
  </prompt-modal>
</template>

<script setup lang="ts" generic="T extends Item">
import { computed, onMounted } from "vue";
import { useConfirmDialog } from "@vueuse/core";
import AlertInline from "@/components/AlertInline.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import type { Item, PabcService } from "@/services/pabcService";
import { useItemList } from "@/composables/use-item-list";
import { useItemForm } from "@/composables/use-item-form";

const { pabcService, itemName } = defineProps<{ pabcService: PabcService<T>; itemName: string }>();

const formDialog = useConfirmDialog();

const loading = computed(() => listLoading.value || formLoading.value);
const error = computed(() => listError.value || formError.value);

const {
  items,
  loading: listLoading,
  error: listError,
  fetchItems
} = useItemList(pabcService, itemName);

const {
  form,
  loading: formLoading,
  error: formError,
  fetchItem,
  submitItem
} = useItemForm<T>(pabcService, itemName);

const handleEdit = async (id?: string) => {
  if (!id) return;

  await fetchItem(id);

  await formDialog.reveal();
};

onMounted(() => fetchItems());
</script>

<style lang="scss" scoped>
ul > li {
  display: flex;
  align-items: flex-end;
  gap: var(--spacing-large);
  padding-block-start: var(--spacing-small);
  margin-block-end: var(--spacing-small);
  border-top: 1px solid var(--border);

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
